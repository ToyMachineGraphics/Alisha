using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform AnchorPoint;

    private float _width;
    private float _height;

    // Use this for initialization
    private void Start()
    {
        _width = ((RectTransform)transform).sizeDelta.x;
        _height = ((RectTransform)transform).sizeDelta.y;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}