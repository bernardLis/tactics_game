using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Experience Orb")]
public class ExperienceOrb : Pickup
{
    public int Amount;
    public ColorVariable Color;
    public int OrbChance;

    public override void Initialize()
    {
        base.Initialize();
        Amount += Mathf.RoundToInt(Amount
                    * GameManager.Instance.GlobalUpgradeBoard.GetUpgradeByName("Hero Exp Bonus").GetValue()
                    * 0.01f);
    }

    public override void Collected(Hero hero)
    {
        hero.AddExp(Amount);
    }
}
