using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Exp Orb")]
public class ExpOrb : BaseScriptableObject
{
    public int Exp;
    public ColorVariable Color;
    public int OrbChance;

    public GameObject Prefab;
    public GameObject CollectEffect;

    public Sound DropSound;
    public Sound CollectSound;

    public void Collected(Hero hero)
    {
        hero.AddExp(Exp);
    }
}
