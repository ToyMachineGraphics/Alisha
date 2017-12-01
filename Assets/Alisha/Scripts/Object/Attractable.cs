using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTriggerSwitch))]
public class Attractable : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private EventTriggerSwitch _attractableSwitch;

    private ISwitch _followSwitch;
    [SerializeField]
    private LookFollowTransform _followTransform;
    public Transform FollowTarget;
    [SerializeField]
    private float _reachThreshold;
    [SerializeField]
    private float _attractSpeed;
    private bool _arrived;

    private void Awake()
    {
        _attractableSwitch = GetComponent<EventTriggerSwitch>();

        _followTransform = GetComponent<LookFollowTransform>();
        if (_followTransform == null)
        {
            _followTransform = gameObject.AddComponent<LookFollowTransform>();
        }
        _followSwitch = _followTransform as ISwitch;
    }

    private void Start()
    {
        VRSceneObjectManager.Instance.AttractableObjects.Add(this);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPointerClick(null);
        }
#endif

        if (_arrived && GvrControllerInput.ClickButtonDown)
        {
            _arrived = false;
            transform.SetParent(null);
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                c.enabled = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (_arrived)
        {
            transform.rotation = Quaternion.LookRotation(FollowTarget.forward, FollowTarget.up);
        }
    }

    private void SaveTransform()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        if (!_arrived)
        {
            SaveTransform();

            _followTransform.OnArrived -= OnArrived;
            _followTransform.OnArrived += OnArrived;
            _followTransform.Reset(FollowTarget);
            _followTransform.ReachThreshold = _reachThreshold;
            _followTransform.MoveSpeed = _attractSpeed;
            _followTransform.RotateSpeed = _attractSpeed;
            _followSwitch.Toggle(true);
        }
    }

    private void OnArrived(GameObject go)
    {
        
        //transform.SetParent(MinimalDaydream.Instance.ControllerModel, true);
        //Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        //foreach (Collider c in colliders)
        //{
        //    c.enabled = false;
        //}
        //transform.position += MinimalDaydream.Instance.ControllerModel.forward * FollowTransform.ReachThreshold;
        //transform.SetParent(null);
        transform.SetParent(FollowTarget, true);
        transform.position += (FollowTarget.forward - 2 * FollowTarget.up) * _reachThreshold;
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        _arrived = true;
    }
}
