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
        if (_currentShowCD > 0)
        {
            _currentShowCD -= Time.deltaTime;
            if (_currentShowCD < 0)
            {
                TweenAnim.DOPlayBackwards();
            }
        }
    }

    public void ShowWindow(Vector3 pos, Transform camera, string info)
    {
        InfoText.text = info;
        transform.position = pos;
        transform.LookAt(camera);
        _currentShowCD = ShowCD;
        TweenAnim.DOPlayForward();
    }
}