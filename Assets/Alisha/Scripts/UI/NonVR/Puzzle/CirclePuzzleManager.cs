using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CirclePuzzleManager : NetworkBehaviour
{
    public GameObject UnlockImg;
    public GameObject Cue;

    [SyncVar(hook = "UnlockUpdate")]
    private bool _isUnlock;

    private CirclePuzzleBehavior[] Puzzles;

    // Use this for initialization
    private void Start()
    {
        Puzzles = GameObject.FindObjectsOfType<CirclePuzzleBehavior>();
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
        if (isServer)
            _isUnlock = isUnlock;
        else
            Cmd_Unlock(isUnlock);
    }

    [Command]
    private void Cmd_Unlock(bool isUnlock)
    {
        _isUnlock = isUnlock;
    }

    private void UnlockUpdate(bool isUnlock)
    {
        UnlockImg.SetActive(isUnlock);
    }
}