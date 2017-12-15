using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CirclePuzzleManager : MonoBehaviour
{
    public GameObject UnlockImg;
    public GameObject Cue;

    private bool _isUnlock;

    private CirclePuzzleBehavior[] Puzzles;

    // Use this for initialization
    private void Start()
    {
        Puzzles = GameObject.FindObjectsOfType<CirclePuzzleBehavior>();
        NetworkSyncField.Instance.OnPuzzel_TriggersChanged += UnlockUpdate;
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < Puzzles.Length; i++)
        {
            if (Puzzles[i].CurrentSplits == 0 && i == Puzzles.Length - 1)
                Unlock(true);
            else if (Puzzles[i].CurrentSplits != 0)
            {
                Unlock(false);
                break;
            }
        }
    }

    public void PuzzleTrigger(bool Active)
    {
        foreach (var puzzle in Puzzles)
        {
            puzzle.IsActive = Active;
        }
        if (!Active)
        {
            HandBehavior.Instance.Back();
            Cue.SetActive(false);
        }
        else
        {
            Cue.SetActive(true);
        }
    }

    private void Unlock(bool isUnlock)
    {
        NetworkSyncField.Instance.Setvalue(SyncFields.boolArray5_Puzzel_Triggers, new bool[5] { isUnlock, false, false, false, false });
        //if (isServer)
        //    _isUnlock = isUnlock;
        //else
        //    Cmd_Unlock(isUnlock);
    }

    //[Command]
    //private void Cmd_Unlock(bool isUnlock)
    //{
    //    _isUnlock = isUnlock;
    //}

    private void UnlockUpdate(SyncListBool isUnlock)
    {
        UnlockImg.SetActive(isUnlock[0]);
    }
}