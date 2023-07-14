using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Base/Base Upgrade")]
public class BaseUpgrade : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;

    public bool IsPurchased;
    public int Cost;

    public GameObject Prefab;

    public virtual void Initialize()
    {

    }

    public virtual void InitializeBattle()
    {
        if (!IsPurchased) return;
        if (Prefab == null) return;
        GameObject baseObject = BattleBase.Instance.gameObject;
        Vector3 pos = new Vector3(0, 10, 0); // TODO: problem is many upgrades are initialized at the same time
        GameObject instance = Instantiate(Prefab, pos, Quaternion.identity);
        instance.transform.parent = baseObject.transform;
    }

    public void Purchased()
    {
        IsPurchased = true;
        InitializeBattle();
    }


}


