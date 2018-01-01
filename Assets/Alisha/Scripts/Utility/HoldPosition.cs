using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPosition : MonoBehaviour
{
    public Transform Target;

    // Update is called once per frame
    private void Update()
    {
        transform.position = Target.position;
    }
}