using System;
using UnityEngine;

public abstract class LookFollow : MonoBehaviour, ISwitch
{
    #region Configuration
    public float ReachThreshold = 0.0078125f;
    public float Sensitivity = 180;
    public float PerFrameSensitivity = 180;
    public float MoveSpeed = 1;
    public float RotateSpeed = 1;
    #endregion

    public enum FollowType
    {
        Transform,
        Position
    }
    public abstract FollowType FollowingType
    {
        get;
    }

    protected bool inited;

    public abstract bool ValidTarget { get; }
    public abstract Vector3 CurrentPosition { get; }

    public event Action<GameObject> OnArrived;

    private Quaternion _originalRotation;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _originalRotation = transform.rotation;

        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody)
        {
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }
    }

    protected void Start()
    {
        enabled = inited;
    }

    public abstract bool Reset(Transform follow);

    private void FixedUpdate()
    {
        if (!ValidTarget || !enabled)
        {
            enabled = inited = false;
            Debug.Log("Stop!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Vector3 direction = CurrentPosition - transform.position;
        float perFrameAngleDiff = 0;
        float totoalAngleDiff = 0;
        Quaternion lookRot = transform.rotation;
        if (direction.magnitude > 0.0009765625f)
        {
            lookRot = Quaternion.LookRotation(direction);
            perFrameAngleDiff = CalculateAngleDifferenceOfRotation(transform.rotation, lookRot);
            totoalAngleDiff = CalculateAngleDifferenceOfRotation(_originalRotation, lookRot);
        }
        if (Mathf.Abs(perFrameAngleDiff) > PerFrameSensitivity)
        {
            Debug.LogWarningFormat("abs angle difference > {0} compared to rotation of last frame: {1}", PerFrameSensitivity, perFrameAngleDiff);
        }
        else if (Mathf.Abs(totoalAngleDiff) > Sensitivity)
        {
            Debug.LogWarningFormat("abs angle difference > {0} compared to original rotation: {1}", Sensitivity, totoalAngleDiff);
        }
        else
        {
            Quaternion q = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * RotateSpeed);
            if (_rigidbody)
            {
                _rigidbody.MoveRotation(q);
                // or
                //Quaternion q = lookRot * Quaternion.Inverse(transform.rotation);
                //_rigidbody.AddTorque(q.x * 16, q.y * 16, q.z * 16, ForceMode.Force);

                _rigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * MoveSpeed);
                // or
                //_rigidbody.AddForce(transform.forward * moveSpeed, ForceMode.Force);
            }
            else
            {
                transform.rotation = q;
                transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed, Space.Self);
            }

            float sqrtDiff = (CurrentPosition - transform.position).sqrMagnitude;
            if (sqrtDiff <= ReachThreshold)
            {
                transform.position = CurrentPosition;
                if (OnArrived != null)
                {
                    OnArrived(gameObject);
                }
                Reset(null);
            }
        }
    }

    private float CalculateAngleDifferenceOfRotation(Quaternion compareDest, Quaternion compareSrc)
    {
        float perFrameAngle = Quaternion.Angle(compareDest, compareSrc);
        if (perFrameAngle > 180)
        {
            perFrameAngle -= 180;
        }
        else if (perFrameAngle < -180)
        {
            perFrameAngle += 180;
        }
        return perFrameAngle;
    }

    #region Switch
    public bool Toggle(bool activate)
    {
        enabled = activate && inited && ValidTarget;
        return enabled;
    }
    #endregion
}