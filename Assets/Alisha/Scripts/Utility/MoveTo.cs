using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    public void Move(Transform target)
    {
        transform.position = target.position;
    }
}