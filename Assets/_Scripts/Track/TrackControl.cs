using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
public class TrackControl : MonoBehaviour
{
    [SerializeField] List<Track> tracks;
    [SerializeField] List<Track> currTracks;
    Transform pivot;
    CamPivot camPivot;
    Transform player;
    [ReadOnlyInspector][SerializeField] Track currTrack;
    [ReadOnlyInspector][SerializeField] Track prevTrack;
    int maxTrackCount = 6;
    Vector3[,] checkPoints;
    Vector3[,] checkPointsEuler;
    bool[] isSpawnTrack;
    [SerializeField] Vector2Int currCheckPoint;
    [SerializeField] WaterSpawner waterSpawner;
    [SerializeField] SphericalBGControl sphericalBGControl;
    void Start()
    {
        pivot = transform.Find("Pivot");
        camPivot = transform.Find("CamPivot").GetComponent<CamPivot>();
        player = Player.I.transform;
        PoolBehaviour pb;
        pb = PoolManager.I.Spawn(tracks[0], Vector3.zero, Quaternion.identity, transform);
        pb.transform.position = 999f * Vector3.one;
        currTracks.Add(pb as Track);
        pb = PoolManager.I.Spawn(tracks[0], Vector3.zero, Quaternion.identity, transform);
        pb.transform.position = 999f * Vector3.one;
        currTracks.Add(pb as Track);
        pb = PoolManager.I.Spawn(tracks[0], Vector3.zero, Quaternion.identity, transform);
        pb.transform.position = 999f * Vector3.one;
        currTracks.Add(pb as Track);
        pb = PoolManager.I.Spawn(tracks[0], Vector3.zero, Quaternion.identity, transform);
        pb.transform.position = 999f * Vector3.one;
        currTracks.Add(pb as Track);
        pb = PoolManager.I.Spawn(tracks[0], Vector3.zero, Quaternion.identity, transform);
        currTracks.Add(pb as Track);
        pb.transform.position = 999f * Vector3.one;
        pb = PoolManager.I.Spawn(tracks[1], Vector3.zero, Quaternion.identity, transform);
        currTracks.Add(pb as Track);
        currTrack = pb as Track;
        waterSpawner.isInside = currTrack.isInside;
        GetAllCheckPoints();
        StartCoroutine(nameof(GetCurrentCheckPointLoop));
        pb.transform.position = new Vector3(0f, 20f, 0f);
    }
    void GetAllCheckPoints()
    {
        int count = currTrack.checkPoint.Length;
        checkPoints = new Vector3[count, 180];
        checkPointsEuler = new Vector3[count, 180];
        isSpawnTrack = new bool[count];
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < 180; j++)
            {
                float t = ((float)j) / (180-1);
                currTrack.checkPoint[i].clip.SampleAnimation(pivot.gameObject, t);
                checkPoints[i, j] = pivot.localPosition;
                checkPointsEuler[i, j] = pivot.localRotation.eulerAngles;
            }
        }
    }
    Vector2Int GetCurrentCheckPoint()
    {
        int count = currTrack.checkPoint.Length;
        float minDist = 999999f;
        Vector2Int find = new Vector2Int(-999, -999);
        for (int i = 0; i < count; i++)
            for (int j = 0; j < 180; j++)
            {
                Vector3 vector = currTrack.transform.position
                + checkPoints[i, j].x * currTrack.transform.right + checkPoints[i, j].y * currTrack.transform.up + checkPoints[i, j].z * currTrack.transform.forward;
                float dist = (player.position - vector).sqrMagnitude;
#if UNITY_EDITOR
                //Debug.DrawLine(player.position, vector, Color.gray, 0.2f);
#endif
                if (dist < minDist)
                {
                    minDist = dist;
                    find = new Vector2Int(i, j);
                }
                if (dist < 4f)
                {
                    return find;
                }
            }
        return find;
    }
    IEnumerator GetCurrentCheckPointLoop()
    {
        yield return YieldInstructionCache.WaitForSeconds(2f);
        while (true)
        {
            currCheckPoint = GetCurrentCheckPoint();
            if (currCheckPoint.y < 999)
            {
                // pivot.transform.position = currTrack.transform.position + currTrack.transform.rotation * checkPoints[currCheckPoint.x, currCheckPoint.y];
                // pivot.transform.rotation = currTrack.transform.rotation * Quaternion.Euler(checkPointsEuler[currCheckPoint.x, currCheckPoint.y]);
                camPivot.targetQuat = currTrack.transform.rotation * Quaternion.Euler(checkPointsEuler[currCheckPoint.x, currCheckPoint.y]);
            }
            yield return YieldInstructionCache.WaitForSeconds(0.8f);
            if (currCheckPoint.y <= 50 && currCheckPoint.y >= 40)
            {
                if (!isSpawnTrack[currCheckPoint.x])
                {
                    string _prevTrackName;
                    if (prevTrack == null) _prevTrackName = "";
                    else _prevTrackName = prevTrack.name;
                    float height = currTrack.transform.position.y + checkPoints[currCheckPoint.x,0].y;
                    Debug.Log(height);
                    if (prevTrack != null)
                    {
                        prevTrack.DeSpawn();
                        //currTracks.Remove(prevTrack);
                    }
                    prevTrack = currTrack;
                    int despawnIndex = -1;
                    isSpawnTrack[currCheckPoint.x] = true;
                    for (int i = 0; i < currTracks.Count; i++)
                    {
                        if (currTracks[i] == currTrack) continue;
                        despawnIndex = i;
                        Debug.Log("오래된 트랙 디스폰 & 새로운 트랙 스폰");
                        break;
                    }
                    currTracks[despawnIndex].DeSpawn();
                    currTracks.RemoveAt(despawnIndex);
                    // 새로운 트랙 스폰될때 조건. 
                    // 직전 2개의 트랙과 다른 종류의 트랙이 나와야함.
                    // 엔드포인트 높이와 같은 스타트 높이를 가진 트랙이 나와야함.
                    Track track = tracks[2];
                    for (int j = 0; j < 500; j++)
                    {
                        track = tracks[Random.Range(2, tracks.Count)];
                        if (track.name == currTrack.name)
                        {
                            //Debug.Log("a");
                            continue;
                        }
                        if (track.name == _prevTrackName)
                        {
                            //Debug.Log("b");
                            continue;
                        }
                        if (Mathf.Abs(track.startFloor - height) > 0.5f)
                        {
                            //Debug.Log("c");
                            continue;
                        }
                        if (track.name != currTrack.name && track.name != _prevTrackName && Mathf.Abs(track.startFloor - height) < 0.5f)
                        {
                            break;
                        }
                    }
                    //Debug.Log($"{track.name}vs{_prevTrackName},{currTrack.name} , {track.startFloor}vs{height} ");
                    PoolBehaviour pb = PoolManager.I.Spawn(track, Vector3.zero, Quaternion.identity, transform);
                    currTracks.Add(pb as Track);
                    pb.transform.position = currTrack.endPoint[currCheckPoint.x].position;
                    pb.transform.rotation = currTrack.endPoint[currCheckPoint.x].rotation;
                }
            }
            if (currCheckPoint.y <= 8)
            {
                float minDistance = 99999f;
                int find = -1;
                for (int i = 0; i < currTracks.Count; i++)
                {
                    if (currTracks[i] == currTrack) continue;
                    float dist = (player.position - currTracks[i].transform.position).sqrMagnitude;
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        find = i;
                    }
                }
                Vector3 currLast = checkPoints[currCheckPoint.x, 1];
                if (minDistance < (player.position - currLast).sqrMagnitude)
                {
                    currTrack = currTracks[find];
                    GetAllCheckPoints();
                    Debug.Log("새 트랙을 현재 트랙으로 갱신");
                }
                waterSpawner.isInside = currTrack.isInside;
                sphericalBGControl.ChangeBG(currTrack.groundType);
                if (currTrack.isInside) SoundManager.I.ausAmbience.DOFade(0.56f, 2f);
                else SoundManager.I.ausAmbience.DOFade(1f, 2f);
            }
        }
    }








}
