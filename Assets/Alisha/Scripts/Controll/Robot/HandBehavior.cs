using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HandBehavior : MonoBehaviour
{
    private Vector3 _initLocalPos;
    public Animator Anim;
    private Transform TakeTarget;
	public AudioClip OnPuzzel;

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
		transform.DOLocalMove(Vector3.forward * 3 + Vector3.down*0.1f+ Vector3.left*0.1f, 0.5f)
            .SetRelative(true)
            .OnComplete(() =>
            {
                Anim.SetTrigger("get");
                Back();
            });
    }

    public void Back()
    {
        transform.DOLocalMove(_initLocalPos, 0.5f)
			.SetDelay(.2f)
			.OnComplete(() =>
        {
			RobotBehavior.Instance.FlashlightInstance.gameObject.SetActive (true);
            Anim.SetTrigger("open");
            if (TakeTarget)
                Destroy(TakeTarget.gameObject);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!RobotBehavior.Instance.isLocalPlayer)
            return;

        if (!TakeTarget && other.GetComponent<ItemObjectInfomation>())
        {
            TakeTarget = other.transform;
            TakeTarget.DOLocalMove(Vector3.zero, 0.1f);
            TakeTarget.DOScale(0, 0.2f)
                .SetDelay(0.2f);
            TakeTarget.parent = transform;
            ItemManager.Instance.ItemOpen(other.GetComponent<ItemObjectInfomation>().ID);
        }
        if (other.tag == "CircleTrigger")
        {
			SEManager.Instance.PlaySEClip (OnPuzzel, SEChannels.GameEvent, false, false, false);
            transform.DOPause();
            transform.DOKill();
            other.transform.parent.GetComponent<CirclePuzzleManager>().PuzzleTrigger(true);
			RobotBehavior.Instance.FlashlightInstance.gameObject.SetActive (false);
        }
    }
}