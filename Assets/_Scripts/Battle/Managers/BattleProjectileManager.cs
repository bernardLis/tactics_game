using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleProjectileManager : PoolManager<BattleProjectileOpponent>
{
    [SerializeField] GameObject _projectilePrefab;

    public void Initialize()
    {
        CreatePool(_projectilePrefab);
    }

}
