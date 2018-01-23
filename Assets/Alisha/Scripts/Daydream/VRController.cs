using System;
using UnityEngine;

public sealed class VRController : MinimalDaydream
{
    #region Prefab Singleton

    private static bool _isQuitting;
    private static VRController _instance;

    public static VRController Instance
    {
        get
        {
            if (_isQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                VRController prefab = Resources.Load<VRController>(typeof(VRController).Name);
                _instance = Instantiate(prefab);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            if (_instance.gameObject != gameObject)
            {
                Destroy(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _isQuitting = true;
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    #endregion Prefab Singleton

    private GvrLaserPointer _laserPointer;
    private GvrLaserVisual _laserVisual;

    [SerializeField]
    private GameObject _handPrefab;

    [SerializeField]
    private Vector3 _handScale;

    [SerializeField]
    private Transform _hand;

    public Transform Hand
    {
        get { return _hand; }
    }

    private Vector3 _defaultControllerModelForward = Vector3.forward;

    public Action<float> OnPlayerHeightChanged;

    public enum ControllerState
    {
        Normal, MenuUI, WaitForTouchPadUp
    }

    private ControllerState _state;

    public ControllerState State
    {
        set { _state = value; }
    }

    private float _uiTriggerTimer;
    public GameObject VRMenuUI;
    public Flashlight Flashlight;

    protected override void Start()
    {
        base.Start();
        if (OnPlayerHeightChanged != null)
        {
            OnPlayerHeightChanged(Player.transform.position.y);
        }

        //_laserPointer = ControllerPointer.transform.GetChild(1).GetComponent<GvrLaserPointer>();
        //_laserPointer.raycastMode = GvrBasePointer.RaycastMode.Direct;
        //_laserPointer.overrideCameraRayIntersectionDistance = 0f;
        //_laserVisual = _laserPointer.GetComponent<GvrLaserVisual>();
        //_laserVisual.shrinkLaser = false;
        //_laserVisual.maxLaserDistance = 20f;

        _hand = Instantiate(_handPrefab, ControllerModel.position, ControllerModel.rotation, transform).transform;

        _state = ControllerState.Normal;
    }

    private RaycastHit[] _raycastHitBuffer = new RaycastHit[4];
    private ObjInfomation _objInfoSelected;

    private void Update()
    {
        if (GvrControllerInput.Recentered)
        {
            _hand.position = ControllerModel.transform.position + (ControllerModel.transform.forward + ControllerModel.transform.up) * 0.09375f;
            _defaultControllerModelForward = ControllerModel.transform.forward;
        }
        if (_state == ControllerState.Normal || _state == ControllerState.WaitForTouchPadUp)
        {
            _hand.rotation = ControllerModel.transform.rotation;
        }

        if (GvrPointerInputModule.CurrentRaycastResult.isValid)
        {
            Attractable attractable = GvrPointerInputModule.CurrentRaycastResult.gameObject.GetComponent<Attractable>();
            if (attractable)
            {
                attractable.FollowTarget = _hand;
            }
        }

        if (!VRMenuUI)
        {
            return;
        }

        VRMenuUI vrMenuUI = VRMenuUI.GetComponent<VRMenuUI>();
        vrMenuUI.BackpackHierachy2.worldCamera = MainCamera;
        vrMenuUI.Hiarachy1Root.GetComponent<Canvas>().worldCamera = MainCamera;
        vrMenuUI.Hierachy2BackpackRoot.GetComponent<Canvas>().worldCamera = MainCamera;
        vrMenuUI.Hierarchy3BackpackRoot.GetComponent<Canvas>().worldCamera = MainCamera;
        vrMenuUI.Hierarchy2RadioRoot.GetComponent<Canvas>().worldCamera = MainCamera;

        if ((false && !vrMenuUI.SelectionsRoot.gameObject.activeInHierarchy &&
            !vrMenuUI.BackpackRoot.gameObject.activeInHierarchy &&
            !vrMenuUI.BackpackHierachy2.gameObject.activeInHierarchy) ||

            !vrMenuUI.Hiarachy1Root.gameObject.activeInHierarchy &&
            !vrMenuUI.Hierachy2BackpackRoot.gameObject.activeInHierarchy &&
            !vrMenuUI.BackpackHierachy2.gameObject.activeInHierarchy &&
            !vrMenuUI.Hierarchy2RadioRoot.gameObject.activeInHierarchy)
        {
            Ray ray = MainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            int count = Physics.RaycastNonAlloc(ray, _raycastHitBuffer, 100, LayerMask.GetMask("Default"));
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
#endif
            if (count > 0)
            {
                bool casted = false;
                for (int i = 0; i < count; i++)
                {
                    //Debug.Log(_raycastHitBuffer[i].transform.name);
                    ObjInfomation objInfo = _raycastHitBuffer[i].transform.GetComponent<ObjInfomation>();
                    if (objInfo)
                    {
                        _objInfoSelected = objInfo;
                        ObjInfoWindow.Instance.Hit = objInfo.Hit = true;
                        ObjInfoWindow.Instance.ShowWindow(objInfo.HintParent.transform.position, MainCamera.transform, objInfo.Info);
                        casted = true;
                        break;
                    }
                }
                if (!casted)
                {
                    if (_objInfoSelected)
                    {
                        _objInfoSelected.Hit = false;
                    }
                    ObjInfoWindow.Instance.Hit = false;
                }
                //Debug.Log(hit.transform.name);
                //ObjInfomation objInfo = hit.transform.GetComponent<ObjInfomation>();
                //if (objInfo)
                //{
                //    _objInfoSelected = objInfo;
                //    ObjInfoWindow.Instance.Hit = objInfo.Hit = true;
                //    ObjInfoWindow.Instance.ShowWindow(objInfo.HintParent.transform.position, MainCamera.transform, objInfo.Info);
                //}
                //else
                //{
                //    if (_objInfoSelected)
                //    {
                //        _objInfoSelected.Hit = false;
                //    }
                //    ObjInfoWindow.Instance.Hit = false;
                //}
            }
            else
            {
                ObjInfoWindow.Instance.Hit = false;
                if (_objInfoSelected)
                {
                    _objInfoSelected.Hit = false;
                }
            }
        }

#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
        if (Input.GetMouseButtonDown(0))
#elif UNITY_ANDROID
        if (GvrControllerInput.ClickButtonDown)
#endif
        {
            switch (_state)
            {
                case ControllerState.Normal:
                    _uiTriggerTimer = 0;
                    break;

                case ControllerState.MenuUI:
                    break;
            }
        }
#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
        else if (Input.GetMouseButton(0))
#elif UNITY_ANDROID
        else if (GvrControllerInput.ClickButton)
#endif
        {
            switch (_state)
            {
                case ControllerState.Normal:
                    _uiTriggerTimer += Time.deltaTime;
                    if (vrMenuUI.OnOpenFlag == global::VRMenuUI.OnOpen.None && _uiTriggerTimer > 1)
                    {
                        Vector3 reference = MainCamera.transform.position + (MainCamera.transform.forward - Vector3.up) * 0.5f;
                        _hand.position = reference;
                        _hand.rotation = Quaternion.Euler(0, -90, 0) * MainCamera.transform.rotation;
                        VRMenuUI.transform.position = vrMenuUI.LookTowardsCamera.position = MainCamera.transform.position + MainCamera.transform.forward * 0.5f;
                        vrMenuUI.LookTowardsCamera.LookAt(MainCamera.transform.position, Vector3.up);
                        VRMenuUI.transform.rotation = vrMenuUI.LookTowardsCamera.rotation;
                        vrMenuUI.Touch(GvrControllerInput.TouchPosCentered);
                        vrMenuUI.OnOpenFlag = global::VRMenuUI.OnOpen.Hierarchy1Root;
                        vrMenuUI.SelectionsRoot.gameObject.SetActive(true);
                        vrMenuUI.Hiarachy1Root.gameObject.SetActive(true);
                        vrMenuUI.OnVRMenuUIEnable = true;
                        _state = ControllerState.MenuUI;
                    }
                    break;
            }
        }
#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
        else if (Input.GetMouseButtonUp(0))
#elif UNITY_ANDROID
        else if (GvrControllerInput.ClickButtonUp)
#endif
        {
            if ((vrMenuUI.OnOpenFlag == global::VRMenuUI.OnOpen.Hierarchy1Root && vrMenuUI.Hiarachy1Root.gameObject.activeInHierarchy) || (false && vrMenuUI.SelectionsRoot.gameObject.activeInHierarchy))
            {
                _state = ControllerState.Normal;
                ResetHand();

                VRMenuUI.GetComponent<VRMenuUI>().Confirm();
            }
            if (_state == ControllerState.WaitForTouchPadUp)
            {
                Debug.Log("WaitForTouchPadUp to Normal");
                _state = ControllerState.Normal;
            }
        }
    }

    public void ResetHand()
    {
        _hand.position = ControllerModel.transform.position + (ControllerModel.transform.forward + ControllerModel.transform.up) * 0.09375f;
        _hand.rotation = ControllerModel.transform.rotation;
    }
}