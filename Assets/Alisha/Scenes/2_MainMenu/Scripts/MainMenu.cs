using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public LoadingPage LoadingPageBase;

    public string NextScene;

    private void Start()
    {
        Info.Source = MonoSingleton<Info>.InstanceSource.Prefab;
        Info.PrefabName = "LogCanvas";
        Info.Persistent = true;
        Info.Instance.InfoText.text = "Debug Info!";
    }

    private void Update()
    {

    }
}
