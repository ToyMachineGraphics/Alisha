using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DenryuIrairaBoDestination : MonoBehaviour
{
    private bool _onUnlock;
    public UnityEvent OnUnlock;
    public int UnlockCount = 1;
    private HashSet<Transform> _arrivedAgents = new HashSet<Transform>();

    private void Start ()
    {
        Reset();
    }

    private void Reset()
    {
        _onUnlock = false;
    }

    private void Update()
    {
        if (_arrivedAgents.Count == UnlockCount && !_onUnlock)
        {
            _onUnlock = true;
            OnUnlock.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _arrivedAgents.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        _arrivedAgents.Remove(other.transform);
    }
}
