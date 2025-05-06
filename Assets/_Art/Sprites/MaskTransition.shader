Shader "UI/MaskTransition"
{
    Properties
    {
        _MainTex ("Ink Mask (Grayscale)", 2D) = "white" {}
        _Threshold ("Threshold", Range(0,1)) = 0
        _Smoothness ("Smooth Range", Range(0.001, 0.2)) = 0.05
        _Color ("Fade Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float _Smoothness;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 텍스처에서 마스크 값을 가져옴
                float mask = tex2D(_MainTex, i.uv).r;

                // Threshold 값을 기준으로 마스크를 부드럽게 처리 (페이드 효과)
                float alpha = smoothstep(_Threshold - _Smoothness, _Threshold + _Smoothness, mask);

                // 페이드 색상과 alpha 값을 결합하여 최종 색상 반환
                return fixed4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
