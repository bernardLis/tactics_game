using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Horseshoe")]
public class Horseshoe : Pickup
{
    public override void Collected(Hero hero)
    {
        BattleManager.Instance.HorseshoeCollected();
    }
}
