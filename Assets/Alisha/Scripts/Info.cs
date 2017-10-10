using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    #region Singleton
    private static Info _infoBase;
    private static Info _instance;
    public static Info Instance
    {
        get
        {
            if (_instance == null)
            {
                if (_infoBase == null)
                {
                    _infoBase = Resources.Load<Info>("InfoCanvas");
                }
                _instance = Instantiate(_infoBase);
            }
            return _instance;
        }
    }
    #endregion

    public Text info;

    private void Awake()
    {
        #region Singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            info = _instance.info;
        }
        else
        {
            if (_instance.gameObject == gameObject)
            {
                Destroy(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
