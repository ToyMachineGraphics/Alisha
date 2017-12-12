using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter2NonVR : MonoBehaviour
{
    [SerializeField]
    private Canvas _networkUI;
    [SerializeField]
    private Camera _camera;

    private void Start ()
    {
        _networkUI.worldCamera = _camera;
        _networkUI.gameObject.SetActive(true);
        _camera.gameObject.SetActive(true);
    }

    private void Update ()
    {
		
	}
}
