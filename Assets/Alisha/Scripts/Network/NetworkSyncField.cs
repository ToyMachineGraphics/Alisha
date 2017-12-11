using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEngine;

public enum SyncFields
{
    float_ClearTime,
    intArray5_Puzzel1_Triggers,
    boolArray3_DoorState
}

public class NetworkSyncField : NetworkBehaviour
{
    [SyncVar(hook = "ClearTimeUpdate")]
    public float float_ClearTime = 99f;

    public SyncListInt intArray5_Puzzel1_Triggers = new SyncListInt();

    public SyncListBool boolArray3_DoorState = new SyncListBool();

    private string _variableName;
    private object _value;

    public static NetworkSyncField Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Examples
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            intArray5_Puzzel1_Triggers.Add(0);
        }
        for (int i = 0; i < 3; i++)
        {
            boolArray3_DoorState.Add(false);
        }
        intArray5_Puzzel1_Triggers.Callback += Plzzle1Update;
        boolArray3_DoorState.Callback += DoorStateUpdate;
        Setvalue(SyncFields.boolArray3_DoorState, new bool[3] { true, true, false });
        Setvalue(SyncFields.intArray5_Puzzel1_Triggers, new int[5] { 5, 0, 0, 0, 0 });
        Setvalue(SyncFields.float_ClearTime, 7777f);
    }

    public void Setvalue(SyncFields variableName, object value)
    {
        _value = value;
        _variableName = variableName.ToString();
        SyncFieldStruct field = new SyncFieldStruct();
        switch (variableName)
        {
            case SyncFields.float_ClearTime:
                field = new SyncFieldStruct(SyncFieldType.Float, value);
                break;

            case SyncFields.intArray5_Puzzel1_Triggers:
                field = new SyncFieldStruct(SyncFieldType.IntArray, value);
                break;

            case SyncFields.boolArray3_DoorState:
                field = new SyncFieldStruct(SyncFieldType.BoolArray, value);
                break;

            default:
                break;
        }
        Cmd_setvalue(_variableName, field);
    }

    [Command]
    public void Cmd_setvalue(string _variableName, SyncFieldStruct _value)
    {
        ////string name = Enum.GetName(variableName.GetType(), variableName);
        //Enum varName = variableName;
        //string name = varName.ToString("G");
        //fidleTemp = this.GetType().GetField(_variableName);

        //Debug.Log("before: " + fidleTemp.GetValue(this).ToString());

        //fidleTemp.SetValue(this, (object)_value);

        //Debug.Log("After: " + fidleTemp.GetValue(this).ToString());

        SyncFields variableEnum = (SyncFields)Enum.Parse(typeof(SyncFields), _variableName);

        switch (variableEnum)
        {
            case SyncFields.float_ClearTime:
                float_ClearTime = (float)_value.GetValue();
                break;

            case SyncFields.intArray5_Puzzel1_Triggers:
                int[] tempInt = (int[])_value.GetValue();
                for (int i = 0; i < intArray5_Puzzel1_Triggers.Count; i++)
                {
                    intArray5_Puzzel1_Triggers[i] = tempInt[i];
                }
                break;

            case SyncFields.boolArray3_DoorState:
                bool[] tempBool = (bool[])_value.GetValue();
                for (int i = 0; i < boolArray3_DoorState.Count; i++)
                {
                    boolArray3_DoorState[i] = tempBool[i];
                }
                break;

            default:
                break;
        }
    }

    private void DoorStateUpdate(SyncListBool.Operation op, int index)
    {
        Debug.Log("On element " + index + " of DoorState Updated, new Value: " + boolArray3_DoorState[index]);
    }

    private void Plzzle1Update(SyncListInt.Operation op, int index)
    {
        Debug.Log("On element " + index + " of Plzzle1 Updated, new Value: " + intArray5_Puzzel1_Triggers[index]);
    }

    private void ClearTimeUpdate(float time)
    {
        Debug.Log("On ClearTime Updated, new Value: " + time);
    }
}