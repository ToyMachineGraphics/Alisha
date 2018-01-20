using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, RobotBehavior.Instance.transform.localEulerAngles.y);
    }
}