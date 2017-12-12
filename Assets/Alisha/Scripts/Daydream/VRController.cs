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

    public Action<float> OnPlayerHeightChanged;

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
    }
	
	private void Update ()
    {
        if (GvrControllerInput.Recentered)
        {
            _hand.position = ControllerModel.transform.position;
        }
        _hand.rotation = ControllerModel.transform.rotation;

        if (GvrPointerInputModule.CurrentRaycastResult.isValid)
        {
            Attractable attractable = GvrPointerInputModule.CurrentRaycastResult.gameObject.GetComponent<Attractable>();
            if (attractable)
            {
                attractable.FollowTarget = _hand;
            }
        }
    }
}
