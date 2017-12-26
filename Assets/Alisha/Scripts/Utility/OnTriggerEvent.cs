using UnityEngine;
using UnityEngine.Events;
using System;

public class OnTriggerEvent : MonoBehaviour
{
	public UnityColliderEvent onTriggerEnter;
	public UnityColliderEvent onTriggerStay;
	public UnityColliderEvent onTriggerExit;

	private void OnTriggerEnter(Collider collider)
	{
		if (onTriggerEnter != null) {
			onTriggerEnter.Invoke (collider);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (onTriggerStay != null) {
			onTriggerStay.Invoke(other);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (onTriggerExit != null) {
			onTriggerExit.Invoke(collider);
		}
	}
}
	
[Serializable]
public class UnityColliderEvent : UnityEvent<Collider>
{
}