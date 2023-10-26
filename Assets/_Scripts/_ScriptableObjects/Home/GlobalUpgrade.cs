using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global Upgrade")]
public class GlobalUpgrade : BaseScriptableObject
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


