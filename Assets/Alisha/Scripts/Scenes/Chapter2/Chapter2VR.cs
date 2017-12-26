using System.Collections;
using UnityEngine;

public class Chapter2VR : MonoBehaviour
{
    [SerializeField]
    private VRController _controller;
    [SerializeField]
    private DenryuIrairaBo _denryuIrairaBo;

    [SerializeField]
    private Canvas _networkUI;
    [SerializeField]
    private GameObject _vrMenuUI;

    private IEnumerator Start ()
    {
        do
        {
            _controller = VRController.Instance;
        } while (!_controller);
        _controller.OnPlayerHeightChanged = OnPlayerHeightChanged;
        _controller.VRMenuUI = _vrMenuUI;
        _controller.MainCamera.enabled = true;

        _denryuIrairaBo.RaycastAction = AttractDenryuIrairaBoAgent;
        while (!_denryuIrairaBo.gameObject.activeInHierarchy)
        {
            yield return null;
        }

        //if (_denryuIrairaBo.isServer)
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        _denryuIrairaBo.CmdSpawnAgentDefault();
        //        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 3));
        //    }
        //}
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_denryuIrairaBo.isServer)
            {
                _denryuIrairaBo.SpawnAgentDefault();
            }
        }
    }

    public void OnPlayerHeightChanged(float height)
    {
        _networkUI.transform.position = new Vector3(_networkUI.transform.position.x, height, _networkUI.transform.position.z);
        _networkUI.worldCamera = _controller.MainCamera;
        _networkUI.gameObject.SetActive(true);
    }

    private void AttractDenryuIrairaBoAgent()
    {
        if (GvrPointerInputModule.CurrentRaycastResult.isValid)
        {
            _denryuIrairaBo.Target.position = GvrPointerInputModule.CurrentRaycastResult.worldPosition;
        }
    }
}
