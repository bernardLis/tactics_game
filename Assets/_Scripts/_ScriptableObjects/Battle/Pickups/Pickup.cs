using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : BaseScriptableObject
{
    public GameObject Prefab;
    public GameObject CollectEffect;

    public Sound DropSound;
    public Sound CollectSound;

    public virtual void Initialize()
    {
        
    }

    public virtual void Collected(Hero hero)
    {
        
    }


}
