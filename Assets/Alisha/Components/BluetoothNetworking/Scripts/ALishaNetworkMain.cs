using LostPolygon.AndroidBluetoothMultiplayer;
using LostPolygon.AndroidBluetoothMultiplayer.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ALishaNetworkMain : MonoBehaviour
{
    public AndroidBluetoothNetworkManagerHelper AndroidBluetoothNetworkManagerHelper;
    public ALishaNetworkManager BluetoothMultiplayerDemoNetworkManager;

    public DeviceBrowserController CustomDeviceBrowser;

    public GameObject UIPanelGameObject;
    public GameObject ErrorUIPanelGameObject;

    public GameObject StartServerButtonGameObject;
    public GameObject ConnectToServerButtonGameObject;
    public GameObject DisconnectButtonGameObject;

    public Toggle UseCustomDeviceBrowserUIToggle;

#if !UNITY_ANDROID
        private void Awake() {
            Debug.LogError("Build platform is not set to Android. Please choose Android as build Platform in File - Build Settings...");
        }

        //protected override void OnEnable() {
        //    base.OnEnable();
        private void OnEnable()
        {
            UIPanelGameObject.SetActive(false);
            ErrorUIPanelGameObject.SetActive(true);
        }
#else

    private void Awake()
    {
        // Enabling verbose logging. See logcat!
        AndroidBluetoothMultiplayer.SetVerboseLog(true);
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Refresh UI
        UIPanelGameObject.SetActive(AndroidBluetoothNetworkManagerHelper.IsInitialized);
        ErrorUIPanelGameObject.SetActive(!AndroidBluetoothNetworkManagerHelper.IsInitialized);

        if (!AndroidBluetoothNetworkManagerHelper.IsInitialized)
            return;

        BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
        StartServerButtonGameObject.SetActive(currentMode == BluetoothMultiplayerMode.None);
        ConnectToServerButtonGameObject.SetActive(currentMode == BluetoothMultiplayerMode.None);
        DisconnectButtonGameObject.SetActive(currentMode != BluetoothMultiplayerMode.None);
        if (DisconnectButtonGameObject.activeInHierarchy)
        {
            DisconnectButtonGameObject.GetComponentInChildren<Text>().text = currentMode == BluetoothMultiplayerMode.Client ? "Disconnect" : "Stop server";
        }

        bool togglesInteractable = currentMode == BluetoothMultiplayerMode.None;
        bool togglesActive = currentMode != BluetoothMultiplayerMode.Client;

        //UseCustomDeviceBrowserUIToggle.interactable = togglesInteractable;
        //UseCustomDeviceBrowserUIToggle.gameObject.SetActive(togglesActive);
    }

    #region UI Handlers

    public void OnStartServerButton()
    {
        AndroidBluetoothNetworkManagerHelper.StartHost();
    }

    public void OnConnectToServerButton()
    {
        //AndroidBluetoothNetworkManagerHelper.SetCustomDeviceBrowser(UseCustomDeviceBrowserUIToggle.isOn ? CustomDeviceBrowser : null);
        AndroidBluetoothNetworkManagerHelper.SetCustomDeviceBrowser(CustomDeviceBrowser);
        AndroidBluetoothNetworkManagerHelper.StartClient();
    }

    public void OnStopServerButton()
    {
        AndroidBluetoothMultiplayer.StopDiscovery();
        AndroidBluetoothMultiplayer.Stop();
        BluetoothMultiplayerDemoNetworkManager.StopHost();
    }

    public void OnDisconnectButton()
    {
        AndroidBluetoothMultiplayer.StopDiscovery();
        AndroidBluetoothMultiplayer.Stop();
        BluetoothMultiplayerDemoNetworkManager.StopClient();
    }

    #endregion UI Handlers

#endif
}