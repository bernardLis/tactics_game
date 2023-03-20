using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public int NumberOfMeleeEnemies;
    public int NumberOfRangedEnemies;

    [HideInInspector] public Character Character;

    public bool Won;

    public void RandomizeBattle()
    {
        NumberOfMeleeEnemies = Random.Range(10, 20);
        NumberOfRangedEnemies = Random.Range(10, 20);
    }

}
