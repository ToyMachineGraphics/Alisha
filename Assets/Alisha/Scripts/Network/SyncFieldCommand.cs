using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncFieldCommand : NetworkBehaviour
{
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
}