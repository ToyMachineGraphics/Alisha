using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public LoadingPage loadingPageBase;

    public Button vrMode;
    public Button normalMode;
    public string vrScene, normalScene;

    private void Start()
    {

    }

    private void Update()
    {

    }

    #region Button Callback
    public void SelectVRMode()
    {
        GameConfig.isVRMode = true;
        VRGameNetworking.Instance.Connect();
        LoadingPage loading = Instantiate(loadingPageBase);
        StartCoroutine(SceneLoader.LoadSceneAsync(vrScene, loading.OnProgressUpdate));
    }

    public void SelectNormalMode()
    {
        GameConfig.isVRMode = false;
        LoadingPage loading = Instantiate(loadingPageBase);
        StartCoroutine(SceneLoader.LoadSceneAsync(normalScene, loading.OnProgressUpdate));
    }
    #endregion
}
