using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class MethodCollection
{
    // 피셔-예이츠 리스트 셔플. 만든사람 : 피셔-예이츠
    // 사용법 : 리스트 뒤에 .Shuffle()
    private static System.Random random = new System.Random();
    public static void Shuffle<T>(this IList<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int k = random.Next(i + 1);
            T value = values[k];
            values[k] = values[i];
            values[i] = value;
        }
    }


    // 길이 length의 랜덤 문자열 생성. 만든사람 : 김장훈
    // 사용법 : string str = RandomString(8);
    private const string VALID_CHARS = "0123456789abcdefghijklmnopqrstuvwxyz";
    public static string RandomString(int length)
    {
        var sb = new System.Text.StringBuilder(length);
        var r = new System.Random();
        for (int i = 0; i < length; i++)
        {
            int pos = r.Next(VALID_CHARS.Length);
            char c = VALID_CHARS[pos];
            sb.Append(c);
        }
        return sb.ToString();
    }


    // 이름 맨 앞에 --- 가 붙은것을 제외하고, 씬에서 실질적인 최상위 루트를 찾아주는 메소드.  만든사람 : 김장훈
    // 사용법 : 콜라이더나 트랜스폼 뒤에 .Root()
    public static Transform Root(this Collider x)
    {
        Transform result = x.transform.root;
        if (result == null) result = x.transform;
        if (result.name.Substring(0, 3) != "---") return result;
        List<Transform> trs = x.transform.GetComponentsInParent<Transform>().ToList();
        foreach (var tr in trs)
        {
            if (tr.parent == result)
            {
                result = tr;
                break;
            }
        }
        return result;
    }
    public static Transform Root(this Transform x)
    {
        Transform result = x.transform.root;
        if (result == null) result = x.transform;
        if (result.name.Substring(0, 3) != "---") return result;
        List<Transform> trs = x.transform.GetComponentsInParent<Transform>().ToList();
        foreach (var tr in trs)
        {
            if (tr.parent == result)
            {
                result = tr;
                break;
            }
        }
        return result;
    }


    // 공간상에서 두 Ray 가 만나는 교점을 찾는 메소드. 만든사람 : 김장훈
    // 리턴 값이 (-999, -999, -999) 라는건 두 Ray가 공간상에서 서로 안겹친다는 의미
    public static Vector3 RayRayIntersection(Vector3 ray1_origin, Vector3 ray1_dir, Vector3 ray2_origin, Vector3 ray2_dir, bool oneside = true)
    {
        Vector3 result = new Vector3(-999, -999, -999);
        ray1_dir.Normalize();
        ray2_dir.Normalize();
        Vector3 w0 = ray1_origin - ray2_origin;
        float a = Vector3.Dot(ray1_dir, ray1_dir); // 1
        float b = Vector3.Dot(ray1_dir, ray2_dir);
        float c = Vector3.Dot(ray2_dir, ray2_dir); // 1
        float d = Vector3.Dot(ray1_dir, w0);
        float e = Vector3.Dot(ray2_dir, w0);
        float denominator = a * c - b * b;
        // 두 직선이 거의 평행한 경우
        if (Mathf.Abs(denominator) < 1e-6f)
        {
            if ((Vector3.Cross(ray1_dir, ray2_dir).magnitude < 1e-6f) 
            && (Vector3.Cross(ray1_origin - ray2_origin, ray1_dir).magnitude < 1e-6f))
            {
                return ray1_origin; // 두 직선이 완전히 포개지는경우 -> 무한히 많은 교점이 생기지만 origin 하나만 리턴시켰음
            }
            return result; // 두직선이 평행하면서 교차점 없음
        }
        float s = (b * e - c * d) / denominator;
        float t = (a * e - b * d) / denominator;
        // 단방향일 경우: 레이의 앞 방향만 유효하게 (레이의 뒤쪽으로 생기는 직선은 교차점으로 취급 안하는 설정)
        if (oneside && (s < 0 || t < 0))
            return result;
        Vector3 point1 = ray1_origin + s * ray1_dir;
        Vector3 point2 = ray2_origin + t * ray2_dir;
        if (Vector3.Distance(point1, point2) > 1e-3f)
            return result;
        return (point1 + point2) / 2f;
    }





    /*


    // 게임 여러곳에서 자주 사용하는 유용한 스태틱메소드 생각나면 아래에 더 추가
    // 용도와 사용법 만든사람 같이 적어주면 좋음



    */











}


    









