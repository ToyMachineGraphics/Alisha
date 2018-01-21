using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaitPool : MonoBehaviour
{
    #region Singleton
    private static bool _isQuitting;
    private static WaitPool _instance;
    public static WaitPool Instance
    {
        get
        {
            if (_isQuitting)
            {
                return null;
            }
            if (_instance == null)
            {
                GameObject go = new GameObject(typeof(WaitPool).Name);
                _instance = go.AddComponent<WaitPool>();
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
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _isQuitting = true;
        }
    }
    #endregion

    public WaitForSeconds this[float second]
    {
        get
        {
            WaitForSeconds wait;
            if (!_pool.TryGetValue(second, out wait))
            {
                wait = _pool[second] = new WaitForSeconds(second);
            }
            return wait;
        }
        set
        {
            _pool[second] = value;
        }
    }

    private Dictionary<float, WaitForSeconds> _pool = new Dictionary<float, WaitForSeconds>();
    public readonly WaitForSeconds WaitForTwoSecond = new WaitForSeconds(2);
    public readonly WaitForSeconds WaitForOneSecond = new WaitForSeconds(1);
    public readonly WaitForSeconds WaitForHalfSecond = new WaitForSeconds(0.5f);
    public readonly WaitForSeconds WaitForQuaterSecond = new WaitForSeconds(0.25f);
    public readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

    private HashSet<ReusableWaitWhile> _runningWaitWhile = new HashSet<ReusableWaitWhile>();
    private HashSet<ReusableWaitWhile> _availableWaitWhile = new HashSet<ReusableWaitWhile>();
    public int WaitWhileAvailableCount
    {
        get { return _availableWaitWhile.Count; }
    }
    public int WaitWhileRunningCount
    {
        get { return _runningWaitWhile.Count; }
    }

    public ReusableWaitWhile GetWaitWhileAvailable(Func<bool> predicate)
    {
        ReusableWaitWhile waitWhile = null;
        if (_availableWaitWhile.Count != 0)
        {
            HashSet<ReusableWaitWhile>.Enumerator e = _availableWaitWhile.GetEnumerator();
            while (e.Current == null && e.MoveNext());
            waitWhile = e.Current;
            waitWhile.Predicate = predicate;
            _availableWaitWhile.Remove(waitWhile);
        }
        else
        {
            waitWhile = new ReusableWaitWhile(predicate);
        }
        waitWhile.Available = false;
        waitWhile.OnComplete = OnWaitWhileComplete;
        _runningWaitWhile.Add(waitWhile);
        return waitWhile;
    }

    private void OnWaitWhileComplete(ReusableWaitWhile waitWhile)
    {
        _runningWaitWhile.Remove(waitWhile);
        _availableWaitWhile.Add(waitWhile);
    }
}
