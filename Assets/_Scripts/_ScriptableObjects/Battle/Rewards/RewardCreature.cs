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

        int maxTier = _gameManager.SelectedBattle.Spire.StoreyTroops.CreatureTierTree.CurrentValue.Value;

        Creature baseCreature = _gameManager.HeroDatabase.GetRandomCreatureByUpgradeTierAndLower(maxTier);
        Creature = Instantiate(baseCreature);
    }

    public override void GetReward()
    {
        base.GetReward();
        Creature.InitializeBattle(_hero);
        _hero.AddCreature(Creature);

        BattleDeploymentManager bdsm = BattleManager.Instance.GetComponent<BattleDeploymentManager>();
        bdsm.HandlePlayerArmyDeployment(new List<Creature>() { Creature });
    }
}
