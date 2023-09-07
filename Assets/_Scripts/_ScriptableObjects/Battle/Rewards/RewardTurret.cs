using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Turret")]
public class RewardTurret : Reward
{
    public Turret Turret;

    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        Turret = Instantiate(_gameManager.EntityDatabase.GetRandomTurret());
        Turret.InitializeBattle(0);
    }

    public override void GetReward()
    {
        base.GetReward();
        BattleManager.Instance.GetComponent<BattleDeploymentManager>().HandleTurretDeployment(Turret);
    }
}
