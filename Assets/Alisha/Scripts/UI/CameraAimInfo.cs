using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAimInfo : MonoBehaviour
{
    private Vector3 _forward;
    private RaycastHit _hit;
    // Use this for initialization

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        _forward = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, _forward, out _hit))
        {
            ObjInfomation _objInfo = _hit.collider.GetComponent<ObjInfomation>();
            if (_objInfo)
            {
                ObjInfoWindow.Instance.ShowWindow(_objInfo.transform.position, transform, _objInfo.Info);
            }
        }
    }
}