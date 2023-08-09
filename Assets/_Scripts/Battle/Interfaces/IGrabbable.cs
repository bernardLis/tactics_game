using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    public abstract bool CanBeGrabbed();
    public abstract void Grabbed();
    public abstract void Released();
}
