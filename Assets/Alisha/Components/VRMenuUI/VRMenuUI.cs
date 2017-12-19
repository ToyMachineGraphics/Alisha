using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRMenuUI : MonoBehaviour
{
    public Transform[] Sections;
    private int _currentSelectedIndex;
    private int _lastSelectedIndex;
    [SerializeField]
    private Transform _currentSelected;
    [SerializeField]
    private Material _defaultMaterial;
    public Material SelectedMaterial;
    public UnityEvent[] SelectActions;

    private float _timer = 0;
    private Func<int, bool> GetPressed;

    public Transform LookTowardsCamera;

    private void Start ()
    {
        _currentSelectedIndex = _lastSelectedIndex = 0;
        GetPressed = Input.GetMouseButton;
        LookTowardsCamera = new GameObject("LookTowardsCamera").transform;
        LookTowardsCamera.position = transform.position;
        gameObject.SetActive(false);
    }

    private void Update ()
    {
		if (GetPressed(0))
        {
            float deltaX = Input.mousePosition.x - Screen.width / 2;
            float deltaY = Input.mousePosition.y - Screen.height / 2;
            Touch(new Vector2(deltaX, deltaY));
        }

        LookTowardsCamera.position = VRController.Instance.Hand.transform.position + Vector3.up * 0.25f;
        LookTowardsCamera.LookAt(VRController.Instance.MainCamera.transform.position, VRController.Instance.MainCamera.transform.up);
        transform.position = Vector3.Lerp(transform.position, LookTowardsCamera.position, Time.deltaTime * 8f);
        transform.rotation = LookTowardsCamera.rotation;
    }

    public void Touch(Vector2 position)
    {
        bool changed = false;
        float rad = Mathf.Atan2(position.y, position.x);
        float deg = Mathf.Rad2Deg * rad;
        if (deg < 0)
        {
            deg += 360;
        }
        Debug.Log(deg);
        float anglePerSection = 360.0f / Sections.Length;
        for (int i = 0; i < Sections.Length; i++)
        {
            if (deg >= anglePerSection * i && deg < anglePerSection * (i + 1))
            {
                if (_currentSelectedIndex != i)
                {
                    _lastSelectedIndex = _currentSelectedIndex;
                    _currentSelectedIndex = i;
                    _currentSelected = Sections[_currentSelectedIndex];
                    changed = true;
                }
                break;
            }
        }
        if (changed)
        {
            for (int i = 0; i < Sections.Length; i++)
            {
                Sections[i].GetComponent<Renderer>().material = _defaultMaterial;
            }
            _currentSelected.GetComponent<Renderer>().material = SelectedMaterial;
        }
    }

    public void Confirm()
    {
        if (SelectActions[_currentSelectedIndex] != null)
        {
            SelectActions[_currentSelectedIndex].Invoke();
        }
    }
}
