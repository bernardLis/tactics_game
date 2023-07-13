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
        Vector3 pos = new Vector3(0, 10, 0);
        GameObject instance = Instantiate(Prefab, pos, Quaternion.identity);
    }
}


