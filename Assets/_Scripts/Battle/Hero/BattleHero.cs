using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BattleHero : BattleEntity
{

    public Hero Hero { get; private set; }

    List<BattleEntity> _hitters = new();

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);

        Hero = (Hero)entity;
        Team = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out BattleEntity entity))
            if (entity.Team != 0 && !_hitters.Contains(entity))
                GetHit(entity);
    }

    void GetHit(BattleEntity entity)
    {
        _hitters.Add(entity);
        Hero.CurrentHealth.ApplyChange(-5);
        DisplayFloatingText($"{-5}", _gameManager.GameDatabase.GetColorByName("Health").Color);

        if (Hero.CurrentHealth.Value <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Invulnerability(entity));
    }

    IEnumerator Invulnerability(BattleEntity entity)
    {
        yield return new WaitForSeconds(1f);
        _hitters.Remove(entity);
    }

    void Die()
    {
        Debug.Log($"Hero dies...");
    }

}
