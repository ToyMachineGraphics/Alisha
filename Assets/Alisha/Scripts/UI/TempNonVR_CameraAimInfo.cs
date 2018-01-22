using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempNonVR_CameraAimInfo : MonoBehaviour
{
    private Vector3 _forward;
    private RaycastHit _hit;
    private ObjInfomation _objInfoSelect;

    public ObjInfomation ObjInfoSelect
    {
        get { return _objInfoSelect; }
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        _forward = transform.TransformDirection(Vector3.forward);

        bool hit = Physics.Raycast(transform.position, _forward, out _hit);
        TempNonVR_ObjInfoWindow.Instance.Hit = hit;
        if (hit)
        {
            ObjInfomation objInfo = _hit.collider.GetComponent<ObjInfomation>();

            _objInfoSelect = objInfo;
            TempNonVR_ObjInfoWindow.Instance.Hit = (objInfo != null);
            if (objInfo)
            {
                TempNonVR_ObjInfoWindow.Instance.ShowWindow(objInfo.transform.position, transform, objInfo.Info);
                //ObjInfoWindow.Instance.TweenAnim.DOPlayForward();
            }
            else
                TempNonVR_ObjInfoWindow.Instance._opened = false;
        }
    }
}