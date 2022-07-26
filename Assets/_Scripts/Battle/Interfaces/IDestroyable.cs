using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IDestroyable
{
    public Task DestroySelf();
}
