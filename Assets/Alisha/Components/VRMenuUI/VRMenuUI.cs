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

    private Func<int, bool> GetPressed;
    private Func<Vector2> TouchPosition;

    public Transform LookTowardsCamera;
	[SerializeField]
	private float _checkAngleDiffDelay = 0.5f;
	private float _checkAngleDiffDelayTimer;

    public bool OnVRMenuUIEnable;
    public bool OnFlashlightSelected;

    private void OnEnable()
	{
		_currentSelectedIndex = _lastSelectedIndex = 0;
		for (int i = 0; i < Sections.Length; i++)
		{
			Sections[i].GetComponent<Renderer>().material = _defaultMaterial;
		}
		_checkAngleDiffDelayTimer = 0;
        OnVRMenuUIEnable = true;

        Debug.Log ("VRMenuUI OnEnable");
	}

    private void Start ()
    {
#if UNITY_EDITOR
        GetPressed = Input.GetMouseButton;
        TouchPosition = MousePositionCentered;
#elif UNITY_ANDROID
        GetPressed = ClickButton;
        TouchPosition = GetTouchPosition;
#endif
        LookTowardsCamera = new GameObject("LookTowardsCamera").transform;
        LookTowardsCamera.position = transform.position;
        gameObject.SetActive(false);
		Debug.Log ("VRMenuUI Start");
    }

    private Vector2 MousePositionCentered()
    {
        float deltaX = Input.mousePosition.x - Screen.width / 2;
        float deltaY = Input.mousePosition.y - Screen.height / 2;
        return new Vector2(deltaX, deltaY);
    }

#if UNITY_ANDROID
    private bool ClickButton(int button)
    {
        return GvrControllerInput.ClickButton;
    }

    private Vector2 GetTouchPosition()
    {
        return GvrControllerInput.TouchPosCentered;
    }
#endif

    private void Update ()
    {
		if (GetPressed(0))
        {
            Vector2 position = TouchPosition();
            Touch(position);
        }

        LookTowardsCamera.position = VRController.Instance.Hand.transform.position + Vector3.up * 0.25f;
        LookTowardsCamera.LookAt(VRController.Instance.MainCamera.transform.position, VRController.Instance.MainCamera.transform.up);
		transform.position = LookTowardsCamera.position; //Vector3.Lerp(transform.position, LookTowardsCamera.position, Time.deltaTime * 8f);
        transform.rotation = LookTowardsCamera.rotation;

		_checkAngleDiffDelayTimer += Time.deltaTime;
		Vector3 camForward = VRController.Instance.MainCamera.transform.forward;
		float angle = Vector3.Angle (-transform.forward, camForward);
		if (_checkAngleDiffDelayTimer > _checkAngleDiffDelay && angle > 45) {
			gameObject.SetActive (false);
			Debug.LogFormat ("VRMenuUI Update {0} over 45 degree: {1} {2}", angle, camForward, -transform.forward);
			return;
		}
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
		if (SelectActions.Length > _currentSelectedIndex && SelectActions[_currentSelectedIndex] != null)
        {
            Debug.Log("Confirm");
            SelectActions[_currentSelectedIndex].Invoke();
        }
		gameObject.SetActive (false);
    }

    public void OnFlashlightSelectedAction()
    {
        Debug.Log("OnFlashlightSelected true");
        OnFlashlightSelected = true;
    }
}
