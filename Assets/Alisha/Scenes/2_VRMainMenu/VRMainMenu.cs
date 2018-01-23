using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRMainMenu : MonoBehaviour
{
    public string NextScene;
    private VRController _controller;

    [SerializeField]
    private Canvas _networkUI;

    [SerializeField]
    private Canvas _networkUI1;

    [SerializeField]
    private GameObject _vrMenuUI;

    [SerializeField]
    private CanvasGroup _mainMenu;

    [SerializeField]
    private ALishaNetworkMain _networkMain;

    public Transform Environment;

    private IEnumerator Start()
    {
        //do
        //{
        //    _controller = VRController.Instance;
        //    yield return null;
        //} while (!_controller);
        //_controller.OnPlayerHeightChanged = OnPlayerHeightChanged;
        //_controller.VRMenuUI = _vrMenuUI;
        //_controller.MainCamera.enabled = true;
        yield return null;
    }

    private void Update()
    {
#if UNITY_EDITOR && !USE_DAYDREAM_CONTROLLER
        if (Input.GetMouseButtonDown(0))
#elif UNITY_ANDROID
        if (GvrControllerInput.ClickButtonDown)
#endif
        {
            if (!_networkMain.UIPanelGameObject.activeInHierarchy)
            {
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

    private bool _isFadeOutReady = true;

    public void Fadeout()
    {
        if (_isFadeOutReady)
        {
            _isFadeOutReady = false;
            _mainMenu.DOFade(0, 2).SetEase(Ease.OutQuart).OnComplete(() => SceneManager.LoadScene(NextScene));
        }
    }
}