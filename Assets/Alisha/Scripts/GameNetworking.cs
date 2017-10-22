using UnityEngine;
using UnityEngine.Events;

public sealed class GameNetworking : Photon.PunBehaviour
{
    #region Singleton
    private static GameNetworking _instance = null;
    public static GameNetworking Instance
    {
        get
        {
            EnsureInstantiated();
            return _instance;
        }
        set
        {
            _instantiated = (value != null);
            _instance = value;
        }
    }

    private static bool _instantiated = false;
    private static bool _quitting = false;
    /// <summary>
    /// Stay alive across scenes.
    /// </summary>
    public static bool persistent;

    private static void EnsureInstantiated()
    {
        if (!_instantiated)
        {
            if (_quitting)
                return;

            GameNetworking[] instances = FindObjectsOfType<GameNetworking>();
            if (instances != null && instances.Length > 0)
            {
                for (int i = 1; i < instances.Length; i++)
                {
                    Destroy(instances[i]);
                }
                _instance = instances[0];
            }

            if (_instance == null)
            {
                GameObject singleton = new GameObject(typeof(GameNetworking).FullName + "(Singleton)");
                _instance = singleton.AddComponent<GameNetworking>();
            }

            _instantiated = true;

            if (persistent)
            {
                DontDestroyOnLoad(_instance.gameObject);
            }
        }
    }
    #endregion

    [Tooltip("The maximum number of players per room")]
    public byte maxPlayersPerRoom = 2;

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    private bool isConnecting;

    public UnityEvent onConnectedToMaster;
    public UnityEvent connectActionWhenAlreadyConnected;
    public UnityEvent onDisconnectedFromPhoton;

    /// <summary>
    /// Always call this method if overriding.
    /// </summary>
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

        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
        isConnecting = true;

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            //Debug.Log("Joining Room...");
            //Info.Instance.info.text = "Joining Room...";
            //// #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            ////PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom);
            Info.Instance.info.text = "Already Connected...";
            if (connectActionWhenAlreadyConnected != null)
            {
                connectActionWhenAlreadyConnected.Invoke();
            }
        }
        else
        {
            //Debug.Log("Connecting...");
            Info.Instance.info.text = "Connecting...";

            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(GameConfig.gameVersion);
        }
    }

    /// <summary>
    /// Called after the connection to the master is established and authenticated but only when PhotonNetwork.autoJoinLobby is false.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Region:" + PhotonNetwork.networkingPeer.CloudRegion);

        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            Info.Instance.info.text = ("OnConnectedToMaster: Next -> try to Join Random Room");
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

            //// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            //PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom);
            if (onConnectedToMaster != null)
            {
                onConnectedToMaster.Invoke();
            }
        }
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    /// <remarks>
    /// In some cases, other callbacks are called before OnDisconnectedFromPhoton is called.
    /// Examples: OnConnectionFail() and OnFailedToConnectToPhoton().
    /// </remarks>
    public override void OnDisconnectedFromPhoton()
    {
        //LogFeedback("<Color=Red>OnDisconnectedFromPhoton</Color>");
        Info.Instance.info.text = "<Color=Red>OnDisconnectedFromPhoton</Color>";
        Debug.LogError("DemoAnimator/Launcher:Disconnected");

        // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
        //loaderAnime.StopLoaderAnimation();

        isConnecting = false;
        //controlPanel.SetActive(true);

        if (onDisconnectedFromPhoton != null)
        {
            onDisconnectedFromPhoton.Invoke();
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disconnect");
        PhotonNetwork.Disconnect();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instantiated = false;
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        _quitting = true;
    }
}
