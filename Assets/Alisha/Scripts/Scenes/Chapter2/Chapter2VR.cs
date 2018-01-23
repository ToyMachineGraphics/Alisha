using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Chapter2VR : MonoBehaviour
{
    [SerializeField]
    private VRController _controller;

    [SerializeField]
    private DenryuIrairaBo _denryuIrairaBo;

    [SerializeField]
    private Canvas _networkUI;

    [SerializeField]
    private GameObject _vrMenuUI;

    [SerializeField]
    private CanvasGroup _mainMenu;

    public Transform Environment;
    public ALishaNetworkMain NetworkMain;
    public ALishaNetworkManager ALishaNetworkManager;

    private IEnumerator Start()
    {
        ALishaNetworkManager.OnClientConnectAction = () =>
        {
            NetworkMain.UIPanelGameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(true);
        };
        do
        {
            _controller = VRController.Instance;
            yield return null;
        } while (!_controller);
        _controller.OnPlayerHeightChanged = OnPlayerHeightChanged;
        _controller.VRMenuUI = _vrMenuUI;
        _controller.MainCamera.enabled = true;

        _denryuIrairaBo.RaycastAction = AttractDenryuIrairaBoAgent;
        while (!_denryuIrairaBo.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        //while (true)
        //{
        //    if (DenryuIrairaBoAgent.AgentCount < 50 && _denryuIrairaBo.isServer)
        //    {
        //        _denryuIrairaBo.SpawnAgentDefault();
        //        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3));
        //    }
        //    else
        //    {
        //        yield return null;
        //    }
        //}
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (_denryuIrairaBo.isServer)
        //    {
        //        _denryuIrairaBo.SpawnAgentDefault();
        //    }
        //}

#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
                if (Input.GetMouseButtonDown(0))
#elif UNITY_ANDROID
        if (GvrControllerInput.ClickButtonDown)
#endif
        {
            if (_mainMenu.gameObject.activeInHierarchy)
            {
                _mainMenu.GetComponent<DOTweenAnimation>().DOKill();
                Fadeout();
            }
        }
    }

    public void OnPlayerHeightChanged(float height)
    {
        //_networkUI.transform.position = new Vector3(_networkUI.transform.position.x, height, _networkUI.transform.position.z);
        _networkUI.worldCamera = _controller.MainCamera;
        _networkUI.gameObject.SetActive(true);
    }

    private void AttractDenryuIrairaBoAgent()
    {
        if (GvrPointerInputModule.CurrentRaycastResult.isValid)
        {
            _denryuIrairaBo.Target.position = GvrPointerInputModule.CurrentRaycastResult.worldPosition;
        }
    }

    private bool _isFadeOutReady = true;

    public void Fadeout()
    {
        if (_isFadeOutReady)
        {
            _isFadeOutReady = false;
            _mainMenu.DOFade(0, 2).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                _mainMenu.gameObject.SetActive(false);
                Environment.gameObject.SetActive(true);
                Debug.Log("Enable environment");
            });
        }
    }
}