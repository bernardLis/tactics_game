using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BattleHero : BattleEntity
{
    public Hero Hero { get; private set; }

    ThirdPersonController _thirdPersonController;

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);

        Hero = (Hero)entity;
        Team = 0;

        _thirdPersonController = GetComponent<ThirdPersonController>();
        _thirdPersonController.SetMoveSpeed(Hero.Speed.GetValue());
        Hero.Speed.OnValueChanged += _thirdPersonController.SetMoveSpeed;
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
