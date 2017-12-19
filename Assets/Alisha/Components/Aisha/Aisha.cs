using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Aisha : NetworkBehaviour
{
    public VRController Controller;
    private NetworkIdentity _networkId;
    public GameObject Flashlight;
    private Light _spotlight;

    private void Awake()
    {
        _networkId = GetComponent<NetworkIdentity>();
    }

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
		if (localPlayerAuthority)
        {
            if (Flashlight.activeInHierarchy)
            {
                Flashlight.transform.rotation = Controller.ControllerModel.transform.rotation;
            }
        }
	}

    public void UseSpotlight(bool use)
    {
        Flashlight.SetActive(use);
    }
}
