using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Flashlight : NetworkBehaviour
{
    public GameObject Model;
    [SerializeField]
    private Light _innerSpotlight;
    [SerializeField]
    private float _lightLength = 1;
    public SphereCollider InnerCollider;
    [SerializeField]
    private Light _outerSpotlight;
    public SphereCollider OuterCollider;

    public NetworkIdentity NetId;

    private void Update()
    {
        float innerRadius = _lightLength * Mathf.Tan(_innerSpotlight.spotAngle * Mathf.Deg2Rad / 2.0f);
        InnerCollider.radius = innerRadius;
        InnerCollider.transform.position = _innerSpotlight.transform.position + _innerSpotlight.transform.forward * _lightLength;

        float outerRadius = _lightLength * Mathf.Tan(_outerSpotlight.spotAngle * Mathf.Deg2Rad / 2.0f);
        OuterCollider.radius = outerRadius;
        OuterCollider.transform.position = _outerSpotlight.transform.position + _outerSpotlight.transform.forward * _lightLength;
    }
}
