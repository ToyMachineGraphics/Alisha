using GoogleVR.Demos;
using UnityEngine;

public class MinimalDaydream : MonoBehaviour
{
    public GameObject Player;
    public float PlayerHeight;
    public Camera MainCamera;

    #region Prefab
    public GameObject EventSystemPrefab;
    public GameObject ReticlePointerPrefab;
    public GameObject ControllerPointerPrefab;
    public GameObject ControllerMainPrefab;
    public GameObject DemoInputManagerPrefab;
    #endregion

    public GameObject EventSystem;

    public GvrReticlePointer ReticlePointer;

    public GameObject ControllerPointer;
    public GvrArmModel ArmModel;

    public GameObject ControllerMain;
    public GvrControllerInput ControllerInput;
    public Transform ControllerModel;
    public DemoInputManager InputManager;

    //#region Prefab Singleton
    //private static bool _isQuitting;
    //private static MinimalDaydream _instance;
    //public static MinimalDaydream Instance
    //{
    //    get
    //    {
    //        if (_isQuitting)
    //        {
    //            return null;
    //        }
    //        if (_instance == null)
    //        {
    //            MinimalDaydream prefab = Resources.Load<MinimalDaydream>(typeof(MinimalDaydream).Name);
    //            _instance = Instantiate(prefab);
    //        }
    //        return _instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if (_instance != null)
    //    {
    //        if (_instance.gameObject != gameObject)
    //        {
    //            Destroy(gameObject);
    //        }
    //        else if (_instance != this)
    //        {
    //            Destroy(this);
    //        }
    //    }
    //    else
    //    {
    //        _instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}

    //private void OnApplicationQuit()
    //{
    //    _isQuitting = true;
    //}
    //#endregion

    protected virtual void Start()
    {
        Player.transform.localPosition = Vector3.up * PlayerHeight;

        EventSystem = Instantiate(EventSystemPrefab, transform, false);

        ReticlePointer = Instantiate(ReticlePointerPrefab, MainCamera.transform, false).GetComponent<GvrReticlePointer>();

        ControllerPointer = Instantiate(ControllerPointerPrefab, Player.transform, false);
        ArmModel = ControllerPointer.GetComponent<GvrArmModel>();
        ControllerModel = ControllerPointer.transform.GetChild(0);

        ControllerMain = Instantiate(ControllerMainPrefab, transform, false);
        ControllerInput = ControllerMain.GetComponent<GvrControllerInput>();

        InputManager = Instantiate(DemoInputManagerPrefab, transform, false).GetComponent<DemoInputManager>();
        InputManager.controllerMain = ControllerMain;
        InputManager.controllerPointer = ControllerPointer;
        InputManager.reticlePointer = ReticlePointer.gameObject;
    }

    //private void Update()
    //{
    //    if (GvrPointerInputModule.CurrentRaycastResult.isValid)
    //    {
    //        Attractable attractable = GvrPointerInputModule.CurrentRaycastResult.gameObject.GetComponent<Attractable>();
    //        if (attractable)
    //        {
    //            attractable.FollowTarget = ControllerModel;
    //        }
    //    }
    //}
}
