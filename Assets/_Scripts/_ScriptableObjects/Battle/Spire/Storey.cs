using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey")]
public class Storey : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;

    public bool IsPurchased;
    public int Cost;

    public GameObject Prefab;

    public virtual void Initialize()
    {
    }

    public virtual void Purchased()
    {
        IsPurchased = true;
    }
}


