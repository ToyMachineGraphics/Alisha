using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldText : MonoBehaviour
{
    public static WorldText Instance;
    public Text text;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}