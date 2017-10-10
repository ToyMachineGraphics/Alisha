using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRGameNetworking : Photon.PunBehaviour
{
    [Tooltip("The maximum number of players per room")]
    public byte maxPlayersPerRoom = 2;

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    private bool isConnecting;

    #region Singleton
    private static VRGameNetworking vrGameNetworkingBase;
    private static VRGameNetworking _instance;
    public static VRGameNetworking Instance
    {
        get
        {
            if (_instance == null)
            {
                if (vrGameNetworkingBase == null)
                {
                    vrGameNetworkingBase = Resources.Load<VRGameNetworking>("VRGameNetworking");
                }
                _instance = Instantiate(vrGameNetworkingBase);
            }
            return _instance;
        }
    }
    #endregion

    private void Awake ()
    {
        #region Singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
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

        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }

    private void Update ()
    {
		
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
            Info.Instance.info.text = "Joining Room...";
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            //PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom);
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

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()
            PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom);
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disconnect");
        PhotonNetwork.Disconnect();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Disconnect");
        PhotonNetwork.Disconnect();
    }
}
