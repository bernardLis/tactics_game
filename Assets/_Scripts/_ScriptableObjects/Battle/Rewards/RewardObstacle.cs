using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Obstacle")]
public class RewardObstacle : Reward
{
    public Sprite Icon;
    public Vector3 Size;

    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        Icon = _gameManager.GameDatabase.ObstacleIcon;

        float sizeX = Random.Range(2, 7);
        float sizeY = Random.Range(2, 5);
        float sizeZ = Random.Range(1, 5);

        Size = new(sizeX, sizeY, sizeZ);

    }

    public override void GetReward()
    {
        base.GetReward();
        BattleManager.Instance.GetComponent<BattleDeploymentManager>().HandleObstacleDeployment(Size);
    }
}
