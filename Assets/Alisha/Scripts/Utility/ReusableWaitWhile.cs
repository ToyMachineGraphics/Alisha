using System;
using UnityEngine;

public class ReusableWaitWhile : CustomYieldInstruction
{
    public Func<bool> Predicate;
    public bool Available;
    public Action<ReusableWaitWhile> OnComplete;

    public override bool keepWaiting
    {
        get
        {
            bool result = Predicate();
            if (!result)
            {
                Available = true;
                if (OnComplete != null)
                {
                    OnComplete(this);
                }
            }
            return result;
        }
    }

    public ReusableWaitWhile(Func<bool> predicate)
    {
        Available = true;
        Predicate = predicate;
    }
}
