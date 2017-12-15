using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEngine;

public enum SyncFields
{
    float_ClearTime,
    boolArray5_Puzzel_Triggers,
    boolArray3_DoorState
}

public class NetworkSyncField : NetworkBehaviour
{
    #region SyncFields

    [SyncVar(hook = "ClearTimeUpdate")]
    public float float_ClearTime = 99f;

    public SyncListBool intArray5_Puzzel_Triggers = new SyncListBool();
    public SyncListBool boolArray3_DoorState = new SyncListBool();

    #endregion SyncFields

    #region delegates

    public delegate void SyncListBoolChanged(SyncListBool changedElement);

    public delegate void SyncListIntChanged(SyncListInt changedElement);

    public delegate void SyncListFloatChanged(SyncListFloat changedElement);

    public delegate void BoolChanged(bool changedElement);

    public delegate void IntChanged(int changedElement);

    public delegate void FloatChanged(float changedElement);

    #endregion delegates

    #region Callbacks

    public SyncListBoolChanged OnPuzzel_TriggersChanged;
    public SyncListBoolChanged OnDoorStateChanged;
    public FloatChanged OnClearTimeChanged;

    #endregion Callbacks

    #region Private fields

    private string _variableName;
    private object _value;

    #endregion Private fields

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

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            intArray5_Puzzel_Triggers.Add(false);
        }

        for (int i = 0; i < 3; i++)
        {
            boolArray3_DoorState.Add(false);
        }

        intArray5_Puzzel_Triggers.Callback += PlzzleUpdate;
        boolArray3_DoorState.Callback += DoorStateUpdate;

        //// Examples
        //Setvalue(SyncFields.boolArray3_DoorState, new bool[3] { true, true, false });
        //Setvalue(SyncFields.boolArray5_Puzzel_Triggers, new bool[5] { true, false, false, false, false });
        //Setvalue(SyncFields.float_ClearTime, 7777f);
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

            case SyncFields.boolArray5_Puzzel_Triggers:
                field = new SyncFieldStruct(SyncFieldType.BoolArray, value);
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

            case SyncFields.boolArray5_Puzzel_Triggers:
                bool[] tempInt = (bool[])_value.GetValue();
                for (int i = 0; i < intArray5_Puzzel_Triggers.Count; i++)
                {
                    intArray5_Puzzel_Triggers[i] = tempInt[i];
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
        OnDoorStateChanged(boolArray3_DoorState);
    }

    private void PlzzleUpdate(SyncListBool.Operation op, int index)
    {
        Debug.Log("On element " + index + " of Plzzle Updated, new Value: " + intArray5_Puzzel_Triggers[index]);
        OnPuzzel_TriggersChanged(intArray5_Puzzel_Triggers);
    }

    private void ClearTimeUpdate(float time)
    {
        Debug.Log("On ClearTime Updated, new Value: " + time);
        OnClearTimeChanged(time);
    }
}