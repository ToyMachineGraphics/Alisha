using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class VRItemScrollEntry : MonoBehaviour
{
    public static VRMenuUI MainUI;
    public int DataIndex;

    public void OnValidate()
    {
        DataIndex = int.Parse(Regex.Match(name, @"\d+").Value);
    }

    public void SetDataOnMainView()
    {
        Sprite icon = MainUI.ItemIcons[DataIndex];
        Sprite intro = MainUI.ItemIntros[DataIndex];
        string buttonText = MainUI.ItemWithButtonTexts[DataIndex];
        MainUI.ItemMainView.sprite = icon;
        MainUI.ItemMainView.SetNativeSize();
        MainUI.ItemIntro.sprite = intro;
        MainUI.ItemIntro.SetNativeSize();
        bool empty = string.IsNullOrEmpty(buttonText);
        MainUI.ItemButton.gameObject.SetActive(!empty);
        if (!empty)
        {
            MainUI.ItemButton.transform.Find("Text").GetComponent<Text>().text = buttonText;
        }
    }
}
