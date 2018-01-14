using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRMenuUI : MonoBehaviour
{
    public Transform SelectionsRoot;
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
    public enum OnOpen
    {
        None,
        Flashlight,
        Backpack
    }
    public OnOpen OnOpenFlag;
    public bool OnFlashlightSelected;

    public Transform BackpackRoot;
    public Transform BackpackHierachy1;
    public Transform GalleryHierachy1;
    public Transform AishaState;
    public Vector3 BackpackEntryScale;
    public Transform CurrentSelectedBackpackEntry;
    public float SlideThreshold = 0.125f;
    public float SlideTimer = 0;
    public float SlideInterval; // 0.125f

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
        SelectionsRoot.gameObject.SetActive(false);
        BackpackRoot.gameObject.SetActive(false);
        Debug.Log ("VRMenuUI Start");
    }

    private Vector2 _lastTouchPosition;
    private Vector2 _touchPositionDelta;

    private Vector2 MousePositionCentered()
    {
        float deltaX = Input.mousePosition.x - Screen.width / 2;
        float deltaY = Input.mousePosition.y - Screen.height / 2;
        Vector2 centeredPosition = new Vector2(deltaX, deltaY);
        _touchPositionDelta = centeredPosition - _lastTouchPosition;
        _lastTouchPosition = centeredPosition;
        return centeredPosition;
    }

#if UNITY_ANDROID
    private bool ClickButton(int button)
    {
        return GvrControllerInput.ClickButton;
    }

    private Vector2 GetTouchPosition()
    {
        _touchPositionDelta = GvrControllerInput.TouchPosCentered - _lastTouchPosition;
        _lastTouchPosition = GvrControllerInput.TouchPosCentered;
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
		if (OnOpenFlag != OnOpen.None && _checkAngleDiffDelayTimer > _checkAngleDiffDelay && angle > 75) {
            Disable();
            Debug.LogFormat ("VRMenuUI Update {0} over 75 degree: {1} {2}", angle, camForward, -transform.forward);
			return;
		}

        if (BackpackRoot.gameObject.activeInHierarchy)
        {
            BackpackRoot.position = LookTowardsCamera.position;
            BackpackRoot.rotation = LookTowardsCamera.rotation;
            SlideTimer += Time.deltaTime;
            float xAbs = Mathf.Abs(_touchPositionDelta.x);
            if (xAbs > Mathf.Abs(_touchPositionDelta.y))
            {
                bool change = false;
#if UNITY_EDITOR
                if (SlideTimer > SlideInterval && xAbs > SlideThreshold * 32)
                {
                    SlideTimer = 0;
                    change = true;
                }
#elif UNITY_ANDROID
                if (xAbs > SlideThreshold)
                {
                    change = true;
                }
#endif
                if (change)
                {
                    if (BackpackHierachy1 == CurrentSelectedBackpackEntry)
                    {
                        BackpackHierachy1.localScale = BackpackEntryScale;
                        BackpackHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
                        CurrentSelectedBackpackEntry = GalleryHierachy1;
                    }
                    else if (GalleryHierachy1 == CurrentSelectedBackpackEntry)
                    {
                        GalleryHierachy1.localScale = BackpackEntryScale;
                        GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
                        CurrentSelectedBackpackEntry = BackpackHierachy1;
                    }
                    CurrentSelectedBackpackEntry.DOScale(BackpackEntryScale + Vector3.one * 0.015625f, 0.25f);
                    CurrentSelectedBackpackEntry.GetComponent<Renderer>().material = SelectedMaterial;
                }
            }
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
        Disable();
        if (SelectActions.Length > _currentSelectedIndex && SelectActions[_currentSelectedIndex] != null)
        {
            Debug.Log("Confirm");
            SelectActions[_currentSelectedIndex].Invoke();
        }
    }

    public void Disable()
    {
        SelectionsRoot.gameObject.SetActive(false);
        BackpackRoot.gameObject.SetActive(false);
        BackpackHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
        GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
        BackpackHierachy1.localScale = GalleryHierachy1.localScale = AishaState.localScale = BackpackEntryScale;
        OnOpenFlag = OnOpen.None;
        Debug.Log("Disable");
    }

    public void OnFlashlightSelectedAction()
    {
        Debug.Log("OnOpen.Flashlight");
        OnOpenFlag = OnOpen.Flashlight;
    }

    public void OnOpenBackpack()
    {
        Debug.Log("OnOpen.Backpack");
        BackpackRoot.gameObject.SetActive(true);
        CurrentSelectedBackpackEntry = BackpackHierachy1;
        BackpackHierachy1.GetComponent<Renderer>().material = SelectedMaterial;
        GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
        BackpackHierachy1.DOScale(BackpackEntryScale + Vector3.one * 0.0078125f, 0.25f);
        SlideTimer = 0;
        OnOpenFlag = OnOpen.Backpack;
    }
}
