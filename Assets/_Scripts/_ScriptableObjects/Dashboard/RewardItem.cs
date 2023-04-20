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

    Item _item;
    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        float chance = Random.value;
        float uncommonChance = UncommonItemChancePerLevel * _hero.Level.Value;
        float rareChance = RareItemChancePerLevel * _hero.Level.Value;
        float epicChance = EpicItemChancePerLevel * _hero.Level.Value;

        if (chance < uncommonChance)
            _item = _gameManager.HeroDatabase.GetRandomItem(ItemRarity.Uncommon);
        else if (chance < rareChance)
            _item = _gameManager.HeroDatabase.GetRandomItem(ItemRarity.Rare);
        else if (chance < epicChance)
            _item = _gameManager.HeroDatabase.GetRandomItem(ItemRarity.Epic);
        else
            _item = _gameManager.HeroDatabase.GetRandomItem(ItemRarity.Common);
    }

    public override void GetReward()
    {
        _hero.AddItem(_item);
    }
}
