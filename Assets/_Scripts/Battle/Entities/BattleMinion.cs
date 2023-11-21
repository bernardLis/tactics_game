using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleMinion : BattleEntity
{
    public Minion Minion { get; private set; }

    BattleHero _targetHero;

    public override void InitializeEntity(Entity entity, int team)
    {
        if (_GFX != null) _GFX.SetActive(true);

        base.InitializeEntity(entity, team);
        Minion = (Minion)entity;

        // minion pool
        IsDead = false;
        _isDeathCoroutineStarted = false;
        Collider.enabled = true;
    }

    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _targetHero = _battleManager.GetComponent<BattleHeroManager>().BattleHero;
        StartRunEntityCoroutine();
    }

    protected override IEnumerator RunEntity()
    {
        if (IsDead) yield break;

        yield return PathToHero();
    }

    IEnumerator PathToHero()
    {
        _agent.stoppingDistance = 2f;
        yield return PathToTarget(_targetHero.transform);

        ReachedHero();
    }

    void ReachedHero()
    {
        Collider.enabled = false;
        SetDead();
        StopAllCoroutines();

        _audioManager.PlaySFX(Minion.ExplosionSound, transform.position);

        Animator.SetTrigger("Attack");

        transform.DOMove(_targetHero.transform.position + Vector3.up * 2, 0.3f);
        transform.DOPunchScale(transform.localScale * 1.2f, 0.2f, 10, 1)
            .SetDelay(0.2f)
            .OnComplete(() =>
            {
                GameObject explosion = Instantiate(Minion.ExplosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.DOMoveY(4, 1f).OnComplete(() => Destroy(explosion, 2f));

                _targetHero.BaseGetHit(5, default);

                _GFX.SetActive(false);
                StartCoroutine(Die(hasLoot: false));
            });
    }

    protected override void DestroySelf()
    {
        gameObject.SetActive(false); // there is a pool of minions
    }
}
