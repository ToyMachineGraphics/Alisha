using UnityEngine;

public class LookFollowTransform : LookFollow
{
    public Transform Follow;
    public override FollowType FollowingType
    {
        get
        {
            return FollowType.Transform;
        }
    }
    public override bool ValidTarget
    {
        get
        {
            return (Follow != null) && Follow.gameObject.activeInHierarchy;
        }
    }
    public override Vector3 CurrentPosition
    {
        get
        {
            return Follow.position;
        }
    }

    public override bool Reset(Transform follow)
    {
        this.Follow = follow;
        inited = (follow != null);
        return inited;
    }
}