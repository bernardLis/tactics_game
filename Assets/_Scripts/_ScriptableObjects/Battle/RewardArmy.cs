using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Reward Army")]
public class RewardArmy : Reward
{
    public Creature Creature { get; private set; }
    public override void CreateRandom(Hero hero)
    {
        base.CreateRandom(hero);

        Creature baseCreature = _gameManager.HeroDatabase.GetRandomCreature();
        Creature = Instantiate(baseCreature);
    }

    public override void GetReward()
    {
        base.GetReward();

        _hero.AddCreature(Creature);
    }
}
