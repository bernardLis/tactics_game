using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Bag")]
public class Bag : Pickup
{
    public override void Collected(Hero hero)
    {
        BattleManager.Instance.BagCollected();
    }
}
