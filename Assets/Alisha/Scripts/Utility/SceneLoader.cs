using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static IEnumerator LoadSceneAsync(string sceneName, Action<int> onProgressUpdate)
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                displayProgress++;
                if (onProgressUpdate != null)
                {
                    onProgressUpdate(displayProgress);
                }
                yield return WaitPool.Instance.waitForEndOfFrame;
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            displayProgress += UnityEngine.Random.Range(1, 16);
            if (displayProgress > toProgress)
            {
                displayProgress = toProgress;
            }
            if (onProgressUpdate != null)
            {
                onProgressUpdate(displayProgress);
            }
            yield return WaitPool.Instance.waitForEndOfFrame;
        }
        op.allowSceneActivation = true;
    }
}
