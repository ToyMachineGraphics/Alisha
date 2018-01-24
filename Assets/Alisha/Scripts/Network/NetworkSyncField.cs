using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using UnityEngine;

public enum SyncFields
{
    boolArray2_Puzzel_Triggers,
}

public class NetworkSyncField : NetworkBehaviour
{
    #region SyncFields

    public SyncListBool boolArray2_Puzzel_Triggers = new SyncListBool();

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
        if (!isServer)
            return;
        for (int i = 0; i < 2; i++)
        {
            boolArray2_Puzzel_Triggers.Add(false);
        }

        boolArray2_Puzzel_Triggers.Callback += PlzzleUpdate;

        //// Examples
        //Setvalue(SyncFields.boolArray3_DoorState, new bool[3] { true, true, false });
        //Setvalue(SyncFields.boolArray5_Puzzel_Triggers, new bool[5] { true, false, false, false, false });
        //Setvalue(SyncFields.float_ClearTime, 7777f);
    }

    public void Setvalue(SyncFields variableName, object value)
    {
        if (!hasAuthority)
        {
            return;
        }
        _value = value;
        _variableName = variableName.ToString();
        SyncFieldStruct field = new SyncFieldStruct();
        switch (variableName)
        {
            case SyncFields.boolArray2_Puzzel_Triggers:
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
            case SyncFields.boolArray2_Puzzel_Triggers:
                bool[] tempInt = (bool[])_value.GetValue();
                for (int i = 0; i < boolArray2_Puzzel_Triggers.Count; i++)
                {
                    boolArray2_Puzzel_Triggers[i] = tempInt[i];
                }
                break;

            default:
                break;
        }
    }

    private void PlzzleUpdate(SyncListBool.Operation op, int index)
    {
        //WorldText.Instance.text.text = "On element " + index + " of Plzzle Updated, new Value: " + boolArray2_Puzzel_Triggers[index];
        Debug.Log("On element " + index + " of Plzzle Updated, new Value: " + boolArray2_Puzzel_Triggers[index]);
        OnPuzzel_TriggersChanged(boolArray2_Puzzel_Triggers);
    }
}