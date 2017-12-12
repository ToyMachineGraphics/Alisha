using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter2VR : MonoBehaviour
{
    [SerializeField]
    private VRController _controller;
    [SerializeField]
    private DenryuIrairaBo _denryuIrairaBo;

    [SerializeField]
    private Canvas _networkUI;

    private void Awake()
    {
        _controller = VRController.Instance;
        _controller.OnPlayerHeightChanged = OnPlayerHeightChanged;
    }

    private void Start ()
    {
        _denryuIrairaBo.RaycastAction = AttractDenryuIrairaBoAgent;
    }

    private void Update ()
    {
		
	}

    public void OnPlayerHeightChanged(float height)
    {
        _networkUI.transform.position = new Vector3(_networkUI.transform.position.x, height, _networkUI.transform.position.z);
        _networkUI.worldCamera = _controller.MainCamera;
        _networkUI.gameObject.SetActive(true);
    }

    private void AttractDenryuIrairaBoAgent()
    {
        if (GvrPointerInputModule.CurrentRaycastResult.isValid)
        {
            _denryuIrairaBo.Target.position = GvrPointerInputModule.CurrentRaycastResult.worldPosition;
        }
    }
}
