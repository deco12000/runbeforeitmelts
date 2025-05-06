using System.Collections.Generic;
using System.Collections;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
public class StageControl : MonoBehaviour
{
    #region UniTask Setting
    CancellationTokenSource cts;
    void OnEnable()
    {
        cts = new CancellationTokenSource();
        Application.quitting += () => UniTaskCancel();
    }
    void OnDisable() { UniTaskCancel(); }
    void OnDestroy() { UniTaskCancel(); }
    void UniTaskCancel()
    {
        try
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        catch (System.Exception e)
        {

            Debug.Log(e);
        }
        cts = null;
    }
    #endregion
    IEnumerator Start()
    {
        camTr = Player.Instance.cam.transform;
        cam = Player.Instance.cam;
        EventHub.Instance.Register<EventScrollStart>(OnScrollStart);
        EventHub.Instance.Register<EventScrollPause>(OnScrollPause);
        //
        PoolBehaviour pb;
        Track track;
        pb = PoolManager.Instance.Spawn(tracks[0], new Vector3(0, 0, -100), Quaternion.identity, transform);
        track = pb as Track;
        activeTracks.Add(track);
        pb = PoolManager.Instance.Spawn(tracks[1], Vector3.zero, Quaternion.identity, transform);
        track = pb as Track;
        activeTracks.Add(track);
        currTrack = track;
        currTrack.SpawnObstacles();
        currTrack.SpawnItems();
        CheckCurrTrackInfo();
        //
        yield return YieldInstructionCache.WaitForSeconds(4f);
        SpawnRainLoop(cts.Token).Forget();
    }
    void Update()
    {
        CheckSelectPath();
        ScrollTrack();
    }

    // 이 밑으로는 Track 관련
    Transform camTr;
    PlayerCamera cam;
    public bool isPause = true;
    public List<Track> tracks = new List<Track>();
    public List<Track> activeTracks = new List<Track>();
    public Track currTrack;
    int pathCount;
    public Vector2Int selectPath;
    public Vector3[,] camPivotLPoss = new Vector3[1, 60];
    public Vector3[,] camPivotLRots = new Vector3[1, 60];
    [SerializeField] TMP_Text textDistance;
    void CheckCurrTrackInfo()
    {
        pathCount = currTrack.camPivot.Length;
        camPivotLPoss = new Vector3[pathCount, 60];
        camPivotLRots = new Vector3[pathCount, 60];
        for (int j = 0; j < pathCount; j++)
        {
            GameObject sample = currTrack.camPivot[j].gameObject;
            for (int i = 0; i < 60; i++)
            {
                float t = (i / (float)(60 - 1));
                currTrack.camPivot[j].clip.SampleAnimation(sample, t);
                camPivotLPoss[j, i] = sample.transform.localPosition;
                camPivotLRots[j, i] = sample.transform.localRotation.eulerAngles;
            }
        }
        cam.trackForward = currTrack.transform.forward;
        cam.camPivotLRots = camPivotLRots;
    }
    void CheckSelectPath()
    {
        float minDistance = 99999f;
        Vector2Int find = Vector2Int.zero;
        Vector3 findPoint = Vector3.zero;
        for (int i = 0; i < pathCount; i++)
            for (int j = 0; j < 60; j++)
            {
                Vector3 point = currTrack.transform.position 
                + currTrack.transform.right * camPivotLPoss[i, j].x 
                + currTrack.transform.up * camPivotLPoss[i, j].y
                + currTrack.transform.forward * camPivotLPoss[i, j].z;
                float distance = (camTr.position - point).sqrMagnitude;
                if (distance < minDistance)
                {
                    find = new Vector2Int(i, j);
                    minDistance = distance;
                    findPoint = point;
                }
                //Debug.DrawLine(camTr.position, point);
            }
        //DebugExtension.DebugWireSphere(findPoint,Color.blue,1f);
        selectPath = find;
        cam.selectPath = selectPath;
        float add = 0f;
        if(selectPath.y < 59)
        {
            for(int i=0; i<selectPath.y; i++)
            {
                add += Vector3.Distance(camPivotLPoss[selectPath.x,i] , camPivotLPoss[selectPath.x,i+1]);
            }
        }
    }
    void OnScrollStart(EventData ed)
    {
        isPause = false;
    }
    void OnScrollPause(EventData ed)
    {
        isPause = true;
    }
    void ScrollTrack()
    {
        if (isPause)
        {
            return;
        }
        int sp_y = selectPath.y;
        if (selectPath.y == 59) sp_y -= 1;
        Vector3 dir = currTrack.transform.rotation * (camPivotLPoss[selectPath.x, sp_y] - camPivotLPoss[selectPath.x, sp_y + 1]);
        foreach (Track track in activeTracks)
        {
            track.transform.position += GameManager.Instance.scrollSpeed * Time.deltaTime * dir.normalized;
        }
        if(count < selectPath.y)
        {
            count = selectPath.y;
            float add = 0f;
            for(int i=0; i<count-1; i++)
            {
                add += Vector3.Distance(camPivotLPoss[selectPath.x, i],camPivotLPoss[selectPath.x, i+1]);
            }
            GameManager.Instance.trackDistance = add;
            float temp = GameManager.Instance.distance + GameManager.Instance.trackDistance;
            textDistance.text = $"{temp/10f:F1} m";
        }
        if (selectPath.y >= 25 && !_bool1)
        {
            _bool1 = true;
            SpawnTrack();
        }
        if (selectPath.y >= 56)
        {
            float distance1 = (camTr.position - (currTrack.transform.position + camPivotLPoss[selectPath.x, 55])).sqrMagnitude;
            float distance2 = (camTr.position - activeTracks[activeTracks.Count - 1].transform.position).sqrMagnitude;
            //Debug.DrawLine(camTr.position, (currTrack.transform.position + camPivotLPoss[selectPath.x, 55]), Color.red);
            //Debug.DrawLine(camTr.position, activeTracks[activeTracks.Count - 1].transform.position, Color.blue);
            if (distance1 > distance2)
            {
                selectPath = Vector2Int.zero;
                float add = 0f;
                for(int i=0; i<59; i++)
                {
                    add += Vector3.Distance(camPivotLPoss[selectPath.x, i],camPivotLPoss[selectPath.x, i+1]);
                }
                GameManager.Instance.distance += add;
                float temp = GameManager.Instance.distance + GameManager.Instance.trackDistance;
                textDistance.text = $"{temp/10f:F1} m";
                currTrack = activeTracks[activeTracks.Count - 1];
                _bool1 = false;
                count = 0;
                GameManager.Instance.trackDistance = 0;
                CheckCurrTrackInfo();
            }
        }
    }
    bool _bool1 = false;
    int count;
    void SpawnTrack()
    {
        if (activeTracks.Count != 2) return;
        Track first = activeTracks[0];
        first.DeSpawn();
        activeTracks.RemoveAt(0);
        Track last = activeTracks[activeTracks.Count - 1];
        PoolBehaviour pb = PoolManager.Instance.Spawn(tracks[2], last.endPivot[selectPath.x].position, last.endPivot[selectPath.x].rotation, transform);
        Track track = pb as Track;
        activeTracks.Add(track);
    }
    // Rain

