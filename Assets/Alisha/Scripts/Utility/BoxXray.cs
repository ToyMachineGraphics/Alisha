using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoxXray : MonoBehaviour
{
    public Material BoxXrayMaterial;
    public Material InBoxXrayMaterial;
    public MeshRenderer[] BoxContent;
    public MeshRenderer[] InBoxContent;
    private List<Material[]> BoxMaterials = new List<Material[]>();
    private List<Material[]> InBoxMaterials = new List<Material[]>();

    // Use this for initialization
    private void Start()
    {
        foreach (MeshRenderer InBox in InBoxContent)
        {
            InBoxMaterials.Add(InBox.materials);
        }
        foreach (MeshRenderer Box in BoxContent)
        {
            BoxMaterials.Add(Box.materials);
        }
        if (!DOTween.IsTweening(InBoxXrayMaterial))
        {
            InBoxXrayMaterial.SetFloat("_Rim", 0);
            InBoxXrayMaterial.DOFloat(1, "_Rim", 1)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void SwitchXrayMode(bool toggle)
    {
        for (int i = 0; i < BoxContent.Length; i++)
        {
            if (toggle)
            {
                BoxContent[i].material = BoxXrayMaterial;
            }
            else
            {
                BoxContent[i].materials = BoxMaterials[i];
            }
        }
        for (int i = 0; i < InBoxContent.Length; i++)
        {
            if (toggle)
            {
                InBoxContent[i].material = InBoxXrayMaterial;
            }
            else
            {
                InBoxContent[i].materials = InBoxMaterials[i];
            }
        }
    }
}