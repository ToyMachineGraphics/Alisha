using System.Collections;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public LoadingPage loadingPageBase;

    public string nextScene;

    private IEnumerator Start ()
    {
        yield return ShowLogo();
	}

    private void Update ()
    {
		
	}

    private IEnumerator ShowLogo()
    {
        yield return new WaitForSeconds(2);

        LoadingPage loading = Instantiate(loadingPageBase);
        yield return SceneLoader.LoadSceneAsync(nextScene, loading.OnProgressUpdate);
    }
}
