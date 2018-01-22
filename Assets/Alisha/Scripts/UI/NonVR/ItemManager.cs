using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Sprite[] ItemSprite;
    public GameObject[] Items;

    public static ItemManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ItemOpen(int ID)
    {
        Items[ID].GetComponent<Button>().enabled = true;
        Items[ID].GetComponent<Image>().sprite = ItemSprite[ID];
    }
}