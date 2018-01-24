using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CirclePuzzleManager : NetworkBehaviour
{
	public AudioClip SolvedSE;
    public int LockID;
    //public GameObject UnlockFx;
    public GameObject Cue;

    private bool _isUnlock;

    public CirclePuzzleBehavior[] Puzzles;

    // Use this for initialization
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        NetworkSyncField.Instance.OnPuzzel_TriggersChanged += UnlockUpdate;
    }

    // Update is called once per frame
    private void Update()
    {

        for (int i = 0; i < Puzzles.Length; i++)
        {
			if (Puzzles [i].CurrentSplits == 0 && i == Puzzles.Length - 1 ) {
				if (_isUnlock) {
					return;
				}
				SEManager.Instance.PlaySEClip (SolvedSE, SEChannels.GameEvent, false, false, false);
				RobotBehavior.Instance.SyncCmd.Cmd_SetPuzzleUnlock (LockID);
				_isUnlock = true;
			}
            else if (Puzzles[i].CurrentSplits != 0)
            {
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
            if (HandBehavior.Instance)
            {
                HandBehavior.Instance.Back();
            }
            Cue.SetActive(false);
        }
        else
        {
            Cue.SetActive(true);
        }
    }

    private void Unlock(bool isUnlock)
    {
        bool[] temp = new bool[2];
        for (int i = 0; i < 2; i++)
        {
            temp[i] = NetworkSyncField.Instance.boolArray2_Puzzel_Triggers[i];
        }
        temp[LockID] = true;
        NetworkSyncField.Instance.Setvalue(SyncFields.boolArray2_Puzzel_Triggers, temp);

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
        PuzzleTrigger(false);
        //UnlockFx.SetActive(isUnlock[LockID]);
    }
}