using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Aisha : NetworkBehaviour
{
    public VRController Controller;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Controller = VRController.Instance;
    }

    private void Start ()
    {
    }
	
	private void Update ()
    {
		
	}
}
