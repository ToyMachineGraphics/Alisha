using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandBehavior : MonoBehaviour
{
    private Vector3 _initLocalPos;
    public Animator Anim;

    public static HandBehavior Instance = null;

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
        _initLocalPos = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Stretch()
    {
        transform.DORewind();
        transform.DOKill();
        transform.DOLocalMove(Vector3.forward * 3, 0.5f)
            .SetRelative(true)
            .OnComplete(() =>
            {
                Anim.SetTrigger("get");
                Back();
            });
    }

    public void Back()
    {
        transform.DOLocalMove(_initLocalPos, 0.2f).OnComplete(() =>
        {
            Anim.SetTrigger("open");
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!RobotBehavior.Instance.isLocalPlayer)
            return;
        if (other.tag == "CircleTrigger")
        {
            transform.DOPause();
            transform.DOKill();
            other.transform.parent.GetComponent<CirclePuzzleManager>().PuzzleTrigger(true);
        }
    }
}