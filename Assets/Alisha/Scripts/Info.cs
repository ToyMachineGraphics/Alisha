using UnityEngine;
using UnityEngine.UI;

public class Info : MonoSingleton<Info>
{
    //#region Singleton
    //private static Info _infoBase;
    //private static Info _instance;
    //public static Info Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            if (_infoBase == null)
    //            {
    //                _infoBase = Resources.Load<Info>("LogCanvas");
    //            }
    //            _instance = Instantiate(_infoBase);
    //        }
    //        return _instance;
    //    }
    //}
    //#endregion

    public Text InfoText;

    //private void Awake()
    //{
    //    #region Singleton
    //    if (_instance == null)
    //    {
    //        _instance = this;
    //        DontDestroyOnLoad(gameObject);
    //        InfoText = _instance.InfoText;
    //    }
    //    else
    //    {
    //        if (_instance.gameObject == gameObject)
    //        {
    //            Destroy(this);
    //        }
    //        else
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //    #endregion
    //}
}
