using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BattleHero : BattleEntity
{
    public Hero Hero { get; private set; }

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);

        Hero = (Hero)entity;
        Team = 0;
    }

    public void GetHit(BattleEntity entity)
    {
        Hero.CurrentHealth.ApplyChange(-5);
        DisplayFloatingText($"{-5}", _gameManager.GameDatabase.GetColorByName("Health").Color);

        if (Hero.CurrentHealth.Value <= 0)
        {
            Die();
            return;
        }
    }

    void Die()
    {
        Debug.Log($"Hero dies...");
    }

}
