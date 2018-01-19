using UnityEngine;
using UnityEngine.EventSystems;

public class Inspectable : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;
    private Material _originalMaterial;
    [SerializeField]
    private Material _outlineMaterial;
    [Range(0.0078125f, 0.125f)]
    public float OutlineWidth;
    [Range(0, 1)]
    public float ApicesCompensation;

    private void Start ()
    {
        VRSceneObjectManager.Instance.InspectableObjects.Add(this);

        //_originalMaterial = _renderer.material;
        _outlineMaterial = new Material(_outlineMaterial);
        _outlineMaterial.SetFloat("_OutlineWidth", OutlineWidth);
        _outlineMaterial.SetFloat("_ApicesCompensation", ApicesCompensation);
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnPointerEnter(null);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnPointerExit(null);
        }
    }
#endif

    public void OnPointerEnter(BaseEventData eventData)
    {
        _originalMaterial = _renderer.material;
        _renderer.material = _outlineMaterial;
    }

    public void OnPointerExit(BaseEventData eventData)
    {
         _renderer.material = _originalMaterial;
    }
}
