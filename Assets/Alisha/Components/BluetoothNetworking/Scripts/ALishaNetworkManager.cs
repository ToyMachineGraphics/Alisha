using LostPolygon.AndroidBluetoothMultiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ALishaNetworkManager : AndroidBluetoothNetworkManager
{
    public bool IsVR;

    public GameObject VRPlayerPrefab;
    public GameObjectEvent VROnOnServerAddPlayer;

    public GameObject NonVRPlayerPrefab;
    public GameObjectEvent NonVROnOnServerAddPlayer;
    [SerializeField]
    private short MaxPlayers = 2;

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler((short)OperationMessageType.SendScreenshot, OnSendScreenshotRequest);
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        ClientScene.RegisterPrefab(VRPlayerPrefab);
        ClientScene.RegisterPrefab(NonVRPlayerPrefab);
        client.RegisterHandler((short)OperationMessageType.SendScreenshot, OnSendScreenshotResponse);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
        {
            if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
            return;
        }

        GameObject player = null;
        IsVR = extraMessageReader.ReadBoolean();
        if (IsVR)
        {
            Debug.Log("VR OnServerAddPlayer!!!!!!!!!!!!");
            player = Instantiate(VRPlayerPrefab, VRPlayerPrefab.transform.position, Quaternion.identity);
            if (VROnOnServerAddPlayer != null)
            {
                VROnOnServerAddPlayer.Invoke(player);
            }
        }
        else
        {
            Debug.Log("NonVR OnServerAddPlayer!!!!!!!!!!!!");
            player = Instantiate(NonVRPlayerPrefab, NonVRPlayerPrefab.transform.position, Quaternion.identity);
            if (NonVROnOnServerAddPlayer != null)
            {
                NonVROnOnServerAddPlayer.Invoke(player);
            }
        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        if (!clientLoadedScene)
        {
            ClientScene.AddPlayer(conn, 0, new PlayerTypeMessgage() { IsVR = this.IsVR });
        }
    }

    #region Server - Request From Client Handler
    private void OnSendScreenshotRequest(NetworkMessage netMsg)
    {
        SendScreenshotMessage sendScreenshot = netMsg.ReadMessage<SendScreenshotMessage>();

        foreach (NetworkConnection connection in NetworkServer.localConnections)
        {
            if (connection == null || connection == netMsg.conn)
                continue;

            connection.Send((short)OperationMessageType.SendScreenshot, sendScreenshot);
        }
    }
    #endregion

    #region Client - Response From Server Handler
    private void OnSendScreenshotResponse(NetworkMessage netMsg)
    {
        SendScreenshotMessage sendScreenshot = netMsg.ReadMessage<SendScreenshotMessage>();
        Texture2D texture = new Texture2D(sendScreenshot.width, sendScreenshot.height, TextureFormat.RGBA32, true);
        bool succeess =texture.LoadImage(sendScreenshot.ImageBytes);
    }
    #endregion
}

public class GameObjectEvent : UnityEvent<GameObject>
{
}

public class PlayerTypeMessgage : MessageBase
{
    public bool IsVR;
}