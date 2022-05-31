using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Effect : MonoBehaviour
{
    public virtual async Task Play(Ability ability, Vector3 targetPos)
    {
        // meant to be overwritten
        await Task.Yield();
    }

}
