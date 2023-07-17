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

    public virtual void InitializeBattle()
    {
        if (!IsPurchased) return;
        if (Prefab == null) return;
        GameObject storeyObject = BattleSpire.Instance.gameObject;
        Vector3 pos = new Vector3(0, 10, 0); // TODO: problem is many upgrades are initialized at the same time
        GameObject instance = Instantiate(Prefab, pos, Quaternion.identity);
        instance.transform.parent = storeyObject.transform;
    }

    public void Purchased()
    {
        IsPurchased = true;
        InitializeBattle();
    }


}


