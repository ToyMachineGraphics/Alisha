using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPage : MonoBehaviour
{
    #region Data
    private int _progress;
    #endregion

    public Text progress;

    public void OnProgressUpdate(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        progress.text = percent.ToString();
    }
}
