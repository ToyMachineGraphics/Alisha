using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EventTriggerSwitch : EventTrigger
{
    public virtual string GroupTag
    {
        get;
        private set;
    }

    private bool _interactive = true;
    public bool Interactable
    {
        get
        {
            return _interactive;
        }
        set
        {
            foreach (Entry entry in triggers)
            {
                int callbackCount = entry.callback.GetPersistentEventCount();
                for (int i = 0; i < callbackCount; i++)
                {
                    entry.callback.SetPersistentListenerState(0, value ? UnityEventCallState.RuntimeOnly : UnityEventCallState.Off);
                }
            }
        }
    }
}
