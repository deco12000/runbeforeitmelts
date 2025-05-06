using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherOcclusion : MonoBehaviour
{
    public Transform player; // 플레이어 트랜스폼
    public LayerMask occluderLayer; // 가려지는 오브젝트 레이어
    public Material ditherMaterial; // 디더링 머티리얼 (DitherLit 머티리얼을 인스펙터에서 지정)
    public float maxFadeTime = 1.0f; // 최대 페이드 시간
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 페이드 커브
    public float checkInterval = 0.05f; // 체크 간격

    private Dictionary<Renderer, DitherState> ditherStates = new();
    private MaterialPropertyBlock mpb;
    private float timer = 0f;
    private Camera camera;
    private List<Renderer> occludingObjects = new(); // 가려진 오브젝트 리스트

    class DitherState
    {
        public float fadeTimer = 0f; // 페이드 타이머
        public bool isBlocking = false; // 막고 있는지 여부
        public Material[] originalMaterials = null; // 원본 머티리얼
        public bool isMaterialSwapped = false; // 머티리얼이 교체되었는지 여부
        public Color originalColor; // 원본 색상 저장 (베이스 색상의 알파값을 수정하기 위해 사용)
        public Texture originalBaseMap; // 원본 베이스 텍스처 저장
        public Texture originalNormalMap; // 원본 노멀맵 저장
        public Coroutine fadeCoroutine; // 페이드 코루틴
    }

    void Start()
    {
        mpb = new MaterialPropertyBlock(); // 머티리얼 프로퍼티 블록 초기화
        camera = Camera.main; // 메인 카메라 가져오기
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckOcclusion(); // 가려진 오브젝트 체크
        }

        // 상태 업데이트
        List<Renderer> toRemove = new();

        foreach (var kvp in ditherStates)
        {
            var renderer = kvp.Key;
            var state = kvp.Value;

            if (renderer == null)
            {
                toRemove.Add(renderer);
                continue;
            }

            // 상태가 비활성화되면 디더링 머티리얼을 복원
            if (!state.isBlocking && state.fadeTimer <= 0 && state.isMaterialSwapped && !occludingObjects.Contains(renderer))
            {
                renderer.sharedMaterials = state.originalMaterials;
                state.originalMaterials = null;
                state.isMaterialSwapped = false;

                // 페이드 코루틴이 있으면 멈추고 복원
                if (state.fadeCoroutine != null)
                {
                    StopCoroutine(state.fadeCoroutine);
                    state.fadeCoroutine = null;
                }
            }

            state.isBlocking = false;

            // 완전히 복원되면 상태 제거
            if (state.fadeTimer <= 0 && !state.isMaterialSwapped && !state.isBlocking)
                toRemove.Add(renderer);
        }

        foreach (var r in toRemove)
            ditherStates.Remove(r);
    }

    // 가려지는 오브젝트 체크
    void CheckOcclusion()
    {
        Vector3 camPos = camera.transform.position;
        Vector3 dir = (player.position + Vector3.up) - camPos;
        float dist = dir.magnitude * 0.9f;
        RaycastHit[] hits = Physics.RaycastAll(camPos, dir, dist, occluderLayer);

        // 가려진 오브젝트 체크
        foreach (var hit in hits)
        {
            var rend = hit.transform.GetComponent<Renderer>();
            if (rend == null) continue;

            // 상태가 없으면 추가
            if (!ditherStates.ContainsKey(rend))
                ditherStates[rend] = new DitherState();

            // 막고 있다는 상태로 설정
            if (!ditherStates[rend].isBlocking)
            {
                // 디더링 머티리얼로 교체와 알파값 변경을 코루틴으로 처리
                ditherStates[rend].fadeCoroutine = StartCoroutine(FadeInDither(rend, ditherStates[rend]));
            }

            ditherStates[rend].isBlocking = true;
            if (!occludingObjects.Contains(rend)) occludingObjects.Add(rend); // 가려진 오브젝트 리스트에 추가
        }

        // 더 이상 가려지지 않는 오브젝트 처리
        List<Renderer> toRemove = new();
        foreach (var occludedRenderer in occludingObjects)
        {
            bool isOccluding = false;

            foreach (var hit in hits)
            {
                if (hit.transform.GetComponent<Renderer>() == occludedRenderer)
                {
                    isOccluding = true;
                    break;
                }
            }

            if (!isOccluding)
            {
                // 가려지지 않으면 원래 상태로 복원
                ditherStates[occludedRenderer].isBlocking = false;
                toRemove.Add(occludedRenderer);
            }
        }

        foreach (var r in toRemove)
        {
            occludingObjects.Remove(r);
        }
    }

    // 디더링 페이드 인 코루틴
    IEnumerator FadeInDither(Renderer renderer, DitherState state)
    {
        // 원본 머티리얼과 색상 저장
        state.originalMaterials = renderer.sharedMaterials;
        state.originalColor = renderer.material.color;
        state.originalBaseMap = renderer.material.GetTexture("_BaseMap");
        state.originalNormalMap = renderer.material.GetTexture("_BumpMap");

        var newMats = new Material[state.originalMaterials.Length];
        for (int i = 0; i < newMats.Length; i++)
        {
            newMats[i] = ditherMaterial;
            newMats[i].SetTexture("_BaseMap", state.originalBaseMap);
            newMats[i].SetTexture("_BumpMap", state.originalNormalMap);
            newMats[i].SetColor("_BaseColor", state.originalColor);
        }
        renderer.sharedMaterials = newMats;

        state.isMaterialSwapped = true;

        float fadeTimer = 0f;
        while (fadeTimer < maxFadeTime)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / maxFadeTime);
            Color currentColor = state.originalColor;
            currentColor.a = Mathf.Lerp(1f, 0.2f, t);
            renderer.material.color = currentColor;

            yield return null;
        }
    }
}
