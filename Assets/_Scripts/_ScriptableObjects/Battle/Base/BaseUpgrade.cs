using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Base/Base Upgrade")]
public class BaseUpgrade : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;

    public bool Purchased;
    public int Cost;

    public GameObject Prefab;

    public virtual void Initialize()
    {

    }

    public virtual void InitializeBattle()
    {
        if (!Purchased) return;
        if (Prefab == null) return;
        GameObject instance = Instantiate(Prefab, Vector3.zero, Quaternion.identity);
    }
}


