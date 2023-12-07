using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Hammer")]

public class Hammer : Pickup
{

    public override void Collected(Hero hero)
    {
        BattleManager.Instance.GetComponent<BattleVaseManager>().BreakAllVases();
    }

}
