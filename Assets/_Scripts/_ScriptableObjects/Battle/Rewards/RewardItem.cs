using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Item")]
public class RewardItem : Reward
{
    float UncommonItemChancePerLevel = 0.1f;
    float RareItemChancePerLevel = 0.05f;
    float EpicItemChancePerLevel = 0.01f;

    public Item Item { get; private set; }
    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        float chance = Random.value;
        float uncommonChance = UncommonItemChancePerLevel * _hero.Level.Value;
        float rareChance = RareItemChancePerLevel * _hero.Level.Value;
        float epicChance = EpicItemChancePerLevel * _hero.Level.Value;

        if (chance < uncommonChance)
            Item = _gameManager.EntityDatabase.GetRandomItem(ItemRarity.Uncommon);
        else if (chance < rareChance)
            Item = _gameManager.EntityDatabase.GetRandomItem(ItemRarity.Rare);
        else if (chance < epicChance)
            Item = _gameManager.EntityDatabase.GetRandomItem(ItemRarity.Epic);
        else
            Item = _gameManager.EntityDatabase.GetRandomItem(ItemRarity.Common);
    }

    public override void GetReward()
    {
        base.GetReward();

        _hero.AddItem(Item);
    }
}
