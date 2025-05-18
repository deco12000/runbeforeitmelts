// Unity built - in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Modified for transparency based on _Alpha property -- FOR BUILT-IN RENDER PIPELINE ONLY!
Shader "MyShader/SphericalBG"
{
    Properties
    {
        _Tint ("Tint Color", Color) = (.5, .5, .5, 1.0) // 색상 조절 용도
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation", Range(0, 360)) = 0
        [NoScaleOffset] _MainTex ("Spherical  (HDR)", 2D) = "grey" {}
        // Mapping 속성: 6 Frames Layout (0) 또는 Latitude Longitude Layout (1) - Material 인스펙터에서 선택
        [KeywordEnum(6 Frames Layout, Latitude Longitude Layout)] _Mapping("Mapping", Float) = 1
        [Enum(360 Degrees, 0, 180 Degrees, 1)] _ImageType("Image Type", Float) = 0
        [Toggle] _MirrorOnBack("Mirror on Back", Float) = 0
        [Enum(None, 0, Side by Side, 1, Over Under, 2)] _Layout("3D Layout", Float) = 0

        // 새로운 투명도 조절 속성
        _Alpha ("Transparency (Alpha)", Range(0, 1)) = 1.0 // 0 (완전 투명) ~ 1 (완전 불투명)
    }

    SubShader {
        // 투명도를 위한 Tags, Queue, RenderType 변경
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "PreviewType" = "Skybox" }

        // 투명도를 위해 ZWrite Off 유지, Cull Off 유지
        Cull Off ZWrite Off

        Pass {
            // 투명도를 위한 블렌딩 모드 추가
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            // multi_compile_local: _Mapping 속성에 따라 __ (LatLong) 또는 _MAPPING_6_FRAMES_LAYOUT 키워드 활성화
            #pragma multi_compile_local __ _MAPPING_6_FRAMES_LAYOUT

            // Built-in 쉐이더 라이브러리 포함
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            half4 _MainTex_HDR;
            half4 _Tint;
            half _Exposure;
            float _Rotation;
            float _Alpha; // 투명도 속성 변수

            #ifndef _MAPPING_6_FRAMES_LAYOUT
            bool _MirrorOnBack;
            int _ImageType;
            int _Layout;
            #endif

            // Latitude Longitude Layout (파노라마) 매핑 함수
            // _MAPPING_6_FRAMES_LAYOUT 키워드가 정의되지 않았을 때만 이 함수 정의 포함
            #ifndef _MAPPING_6_FRAMES_LAYOUT
            inline float2 ToRadialCoords(float3 coords)
            {
                float3 normalizedCoords = normalize(coords);
                float latitude = acos(normalizedCoords.y);
                float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
                return float2(0.5 - longitude * 0.5/UNITY_PI, 1.0 - latitude * 1.0/UNITY_PI); // Built-in 원본 매핑
            }
            #endif

            // 6 Frames Layout (큐브맵) 매핑 함수
            // _MAPPING_6_FRAMES_LAYOUT 키워드가 정의되었을 때만 이 함수 정의 포함
            #ifdef _MAPPING_6_FRAMES_LAYOUT
            inline float2 ToCubeCoords(float3 coords, float3 layout, float4 edgeSize, float4 faceXCoordLayouts, float4 faceYCoordLayouts, float4 faceZCoordLayouts)
            {
                // Determine the primary axis of the normal
                float3 absn = abs(coords);
                float3 absdir = absn > float3(max(absn.y, absn.z), max(absn.x, absn.z), max(absn.x, absn.y)) ? 1 : 0;
                // Convert the normal to a local face texture coord [ - 1, + 1], note that tcAndLen.z == dot(coords, absdir)
                // and thus its sign tells us whether the normal is pointing positive or negative
                float3 tcAndLen = mul(absdir, float3x3(coords.zyx, coords.xzy, float3(- coords.xy, coords.z)));
                tcAndLen.xy /= tcAndLen.z;
                // Flip - flop faces for proper orientation and normalize to [ - 0.5, + 0.5]
                bool2 positiveAndVCross = float2(tcAndLen.z, layout.x) > 0;
                tcAndLen.xy *= (positiveAndVCross[0] ? absdir.yx : (positiveAndVCross[1] ? float2(absdir[2], 0) : float2(0, absdir[2]))) - 0.5;
                // Clamp values which are close to the face edges to avoid bleeding / seams (ie. enforce clamp texture wrap mode)
                tcAndLen.xy = clamp(tcAndLen.xy, edgeSize.xy, edgeSize.zw);
                // Scale and offset texture coord to match the proper square in the texture based on layout.
                float4 coordLayout = mul(float4(absdir, 0), float4x4(faceXCoordLayouts, faceYCoordLayouts, faceZCoordLayouts, faceZCoordLayouts));
                tcAndLen.xy = (tcAndLen.xy + (positiveAndVCross[0] ? coordLayout.xy : coordLayout.zw)) * layout.yz;
                return tcAndLen.xy;
            }
            #endif // _MAPPING_6_FRAMES_LAYOUT

            // Y축 회전 함수 (도 단위)
            float3 RotateAroundYInDegrees (float3 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, - sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            // 정점 쉐이더 입력 구조체
            struct appdata_t {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // 정점 쉐이더 출력 구조체
            struct v2f {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0; // 프래그먼트로 전달할 정점 위치 (방향 벡터 계산에 사용)
                #ifdef _MAPPING_6_FRAMES_LAYOUT
                float3 layout : TEXCOORD1;
                float4 edgeSize : TEXCOORD2;
                float4 faceXCoordLayouts : TEXCOORD3;
                float4 faceYCoordLayouts : TEXCOORD4;
                float4 faceZCoordLayouts : TEXCOORD5;
                #else // Latitude Longitude Layout
                float2 image180ScaleAndCutoff : TEXCOORD1;
                float4 layout3DScaleAndOffset : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // 정점 쉐이더 함수 (vert)
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Y축 회전 적용
                float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
                // 회전된 정점 위치를 클립 공간으로 변환
                o.vertex = UnityObjectToClipPos(rotated);

                // 원본 정점 위치를 텍스처 좌표로 전달 (프래그먼트에서 방향 벡터로 사용)
                o.texcoord = v.vertex.xyz;

                // 6 Frames Layout 또는 Latitude Longitude Layout 관련 계산 (프래그먼트에서 사용)
                #ifdef _MAPPING_6_FRAMES_LAYOUT
                // layout and edgeSize are solely based on texture dimensions and can thus be precalculated in the vertex shader.
                float sourceAspect = float(_MainTex_TexelSize.z) / float(_MainTex_TexelSize.w);
                // Use the halfway point between the 1 : 6 and 3 : 4 aspect ratios of the strip and cross layouts to
                // guess at the correct format.
                bool3 aspectTest = sourceAspect > float3(1.0, 1.0f / 6.0f + (3.0f / 4.0f - 1.0f / 6.0f) / 2.0f, 6.0f / 1.0f + (4.0f / 3.0f - 6.0f / 1.0f) / 2.0f);
                // For a given face layout, the coordinates of the 6 cube faces are fixed : build a compact representation of the
                // coordinates of the center of each face where the first float4 represents the coordinates of the X axis faces,
                // the second the Y, and the third the Z. The first two float componenents (xy) of each float4 represent the face
                // coordinates on the positive axis side of the cube, and the second (zw) the negative.
                // layout.x is a boolean flagging the vertical cross layout (for special handling of flip - flops later)
                // layout.yz contains the inverse of the layout dimensions (ie. the scale factor required to convert from
                // normalized face coords to full texture coordinates)
                if (aspectTest[0]) // horizontal
                {
                    if (aspectTest[2])
                    { // horizontal strip
                        o.faceXCoordLayouts = float4(0.5, 0.5, 1.5, 0.5);
                        o.faceYCoordLayouts = float4(2.5, 0.5, 3.5, 0.5);
                        o.faceZCoordLayouts = float4(4.5, 0.5, 5.5, 0.5);
                        o.layout = float3(- 1, 1.0 / 6.0, 1.0 / 1.0);
                    }
                    else
                    { // horizontal cross
                        o.faceXCoordLayouts = float4(2.5, 1.5, 0.5, 1.5);
                        o.faceYCoordLayouts = float4(1.5, 2.5, 1.5, 0.5);
                        o.faceZCoordLayouts = float4(1.5, 1.5, 3.5, 1.5);
                        o.layout = float3(- 1, 1.0 / 4.0, 1.0 / 3.0);
                    }
                }
                else
                {
                    if (aspectTest[1])
                    { // vertical cross
                        o.faceXCoordLayouts = float4(2.5, 2.5, 0.5, 2.5);
                        o.faceYCoordLayouts = float4(1.5, 3.5, 1.5, 1.5);
                        o.faceZCoordLayouts = float4(1.5, 2.5, 1.5, 0.5);
                        o.layout = float3(1, 1.0 / 3.0, 1.0 / 4.0);
                    }
                    else
                    { // vertical strip
                        o.faceXCoordLayouts = float4(0.5, 5.5, 0.5, 4.5);
                        o.faceYCoordLayouts = float4(0.5, 3.5, 0.5, 2.5);
                        o.faceZCoordLayouts = float4(0.5, 1.5, 0.5, 0.5);
                        o.layout = float3(- 1, 1.0 / 1.0, 1.0 / 6.0);
                    }
                }
                o.edgeSize.xy = _MainTex_TexelSize.xy * 0.5 / o.layout.yz - 0.5;
                o.edgeSize.zw = - o.edgeSize.xy;
                #else // Latitude Longitude Layout
                // 180도 이미지 스케일 및 오프셋 계산
                if (_ImageType == 0)
                o.image180ScaleAndCutoff = float2(1.0, 1.0);
                else
                o.image180ScaleAndCutoff = float2(2.0, _MirrorOnBack ? 1.0 : 0.5);
                // 3D 레이아웃 스케일 및 오프셋 계산
                if (_Layout == 0)
                o.layout3DScaleAndOffset = float4(0, 0, 1, 1);
                else if (_Layout == 1)
                o.layout3DScaleAndOffset = float4(unity_StereoEyeIndex, 0, 0.5, 1);
                else
                o.layout3DScaleAndOffset = float4(0, 1 - unity_StereoEyeIndex, 1, 0.5);
                #endif

                return o;
            }

            // 프래그먼트 쉐이더 함수 (frag)
            fixed4 frag (v2f i) : SV_Target
            {
                float2 tc;
                #ifdef _MAPPING_6_FRAMES_LAYOUT
                // 6 Frames Layout UV 계산 - _MAPPING_6_FRAMES_LAYOUT 키워드 활성화 시 컴파일
                tc = ToCubeCoords(i.texcoord, i.layout, i.edgeSize, i.faceXCoordLayouts, i.faceYCoordLayouts, i.faceZCoordLayouts);
                #else // Latitude Longitude Layout
                // 파노라마 UV 계산 - _MAPPING_6_FRAMES_LAYOUT 키워드 비활성화 시 컴파일
                tc = ToRadialCoords(i.texcoord);
                // 180도 이미지 타입 처리 및 가장자리 잘라내기 (alpha test)
                if (tc.x > i.image180ScaleAndCutoff[1])
                     discard; // 180도 이미지 가장자리 픽셀 버림
                tc.x = fmod(tc.x * i.image180ScaleAndCutoff[0], 1);
                tc = (tc + i.layout3DScaleAndOffset.xy) * i.layout3DScaleAndOffset.zw;
                #endif

                // 텍스처 샘플링
                half4 tex = tex2D (_MainTex, tc);
                // HDR 디코딩
                half3 c = DecodeHDR (tex, _MainTex_HDR);
                // Tint 색상 및 색 공간 적용 (Tint의 RGB만 사용)
                c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb; // _Tint.rgb 사용
                // 노출 적용
                c *= _Exposure;

                // 새로운 _Alpha 속성 값으로 최종 알파 설정
                return half4(c, _Alpha); // _Alpha 속성 값으로 최종 알파 리턴
            }
            ENDCG
        }
    }

    // CustomEditor 제거 (URP에서는 필요 없음)
    // CustomEditor "SkyboxPanoramicShaderGUI"

    // Fallback 쉐이더 제거 (URP에서는 FallbackError 사용 권장)
    // Fallback Off

}