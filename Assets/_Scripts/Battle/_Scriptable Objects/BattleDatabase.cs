using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BattleDatabase")]
public class BattleDatabase : BaseScriptableObject
{
    public TilemapBiome[] biomes;
    public MapVariant[] mapVariants;
    public Brain[] enemyBrains;


    public TilemapBiome GetRandomBiome()
    {
        return biomes[Random.Range(0, biomes.Length)];
    }

    public MapVariant GetRandomMapVariant()
    {
        return mapVariants[Random.Range(0, mapVariants.Length)];
    }

    public Brain GetRandomEnemyBrain()
    {
        return enemyBrains[Random.Range(0, enemyBrains.Length)];
    }
}
