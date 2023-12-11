using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Skull")]
public class Skull : Pickup
{
    public override void Collected(Hero hero)
    {
        BattleManager.Instance.SkullCollected();
    }
}
