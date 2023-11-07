using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Experience Orb")]
public class ExperienceOrb : Pickup
{
    public int Amount;
    public ColorVariable Color;
    public int OrbChance;

    public GameObject Prefab;
    public GameObject CollectEffect;

    public Sound DropSound;
    public Sound CollectSound;

    public void Collected(Hero hero)
    {
        hero.AddExp(Amount);
    }
}
