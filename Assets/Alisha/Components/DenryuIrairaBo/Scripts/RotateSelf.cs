using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public enum RotateReference
    {
        Local, World
    }
    public RotateReference Reference;
    public Vector3 Axis;
    public float Speed;

    private void Update ()
    {
        transform.Rotate(Axis, Time.deltaTime * Speed, Reference == RotateReference.World ? Space.World : Space.Self);
	}
}
