using System.Collections;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public LoadingPage loadingPageBase;

    public string nextScene;

    private IEnumerator Start()
    {
        yield return ShowIntro();
    }

    private void Update()
    {

    }

    private IEnumerator ShowIntro()
    {
        yield return new WaitForSeconds(2);

        LoadingPage loading = Instantiate(loadingPageBase);
        yield return SceneLoader.LoadSceneAsync(nextScene, loading.OnProgressUpdate);
    }
}
