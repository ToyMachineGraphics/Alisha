using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTriggerSwitch))]
public class Attractable : MonoBehaviour
{
    private EventTriggerSwitch _attractableSwitch;

    public ToggleManually FollowSwitch;
    public LookFollowTransform FollowTransform;
    public Transform FollowTarget;

    private void Awake()
    {
        _attractableSwitch = GetComponent<EventTriggerSwitch>();

        FollowTransform = GetComponent<LookFollowTransform>();
        if (FollowTransform == null)
        {
            FollowTransform = gameObject.AddComponent<LookFollowTransform>();
        }
        FollowSwitch = FollowTransform as ToggleManually;
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPointerClick(null);
        }
    }
#endif

    public void OnPointerClick(BaseEventData eventData)
    {
        FollowTransform.Reset(FollowTarget);
        FollowTransform.moveSpeed = FollowTransform.rotateSpeed = 32;
        FollowSwitch.Toggle(true);
    }
}
