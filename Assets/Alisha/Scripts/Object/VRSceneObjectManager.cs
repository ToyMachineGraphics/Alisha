using System.Collections.Generic;
using UnityEngine;

public sealed class VRSceneObjectManager : MonoBehaviour
{
    #region Singleton
    private static bool _isQuitting;
    private static VRSceneObjectManager _instance;
    public static VRSceneObjectManager Instance
    {
        get
        {
            if (_isQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(VRSceneObjectManager).Name);
                _instance = go.AddComponent<VRSceneObjectManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            if (_instance.gameObject != gameObject)
            {
                Destroy(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            _instance = this;
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
    #endregion

    public HashSet<Attractable> AttractableObjects = new HashSet<Attractable>();
    public HashSet<Inspectable> InspectableObjects = new HashSet<Inspectable>();

    private void Start()
    {
        
    }
}
