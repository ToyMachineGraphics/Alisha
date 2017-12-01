using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter2VR : MonoBehaviour
{
    [SerializeField]
    private VRController _controller;
    [SerializeField]
    private DenryuIrairaBo _denryuIrairaBo;

    private void Awake()
    {
        _controller = VRController.Instance;
    }

    private void Start ()
    {
        _denryuIrairaBo.RaycastAction = AttractDenryuIrairaBoAgent;
    }

    private void Update ()
    {
		
	}

    private void AttractDenryuIrairaBoAgent()
    {
        if (GvrPointerInputModule.CurrentRaycastResult.isValid)
        {
            _denryuIrairaBo.Target.position = GvrPointerInputModule.CurrentRaycastResult.worldPosition;
        }
    }
}
