using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Spice")]
public class CollectableSpice : Collectable
{
    public override void Initialize() { Amount = Random.Range(10, 50); }

    public override void Collect(MapHero hero) { GameManager.Instance.ChangeSpiceValue(Amount); }

}
