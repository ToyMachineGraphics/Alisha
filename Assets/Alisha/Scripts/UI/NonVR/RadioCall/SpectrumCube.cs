using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumCube : MonoBehaviour
{
    public int Bend;
    public float StartHeight;
    public float ScaleMultiplier;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, (SpectrumData._freqBend[Bend] * ScaleMultiplier) + StartHeight);
    }
}