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

    public bool isTestPlay;

    public PlatformType currPlatformType = PlatformType.Android;
    public ControlType currControlType = ControlType.Touch;
    public LanguageType currLanguageType = LanguageType.Korean;

    void Awake()
    {
        Player.Instance.input = this;
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
            _hitsW = Physics.RaycastAll(cursorRay, 2000f);
            cursorObjectsWorld = _hitsW.ToList().OrderBy(x => x.distance).ToList();
#if UNITY_EDITOR
            viewOnly.Clear();
            for (int i = 0; i < cursorObjectsUI.Count; i++)
                viewOnly.Add(cursorObjectsUI[i].gameObject);
            for (int i = 0; i < cursorObjectsWorld.Count; i++)
                viewOnly.Add(cursorObjectsWorld[i].collider.gameObject);
#endif
        }
    }




    ////////////////////
    bool _mouse0;
    public bool isMouse0;
    public UnityAction<GameObject> OnMouseDown0 = (target) => { };
    public UnityAction OnMouseUp0 = () => { };
    ////////////////////
    public Vector2 moveDirection;
    ////////////////////
    public bool isJump;
    public UnityAction OnJumpDown = () => { };
    public UnityAction<Vector2, float> OnJumpUp = (dir, time) => { };
    public UnityAction<Vector2, float> OnJumpAutoRelease = (dir, time) => { };
    [HideInInspector] public Vector2 jumpDirection;
    bool _jump;
    float _startTime;
    Vector2 _jumpDirection;
    float horz;
    float vert;
    ////////////////////
    public bool isSprint;
    ////////////////////
    public UnityAction OnSkillBtnDown = () => { };

    async UniTask CheckInput(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.DelayFrame(1, cancellationToken: token);
            if (Input.GetMouseButton(0))
            {
                if (!_mouse0)
                {
                    _mouse0 = true;
                    await UniTask.DelayFrame(1, cancellationToken: token);
                    GameObject go;
                    if (cursorObjectsUI.Count != 0)
                    {
                        go = cursorObjectsUI[0].gameObject;
                    }
                    else if (cursorObjectsWorld.Count != 0)
                    {
                        go = cursorObjectsWorld[0].collider.gameObject;
                    }
                    else
                    {
                        go = null;
                    }
                    OnMouseDown0.Invoke(go);
                }
            }
            else
            {
                if (_mouse0)
                {
                    _mouse0 = false;
                    OnMouseUp0.Invoke();
                }
            }
#if UNITY_EDITOR
            if(isTestPlay)
            {
                horz = Input.GetAxisRaw("Horizontal");
                vert = Input.GetAxisRaw("Vertical");
                moveDirection = new Vector2(horz, vert);
            }
#endif
        }
    }






}
