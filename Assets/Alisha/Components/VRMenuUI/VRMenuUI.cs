using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VRMenuUI : MonoBehaviour
{
    public RectTransform Hiarachy1Root;
    public RectTransform[] Hiarachy1Sections;
    [SerializeField]
    private RectTransform _currentHiarachy1SectionSelected;
    public UnityEvent[] Hiarachy1SelectActions;
	public RectTransform Hierachy2BackpackRoot;
	public RectTransform Hierachy2Backpack;
	public RectTransform Hierachy2Gallery;
	public RectTransform Hierachy2Diary;
	public RectTransform CurrentSelectedHierachy2Entry;

    public RectTransform Hierarchy3BackpackRoot;
    public RectTransform Hierarchy3BackpackScrollRect;
    public RectTransform Hierarchy3BackpackMainView;
    public Sprite[] ItemIcons;
    public Sprite[] ItemIntros;
    public string[] ItemWithButtonTexts;
    public Image ItemMainView;
    public Image ItemIntro;
    public Button ItemButton;

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
        Hierarchy1Root,
        Hierachy1Flashlight,
        Hierachy1Backpack,
        Hierachy2Backpack,
        Hierachy3Backpack
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

    public Canvas BackpackHierachy2;

    private void OnEnable()
	{
		_currentSelectedIndex = _lastSelectedIndex = 0;
		for (int i = 0; i < Sections.Length; i++)
		{
			Sections[i].GetComponent<Renderer>().material = _defaultMaterial;
		}
        for (int i = 0; i < Hiarachy1Sections.Length; i++)
        {
            Hiarachy1Sections[i].transform.Find("Select").GetComponent<Image>().enabled = false;
        }
		_checkAngleDiffDelayTimer = 0;
        OnVRMenuUIEnable = true;

        Debug.Log ("VRMenuUI OnEnable");
	}

    private void Start ()
    {
#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
        GetPressed = Input.GetMouseButton;
        TouchPosition = MousePositionCentered;
#elif UNITY_ANDROID
        GetPressed = ClickButton;
        TouchPosition = GetTouchPosition;
#endif
        LookTowardsCamera = new GameObject("LookTowardsCamera").transform;
        LookTowardsCamera.position = transform.position;
        SelectionsRoot.gameObject.SetActive(false);
        Hiarachy1Root.gameObject.SetActive(false);
        BackpackRoot.gameObject.SetActive(false);
		Hierachy2BackpackRoot.gameObject.SetActive (false);

        VRItemScrollEntry.MainUI = this;
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
		if (OnOpenFlag == OnOpen.Hierarchy1Root && GetPressed(0))
        {
            Vector2 position = TouchPosition();
            Touch(position);
        }

        VRController controller = VRController.Instance;
        //LookTowardsCamera.position = controller.Hand.transform.position + (controller.MainCamera.transform.forward + Vector3.up) * 0.25f;
        //LookTowardsCamera.position = controller.MainCamera.transform.position + (Vector3.up + controller.MainCamera.transform.forward) * 0.25f;
        //LookTowardsCamera.LookAt(VRController.Instance.MainCamera.transform.position, controller.MainCamera.transform.up);
		//transform.position = LookTowardsCamera.position; //Vector3.Lerp(transform.position, LookTowardsCamera.position, Time.deltaTime * 8f);
        //transform.rotation = LookTowardsCamera.rotation;

		_checkAngleDiffDelayTimer += Time.deltaTime;
		Vector3 camForward = controller.MainCamera.transform.forward;
		float angle = Vector3.Angle (-transform.forward, camForward);
		if (OnOpenFlag != OnOpen.None && _checkAngleDiffDelayTimer > _checkAngleDiffDelay && angle > 75) {
            Debug.LogFormat("VRMenuUI Update {0} over 75 degree: {1} {2} {3}", angle, camForward, -transform.forward, OnOpenFlag);
            if (OnOpenFlag == OnOpen.Hierarchy1Root)
            {
                controller.State = VRController.ControllerState.WaitForTouchPadUp;
            }
            else
            {
                controller.State = VRController.ControllerState.Normal;
            }
            Disable();
            return;
		}

		if ((false && BackpackRoot.gameObject.activeInHierarchy) || Hierachy2BackpackRoot.gameObject.activeInHierarchy)
        {
            if (GvrControllerInput.TouchDown)
            {
                // Clear touch delta on first touch
                TouchPosition();
            }
            TouchPosition();
#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt) && GetPressed(0))
#elif UNITY_ANDROID
            if (GetPressed(0))
#endif
            {
                #region Deprecated
                //BackpackRoot.gameObject.SetActive(false);
                //BackpackHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
                //GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
                //BackpackHierachy1.localScale = GalleryHierachy1.localScale = AishaState.localScale = BackpackEntryScale;

                //BackpackHierachy2.gameObject.SetActive(true);
                //OnOpenFlag = OnOpen.Hierachy2Backpack;
                #endregion

                Hierachy2BackpackRoot.gameObject.SetActive(false);
                Hierachy2Backpack.Find("Select").GetComponent<Image>().enabled = false;
                Hierachy2Gallery.Find("Select").GetComponent<Image>().enabled = false;
                Hierachy2Diary.Find("Select").GetComponent<Image>().enabled = false;
                if (CurrentSelectedHierachy2Entry == Hierachy2Backpack)
                {
                    Hierarchy3BackpackRoot.gameObject.SetActive(true);
                    OnOpenFlag = OnOpen.Hierachy3Backpack;
                }
            }
            BackpackRoot.position = LookTowardsCamera.position;
            BackpackRoot.rotation = LookTowardsCamera.rotation;
			Hierachy2BackpackRoot.position = LookTowardsCamera.position;
			Hierachy2BackpackRoot.rotation = LookTowardsCamera.rotation;
            SlideTimer += Time.deltaTime;
            float xAbs = Mathf.Abs(_touchPositionDelta.x);
            if (xAbs > Mathf.Abs(_touchPositionDelta.y))
            {
                bool change = false;
#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
                // SlideThreshold for 3d menu: 0.125
                if (SlideTimer > SlideInterval && xAbs > SlideThreshold * 32)
                {
                    SlideTimer = 0;
                    change = true;
                }
#elif UNITY_ANDROID
                if (SlideTimer > SlideInterval && xAbs > SlideThreshold)
                {
                    SlideTimer = 0;
                    change = true;
                }
#endif
                if (change)
                {
					#region Deprecated
//					if (BackpackHierachy1 == CurrentSelectedBackpackEntry)
//                    {
//                        BackpackHierachy1.localScale = BackpackEntryScale;
//                        BackpackHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
//                        CurrentSelectedBackpackEntry = GalleryHierachy1;
//                    }
//					else if (GalleryHierachy1 == CurrentSelectedBackpackEntry)
//                    {
//                        GalleryHierachy1.localScale = BackpackEntryScale;
//                        GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
//                        CurrentSelectedBackpackEntry = BackpackHierachy1;
//                    }
//                    CurrentSelectedBackpackEntry.DOScale(BackpackEntryScale + Vector3.one * 0.015625f, 0.25f);
//                    CurrentSelectedBackpackEntry.GetComponent<Renderer>().material = SelectedMaterial;
					#endregion

					if (Hierachy2Backpack == CurrentSelectedHierachy2Entry) {
						Hierachy2Backpack.Find ("Select").GetComponent<Image>().enabled = false;
						if (_touchPositionDelta.x > 0) {
							Hierachy2Gallery.Find ("Select").GetComponent<Image> ().enabled = true;
							CurrentSelectedHierachy2Entry = Hierachy2Gallery;
						} else {
							Hierachy2Diary.Find ("Select").GetComponent<Image> ().enabled = true;
							CurrentSelectedHierachy2Entry = Hierachy2Diary;
						}
					} else if (Hierachy2Gallery == CurrentSelectedHierachy2Entry) {
						Hierachy2Gallery.Find ("Select").GetComponent<Image>().enabled = false;
						if (_touchPositionDelta.x > 0) {
							Hierachy2Diary.Find ("Select").GetComponent<Image> ().enabled = true;
							CurrentSelectedHierachy2Entry = Hierachy2Diary;
						} else {
							Hierachy2Backpack.Find ("Select").GetComponent<Image> ().enabled = true;
							CurrentSelectedHierachy2Entry = Hierachy2Backpack;
						}
					} else if (Hierachy2Diary == CurrentSelectedHierachy2Entry) {
						Hierachy2Diary.Find ("Select").GetComponent<Image>().enabled = false;
						if (_touchPositionDelta.x > 0) {
							Hierachy2Backpack.Find ("Select").GetComponent<Image> ().enabled = true;
							CurrentSelectedHierachy2Entry = Hierachy2Backpack;
						} else {
							Hierachy2Gallery.Find ("Select").GetComponent<Image> ().enabled = true;
							CurrentSelectedHierachy2Entry = Hierachy2Gallery;
						}
					}
                }
            }
        }
        else if (Hierarchy3BackpackRoot.gameObject.activeInHierarchy)
        {

        }

		if (ObjInfoWindow.Instance.Opened) {
			if (GetPressed (0)) {
				ObjInfoWindow.Instance.HideWindow ();
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
        anglePerSection = 360.0f / Hiarachy1Sections.Length;
        for (int i = 0; i < Hiarachy1Sections.Length || (false && Sections.Length > 0); i++)
        {
            if (deg >= anglePerSection * i && deg < anglePerSection * (i + 1))
            {
                if (_currentSelectedIndex != i)
                {
                    _lastSelectedIndex = _currentSelectedIndex;
                    _currentSelectedIndex = i;
                    _currentSelected = Sections[_currentSelectedIndex];
                    _currentSelected = Hiarachy1Sections[_currentSelectedIndex];
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
            for (int i = 0; i < Hiarachy1Sections.Length; i++)
            {
                Hiarachy1Sections[i].transform.Find("Select").GetComponent<Image>().enabled = false;
            }
            if (_currentSelected.GetComponent<Renderer>() != null)
            {
                _currentSelected.GetComponent<Renderer>().material = SelectedMaterial;
            }
            _currentSelected.transform.Find("Select").GetComponent<Image>().enabled = true;
        }
    }

    public void Confirm()
    {
        Disable();
        if (false &&　SelectActions.Length > _currentSelectedIndex && SelectActions[_currentSelectedIndex] != null)
        {
            Debug.Log("Confirm");
            SelectActions[_currentSelectedIndex].Invoke();
        }
        if (Hiarachy1SelectActions.Length > _currentSelectedIndex && Hiarachy1SelectActions[_currentSelectedIndex] != null)
        {
            Debug.Log("Confirm");
            Hiarachy1SelectActions[_currentSelectedIndex].Invoke();
        }
    }

    public void Disable()
    {
        SelectionsRoot.gameObject.SetActive(false);
        BackpackRoot.gameObject.SetActive(false);
        BackpackHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
        GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
        BackpackHierachy1.localScale = GalleryHierachy1.localScale = AishaState.localScale = BackpackEntryScale;
        BackpackHierachy2.gameObject.SetActive(false);

        Hiarachy1Root.gameObject.SetActive(false);
		Hierachy2BackpackRoot.gameObject.SetActive (false);
        Hierarchy3BackpackRoot.gameObject.SetActive(false);

        OnOpenFlag = OnOpen.None;
        VRController.Instance.ResetHand();
        Debug.Log("Disable");
    }

    public void OnFlashlightSelectedAction()
    {
        Debug.Log("OnOpen.Flashlight");
        OnOpenFlag = OnOpen.Hierachy1Flashlight;
    }

    public void OnOpenBackpack()
    {
        Debug.Log("OnOpen.Backpack");
        BackpackRoot.gameObject.SetActive(true);
		Hierachy2BackpackRoot.gameObject.SetActive (true);
        CurrentSelectedBackpackEntry = BackpackHierachy1;
		CurrentSelectedHierachy2Entry = Hierachy2Backpack;
        BackpackHierachy1.GetComponent<Renderer>().material = SelectedMaterial;
		Hierachy2Backpack.Find ("Select").GetComponent<Image> ().enabled = true;
		GalleryHierachy1.GetComponent<Renderer>().material = _defaultMaterial;
		Hierachy2Gallery.Find ("Select").GetComponent<Image> ().enabled = false;
		Hierachy2Diary.Find ("Select").GetComponent<Image> ().enabled = false;
        BackpackHierachy1.DOScale(BackpackEntryScale + Vector3.one * 0.0078125f, 0.25f);
        SlideTimer = 0;
        OnOpenFlag = OnOpen.Hierachy1Backpack;
    }
}
