using UnityEngine;

// template : 틀, 형 사용법<T>
// 싱글톤 : 관리자, 전역 , 하나(유일)
// BS : 런타임시에만 존재  -> SS
public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if(_instance == null)
                {
                   GameObject o = new GameObject(typeof(T).Name);
                   _instance = o.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    protected abstract bool IsDontDestroy();

    protected virtual void Awake()
    {
        if(IsDontDestroy())
        DontDestroyOnLoad(this.gameObject);

        // if(Instance != null && Instance != this)
        // {
        //     Destroy(Instance.gameObject);
        // }


    }



    
}
