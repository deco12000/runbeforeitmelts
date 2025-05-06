using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StageControl : MonoBehaviour
{
    void Start()
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
        CheckCurrTrackInfo();
        //
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
            track.transform.position += 2.7f * Time.deltaTime * dir.normalized;
        }
        if (selectPath.y >= 30 && !_bool1)
        {
            _bool1 = true;
            SpawnTrack();
        }
        if (selectPath.y >= 50)
        {
            float distance1 = (camTr.position - (currTrack.transform.position + camPivotLPoss[selectPath.x, 55])).sqrMagnitude;
            float distance2 = (camTr.position - activeTracks[activeTracks.Count - 1].transform.position).sqrMagnitude;
            //Debug.DrawLine(camTr.position, (currTrack.transform.position + camPivotLPoss[selectPath.x, 55]), Color.red);
            //Debug.DrawLine(camTr.position, activeTracks[activeTracks.Count - 1].transform.position, Color.blue);
            if (distance1 > distance2)
            {
                selectPath = Vector2Int.zero;
                currTrack = activeTracks[activeTracks.Count - 1];
                _bool1 = false;
                CheckCurrTrackInfo();
            }
        }
    }
    bool _bool1 = false;
    bool _bool2 = false;
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


    // 이 밑으로는 Obstacle 관련
    public List<IObstacle> obstacles = new List<IObstacle>();





    // 이 밑으로는 Item 관련
    public List<IObstacle> items = new List<IObstacle>();




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
