using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldRadioBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public AudioClip AdjustClip;
    public float MaxFreq;
    public float MinFreq;
    public float CurrentMinFreq;
    public float CurrentMaxFreq;
    public float UnitFreq;
    private float deltaFreq;

    public float Frequence
    {
        get
        {
            //frequence = MinFreq + (Mathf.CeilToInt(-transform.localPosition.x / lenth * deltaFreq / UnitFreq) * UnitFreq);
            float frequenceOffset = (initLocalPos.x - transform.localPosition.x) / lenth * deltaFreq;
            frequence = MinFreq + frequenceOffset;
            float roundedFrequence = Mathf.CeilToInt(frequence / UnitFreq) * UnitFreq;
            frequence = roundedFrequence;
            return frequence;
        }
        set
        {
            frequence = value;
            float frequenceDelta = value - MinFreq;
            transform.localPosition = initLocalPos + Vector3.left * (frequenceDelta * lenth / deltaFreq);
            //transform.localPosition = new Vector3(-(frequence - MinFreq) * lenth / deltaFreq, transform.localPosition.y);
        }
    }

    private float frequence;

    private Vector3 initLocalPos;
    private Vector3 lastTouchPos;
    private float lenth;

    public static WorldRadioBar Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Use this for initialization
    private void Start()
    {

    }

    public void Initialize()
    {
        deltaFreq = MaxFreq - MinFreq;
        initLocalPos = transform.localPosition;
        lenth = ((RectTransform)transform).sizeDelta.x;
        Frequence = (CurrentMinFreq + CurrentMaxFreq) / 2;
        Debug.Log(Frequence);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastTouchPos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SEManager.Instance.PlaySEClip(AdjustClip, SEChannels.PlayerTrigger, false, false, false);
        Vector3 delta = Input.mousePosition - lastTouchPos;
        transform.localPosition += Vector3.right * delta.x;
        if (Frequence > MaxFreq || Frequence < MinFreq)
            transform.localPosition -= Vector3.right * delta.x;
        WorldRadioManager.Instance.CurrentFrequence = Frequence;
        WorldRadioManager.Instance.Call();
        lastTouchPos = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Frequence = frequence;
        WorldRadioManager.Instance.Call();
    }
}
