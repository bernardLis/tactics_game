using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCreatureBonding : MonoBehaviour
{
    BattleCreature battleCreature;
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out BattleCreature bc))
        {
            battleCreature = bc;
            bc.OnInitialized += Initialize;
        }
    }

    void Initialize()
    {
        battleCreature.Creature.OnLevelUp += OnLevelUp;
    }

    void OnLevelUp()
    {
        if (battleCreature.Creature.Level == 3)
        {
            new CreatureCardFull(battleCreature.Creature);
        }
        if (battleCreature.Creature.Level == 6)
        {
            new CreatureCardFull(battleCreature.Creature);
        }


    }
}
