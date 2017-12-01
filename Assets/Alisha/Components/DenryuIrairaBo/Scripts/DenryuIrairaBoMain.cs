using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DenryuIrairaBoMain : MonoBehaviour {
    public GameObject DenryuIrairaBoBase;
    public GameObject DenryuIrairaBo;

    private void Start ()
    {
        DenryuIrairaBo = Instantiate(DenryuIrairaBoBase);
    }
}