    [SerializeField] GameObject obstacleRainDrop;
    async UniTask SpawnRainLoop(CancellationToken token)
    {
        Transform camTr = Player.Instance.cam.transform;
        PoolBehaviour pb = obstacleRainDrop.GetComponent<PoolBehaviour>();
        while(!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(17, cancellationToken: token);
            if(isPause) continue;
            Vector3 vector = Random.Range(5f,30f) * Random.insideUnitSphere;
            vector.y = 0f;
            Vector3 pos = camTr.position + 30f * camTr.forward + vector + Random.Range(15f,40f) * Vector3.up;
            Ray ray = new Ray();
            ray.direction = Vector3.down;
            ray.origin = pos;
            RaycastHit hit;
            Physics.Raycast(ray,out hit,50f);
            if(hit.collider == null) continue;
            if(hit.distance < 10f) continue;
            if(hit.collider.gameObject.layer != 3) continue;
            await UniTask.DelayFrame(1, cancellationToken: token);
            var rain = PoolManager.Instance.Spawn(pb,pos,Quaternion.identity,currTrack.transform);
            rains.Add(rain.transform);
        }
    }
    List<Transform> rains = new List<Transform>();

    // 이 밑으로는 HDRI 관련
    [SerializeField] Renderer[] spheres;
    public List<hdrSetting> hdrSettings = new List<hdrSetting>();
    [System.Serializable]
    public struct hdrSetting
    {
        public string Name;
        public Texture2D texture;
        public Vector3 sphereScale;
        public float sphereY;
        public float offsetX;
        public bool isNature;
    }
    Renderer currSphere;
    Renderer prevSphere;
    void InitHDR()
    {
        currSphere.gameObject.SetActive(true);
        prevSphere.gameObject.SetActive(true);
    }
    void ClearHDR()
    {
        currSphere = spheres[0];
        prevSphere = spheres[1];
    }
    // 이 밑으로는 FarObject 관련
    public List<GameObject> farObjects = new List<GameObject>();







}
