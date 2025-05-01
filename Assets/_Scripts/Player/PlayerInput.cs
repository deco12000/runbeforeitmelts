using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public enum PlatformType { None, Desktop_WebGL, Mobile_WebGL, Windows, Mac, Android, Ios }
public enum ControlType { Touch, }
public enum LanguageType { English, Korean, Russian, Chinese, Japaness, Spanish, Arabic, Hindi }

public class PlayerInput : MonoBehaviour
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
    
    public PlatformType currPlatformType = PlatformType.Android;
    public ControlType currControlType = ControlType.Touch;
    public LanguageType currLanguageType = LanguageType.Korean;

    void Awake()
    {
        Player.Instance.pinput = this;
    }

    void Start()
    {
#if UNITY_EDITOR
        currPlatformType = PlatformType.Windows;
#elif UNITY_ANDROID
        currPlatformType = PlatformType.Android;
#elif UNITY_IOS
        currPlatformType = PlatformType.Ios;
#endif
        CheckCursor(cts.Token).Forget();
        CheckInput(cts.Token).Forget();
    }

    PointerEventData eventData = new PointerEventData(EventSystem.current);
    private List<RaycastHit> cursorObjectsWorld = new List<RaycastHit>();
    public List<RaycastHit> CursorObjectsWorld => cursorObjectsWorld;
    private List<RaycastResult> cursorObjectsUI = new List<RaycastResult>();
    public List<RaycastResult> CursorObjectsUI => cursorObjectsUI;
    [SerializeField] List<GameObject> viewOnly = new List<GameObject>();
    private Camera cam;
    private Ray cursorRay;
    public Ray CursorRay => cursorRay;
    private RaycastHit[] _hitsW = new RaycastHit[50];
    async UniTask CheckCursor(CancellationToken token)
    {
        eventData = new PointerEventData(EventSystem.current);
        while (!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);
            if (cam == null || !cam.gameObject.activeInHierarchy || !cam.enabled)
            {
                cam = Camera.main;
                await UniTask.DelayFrame(50, cancellationToken: token);
                continue;
            }
            eventData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(eventData, cursorObjectsUI);
            cursorRay = cam.ScreenPointToRay(Input.mousePosition);
            Physics.RaycastNonAlloc(cursorRay, _hitsW, 2000f);
            cursorObjectsWorld = _hitsW.ToList().Where(x => x.collider != null).OrderBy(x => x.distance).ToList();
#if UNITY_EDITOR
            viewOnly.Clear();
            cursorObjectsUI.ForEach(x => viewOnly.Add(x.gameObject));
            cursorObjectsWorld.ForEach(x => viewOnly.Add(x.collider.gameObject));
#endif
        }
    }


    public Vector2 direction;
    async UniTask CheckInput(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);


        }
    }





}
