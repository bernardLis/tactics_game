using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Gold")]
public class CollectableGold : Collectable
{
    public override void Initialize()
    {
        Amount = Random.Range(100, 500);
    }
    public override void Collect(MapHero hero)
    {
        GameManager.Instance.ChangeGoldValue(Amount);

    }
}
