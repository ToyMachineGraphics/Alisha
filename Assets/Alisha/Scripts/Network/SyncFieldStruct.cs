using System.Collections;
using System.Collections.Generic;

public enum SyncFieldType
{
    Float,
    FloatArray,
    Int,
    IntArray,
    Bool,
    BoolArray
}

public struct SyncFieldStruct
{
    private float _floatTemp;
    private int _intTemp;
    private bool _boolTemp;
    private float[] _floatArrayTemp;
    private int[] _intArrayTemp;
    private bool[] _boolArrayTemp;
    public SyncFieldType Type;

    public SyncFieldStruct(SyncFieldType e, object value)
    {
        _floatTemp = 0f;
        _intTemp = 0;
        _boolTemp = false;
        _boolArrayTemp = null;
        _floatArrayTemp = null;
        _intArrayTemp = null;
        Type = e;
        switch (e)
        {
            case SyncFieldType.Float:
                _floatTemp = (float)value;
                break;

            case SyncFieldType.FloatArray:
                _floatArrayTemp = (float[])value;

                break;

            case SyncFieldType.Int:
                _intTemp = (int)value;

                break;

            case SyncFieldType.IntArray:
                _intArrayTemp = (int[])value;

                break;

            case SyncFieldType.Bool:
                _boolTemp = (bool)value;
                break;

            case SyncFieldType.BoolArray:
                _boolArrayTemp = (bool[])value;
                break;
        }
    }

    public object GetValue()
    {
        switch (Type)
        {
            case SyncFieldType.Float:
                return (object)_floatTemp;

            case SyncFieldType.FloatArray:
                return (object)_floatArrayTemp;

            case SyncFieldType.Int:
                return (object)_intTemp;

            case SyncFieldType.IntArray:
                return (object)_intArrayTemp;

            case SyncFieldType.Bool:
                return (object)_boolTemp;

            case SyncFieldType.BoolArray:
                return (object)_boolArrayTemp;

            default:
                return null;
        }
    }
}