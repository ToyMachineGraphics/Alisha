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
        _instance = null;
        _isQuitting = true;
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
    #endregion

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

    private enum ControllerState
    {
        Normal, MenuUI
    }
    private ControllerState _state;
    private float _uiTriggerTimer;
    public GameObject VRMenuUI;
    public Flashlight Flashlight;

    protected override void Start ()
    {
        base.Start();
        if (OnPlayerHeightChanged != null)
        {
            OnPlayerHeightChanged(Player.transform.position.y);
        }

        _laserPointer = ControllerPointer.transform.GetChild(1).GetComponent<GvrLaserPointer>();
        _laserPointer.raycastMode = GvrBasePointer.RaycastMode.Direct;
        _laserPointer.overrideCameraRayIntersectionDistance = 0f;
        _laserVisual = _laserPointer.GetComponent<GvrLaserVisual>();
        _laserVisual.shrinkLaser = false;
        _laserVisual.maxLaserDistance = 20f;

        _hand = Instantiate(_handPrefab, ControllerModel.position, ControllerModel.rotation, transform).transform;

        _state = ControllerState.Normal;
    }

	private void Update ()
    {
        if (GvrControllerInput.Recentered)
        {
            _hand.position = ControllerModel.transform.position + (ControllerModel.transform.forward + ControllerModel.transform.up) * 0.09375f;
			_defaultControllerModelForward = ControllerModel.transform.forward;
        }
		if (_state == ControllerState.Normal)
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

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
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
#if UNITY_EDITOR
		else if (Input.GetMouseButton(0))
#else
        else if (GvrControllerInput.ClickButton)
#endif
        {
            switch (_state)
            {
                case ControllerState.Normal:
                    _uiTriggerTimer += Time.deltaTime;
                    if (vrMenuUI.OnOpenFlag == global::VRMenuUI.OnOpen.None && _uiTriggerTimer > 1)
                    {
						_hand.position = MainCamera.transform.position + (MainCamera.transform.forward - Vector3.up) * 0.5f;
						_hand.rotation = Quaternion.Euler(0, -90, 0) * MainCamera.transform.rotation;
                        VRMenuUI.transform.position = vrMenuUI.LookTowardsCamera.position = _hand.position + Vector3.up * 0.75f;
						vrMenuUI.LookTowardsCamera.LookAt(MainCamera.transform.position, MainCamera.transform.up);
						VRMenuUI.transform.rotation = vrMenuUI.LookTowardsCamera.rotation;
                        vrMenuUI.Touch(GvrControllerInput.TouchPosCentered);
                        vrMenuUI.OnOpenFlag = global::VRMenuUI.OnOpen.None;
                        vrMenuUI.SelectionsRoot.gameObject.SetActive(true);
                        _state = ControllerState.MenuUI;
                    }
                    break;
                case ControllerState.MenuUI:
                    break;
            }
        }
#if UNITY_EDITOR
		else if (Input.GetMouseButtonUp(0))
#else
        else if (GvrControllerInput.ClickButtonUp)
#endif
        {
			if (vrMenuUI.OnOpenFlag == global::VRMenuUI.OnOpen.None && vrMenuUI.SelectionsRoot.gameObject.activeInHierarchy) {
				_state = ControllerState.Normal;
				_hand.position = ControllerModel.transform.position + (ControllerModel.transform.forward + ControllerModel.transform.up) * 0.25f;
				VRMenuUI.GetComponent<VRMenuUI> ().Confirm ();
			}
        }
    }
}
