using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Creature")]
public class RewardCreature : Reward
{
    public Creature Creature { get; private set; }
    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        int maxTier = 0; // HERE: creature tier

        Creature baseCreature = _gameManager.EntityDatabase.GetRandomCreatureByUpgradeTierAndLower(maxTier);
        Creature = Instantiate(baseCreature);
        Creature.InitializeBattle(0);
    }

    public override void GetReward()
    {
        base.GetReward();
        _hero.AddCreature(Creature);

        BattleDeploymentManager bdsm = BattleManager.Instance.GetComponent<BattleDeploymentManager>();
        bdsm.HandlePlayerArmyDeployment(new List<Creature>() { Creature });
    }
}
