using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class SyncFieldCommand : NetworkBehaviour
{
    public Material BallMat;
    public GameObject SyncFieldPrefab;

    public event Action OnStageClear = null;

    // Use this for initialization
    private void Start()
    {
        if (isServer)
        {
            NetworkServer.Spawn(Instantiate(SyncFieldPrefab));
        }
        if (isLocalPlayer)
        {
            NetworkSyncField.Instance.OnPuzzel_TriggersChanged += UnlockUpdate;
        }
    }

    [Command]
    public void Cmd_SetPuzzleUnlock(int puzzleID)
    {
        bool[] temp = new bool[2];
        for (int i = 0; i < 2; i++)
        {
            temp[i] = NetworkSyncField.Instance.boolArray2_Puzzel_Triggers[i];
        }
        temp[puzzleID] = true;
        NetworkSyncField.Instance.Setvalue(SyncFields.boolArray2_Puzzel_Triggers, temp);
    }

    private void UnlockUpdate(SyncListBool isUnlock)
    {
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (isUnlock[i])
            {
                temp++;
            }
        }
        Cmd_UpdateBallState(temp);
        if (isUnlock[0] && isUnlock[1])
        {
            Debug.Log("Clear");
            if (OnStageClear != null)
                OnStageClear();
            Rpc_OnStageClear();
        }
    }

    [Command]
    private void Cmd_OnStageClear()
    {
        Rpc_OnStageClear();
    }

    [ClientRpc]
    private void Rpc_OnStageClear()
    {
        if (Aisha.Instance.isLocalPlayer)
            Aisha.Instance.GameClear();
    }

    [Command]
    private void Cmd_UpdateBallState(int stage)
    {
        Rpc_UpdateBallState(stage);
    }

    [ClientRpc]
    private void Rpc_UpdateBallState(int stage)
    {
        BallMat.DOKill();
        BallMat.SetColor("_EmissionColor", new Color(0, 0, 0));
        BallMat.DOColor(new Color(0.33f * stage, 0.33f * stage, 0.33f * stage), "_EmissionColor", 1)
            .SetLoops(-1, LoopType.Yoyo);
    }
}