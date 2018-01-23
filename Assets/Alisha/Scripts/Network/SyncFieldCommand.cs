using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncFieldCommand : NetworkBehaviour
{
    public GameObject SyncFieldPrefab;
    public Action OnStageClear = null;

    public static SyncFieldCommand Instance = null;

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
        }
    }
}