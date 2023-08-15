using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCreatureBonding : MonoBehaviour
{
    BattleCreature battleCreature;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (TryGetComponent(out battleCreature))
            battleCreature.Creature.OnLevelUp += OnLevelUp;
    }

    void OnLevelUp()
    {
        Debug.Log($"level up {battleCreature.Creature.Level}");
        if (battleCreature.Creature.Level == 3)
        {
            new CreatureCardFull(battleCreature.Creature, true);
        }
        if (battleCreature.Creature.Level == 6)
        {
            new CreatureCardFull(battleCreature.Creature);
        }
    }
}
