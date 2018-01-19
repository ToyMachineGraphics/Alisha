using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class ObjInfoWindow : MonoBehaviour
{
    public DOTweenAnimation TweenAnim;
    public Text InfoText;
    public float ShowCD = 0.1f;
    private float _currentShowCD;
    public Image Hint;
    public bool Hiding;
    private bool _hit;
    public bool Hit
    {
        get { return _hit; }
        set {
            _lastHit = _hit;
            _hit = value;
        }
    }
    private bool _lastHit;
	private bool _opened;
	public bool Opened
	{
		get { return _opened; }
	}

    private static ObjInfoWindow _instance = null;

    public static ObjInfoWindow Instance
    {
        get
        {
            if (!FindObjectOfType<ObjInfoWindow>())
                _instance = ((GameObject)Instantiate(Resources.Load("SceneObjInfoCanvas"))).GetComponent<ObjInfoWindow>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        _currentShowCD = ShowCD;
    }

    // Update is called once per frame
    private void Update()
    {
        //if (_currentShowCD > 0)
        //{
        //    _currentShowCD -= Time.deltaTime;
        //    if (_currentShowCD < 0)
        //    {
        //        TweenAnim.DOPlayBackwards();
        //    }
        //}
    }

    private Tweener _tweener;
    public void ShowWindow(Vector3 pos, Transform camera, string info)
    {
        if (Hiding || _tweener != null && _tweener.IsPlaying() || TweenAnim.transform.localScale.y == 1 ||
			_lastHit == Hit)
        {
            return;
        }
        InfoText.text = "";
        _tweener = InfoText.DOText(info, 12f).SetSpeedBased().SetEase(Ease.OutQuad).OnComplete(OnComplete);
        //InfoText.text = info;
        transform.position = pos;
        transform.LookAt(camera);
        _currentShowCD = ShowCD;
        TweenAnim.DOPlayForward();
    }

    private void OnComplete()
    {
        Hint.gameObject.SetActive(true);
		_opened = true;
    }

    public void HideWindow()
    {
        if (!Hiding)
        {
            Hiding = true;
            TweenAnim.DOPlayBackwards();
        }
    }

    public void OnHidingComplete()
    {
        if (TweenAnim.transform.localScale.y == 0)
        {
            Hiding = false;
			_opened = false;
        }
    }
}