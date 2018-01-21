using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioData : MonoBehaviour
{
    public List<float> ID = new List<float>();
    public List<float> Frequence = new List<float>();
    public List<int> CharactorID_From = new List<int>();
    public List<string> CharactorFrom_Image = new List<string>();
    public List<float> CharactorID_To = new List<float>();
    public List<string> CharactorTo_Image = new List<string>();
    public List<int> Judge1 = new List<int>();
    public List<float> Judge1_Ref = new List<float>();
    public List<int> Judge2 = new List<int>();
    public List<float> Judge2_Ref = new List<float>();
    public List<float> ResultType = new List<float>();
    public List<string> Result = new List<string>();

    public static RadioData instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        DataLoad();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="freq"></param>
    /// <param name="PlayerID">0 = NonVR, 1= VR</param>
    public bool CheckChennal(float freq, int PlayerID)
    {
        for (int i = 0; i < Frequence.Count; i++)
        {
            if (Frequence[i] == freq && CharactorID_From[i] == PlayerID)
                return true;
        }
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="freq"></param>
    /// <param name="PlayerID">0 = NonVR, 1= VR</param>
    public List<string> GetAduioName(float freq, int PlayerID)
    {
        List<string> temp = new List<string>();
        for (int i = 0; i < Frequence.Count; i++)
        {
            if (Frequence[i] == freq && CharactorID_From[i] == PlayerID)
            {
                if (ResultType[i] == 5)
                    temp.Add(Result[i]);
                else
                    temp.Add("");
            }
        }
        return temp;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="freq"></param>
    /// <param name="PlayerID">0 = NonVR, 1= VR</param>
    public List<string> GetTalk(float freq, int PlayerID)
    {
        List<string> temp = new List<string>();
        for (int i = 0; i < Frequence.Count; i++)
        {
            if (Frequence[i] == freq && CharactorID_From[i] == PlayerID)
            {
                if (ResultType[i] == 0)
                    temp.Add(Result[i]);
                else
                    temp.Add("");
            }
        }
        return temp;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="freq"></param>
    /// <param name="PlayerID">0 = NonVR, 1= VR</param>
    public List<string> GetImageID(float freq, int PlayerID)
    {
        List<string> temp = new List<string>();
        for (int i = 0; i < Frequence.Count; i++)
        {
            if (Frequence[i] == freq && CharactorID_From[i] == PlayerID)
            {
                temp.Add(CharactorTo_Image[i]);
            }
        }
        return temp;
    }

    public void DataLoad()
    {
        ID.Clear();
        Frequence.Clear();
        CharactorID_From.Clear();
        CharactorFrom_Image.Clear();
        CharactorID_To.Clear();
        CharactorTo_Image.Clear();
        Judge1.Clear();
        Judge1_Ref.Clear();
        Judge2.Clear();
        Judge2_Ref.Clear();
        ResultType.Clear();
        Result.Clear();

        string path = "Data/RadioData";
        TextAsset dataText = (TextAsset)Resources.Load(path);
        var dataaaa = Resources.Load(path);
        string[] Rows = dataText.text.Split('\n');
        string[] headData = Rows[0].Split(',');

        for (int i = 0; i < Rows.Length; i++)
        {
            if (Rows[i].Length > 3 && Rows[i].Substring(0, 1) != "#")
            {
                string[] data = Rows[i].Split(',');
                int delta = 0;
                for (int m = 0; m < data.Length; m++)
                {
                    if (headData[m] == "#" || headData[m] == "#/r" || headData[m] == "#\r")
                    {
                        delta++;
                    }
                    else
                    {
                        float v = -1;
                        string s = "";

                        if ((m - delta) == 11 || (m - delta) == 5 || (m - delta) == 3)
                            s = data[m];
                        else
                            v = float.Parse(data[m]);

                        switch (m - delta)
                        {
                            case 0:
                                ID.Add(v);
                                break;

                            case 1:
                                Frequence.Add(v);
                                break;

                            case 2:
                                CharactorID_From.Add((int)v);
                                break;

                            case 3:
                                CharactorFrom_Image.Add(s);
                                break;

                            case 4:
                                CharactorID_To.Add(v);
                                break;

                            case 5:
                                CharactorTo_Image.Add(s);
                                break;

                            case 6:
                                Judge1.Add((int)v);
                                break;

                            case 7:
                                Judge1_Ref.Add(v);
                                break;

                            case 8:
                                Judge2.Add((int)v);
                                break;

                            case 9:
                                Judge2_Ref.Add(v);
                                break;

                            case 10:
                                ResultType.Add(v);
                                break;

                            case 11:
                                Result.Add(s);
                                break;
                        }
                    }
                }
            }
        }
    }
}