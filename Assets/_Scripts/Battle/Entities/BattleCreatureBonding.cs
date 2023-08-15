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
        if (battleCreature.Creature.Level == 3)
            new CreatureCardFull(battleCreature.Creature, isChangingName: true);
        if (battleCreature.Creature.Level == battleCreature.Creature.CreatureAbility.UnlockLevel)
            new CreatureCardFull(battleCreature.Creature, isUnlockingAbility: true);
    }
}
