using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTriggerSwitch))]
public class Attractable : MonoBehaviour
{
    private EventTriggerSwitch _attractableSwitch;

    public ISwitch FollowSwitch;
    public LookFollowTransform FollowTransform;
    public Transform FollowTarget;
    private bool _arrived;

    private void Awake()
    {
        _attractableSwitch = GetComponent<EventTriggerSwitch>();

        FollowTransform = GetComponent<LookFollowTransform>();
        if (FollowTransform == null)
        {
            FollowTransform = gameObject.AddComponent<LookFollowTransform>();
        }
        FollowSwitch = FollowTransform as ISwitch;
    }

    private void Start()
    {
        VRSceneObjectManager.Instance.AttractableObjects.Add(this);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FollowTarget = MinimalDaydream.Instance.ControllerModel;
            OnPointerClick(null);
        }
    }
#endif

    private void LateUpdate()
    {
        if (_arrived)
        {
            transform.LookAt(MinimalDaydream.Instance.ControllerModel.position + MinimalDaydream.Instance.ControllerModel.forward, MinimalDaydream.Instance.ControllerModel.up);
        }
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        FollowTransform.OnArrived -= OnArrived;
        FollowTransform.OnArrived += OnArrived;
        FollowTransform.Reset(FollowTarget);
        FollowTransform.ReachThreshold = 1.25f;
        FollowTransform.MoveSpeed = 32;
        FollowTransform.RotateSpeed = 24;
        FollowSwitch.Toggle(true);
    }

    private void OnArrived(GameObject go)
    {
        _arrived = true;
        transform.SetParent(MinimalDaydream.Instance.ControllerModel, true);
        Collider[] colliders = transform.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        transform.position += MinimalDaydream.Instance.ControllerModel.forward * FollowTransform.ReachThreshold;
        transform.SetParent(null);
    }
}
