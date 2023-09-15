using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHero : MonoBehaviour
{
    public Hero Hero { get; private set; }
    public void InitializeHero(Hero hero)
    {
        Hero = hero;
    }
}
