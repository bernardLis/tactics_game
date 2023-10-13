using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;
using DG.Tweening;

public class BattleMinion : BattleEntity
{
    public Minion Minion { get; private set; }

    BattleHero _targetHero;

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Minion = (Minion)entity;
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);

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
        yield return PathToPosition(_targetHero.transform.position);

        _agent.stoppingDistance = 1.5f;
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            _agent.SetDestination(_targetHero.transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        // reached destination
        _agent.avoidancePriority = 0;
        Animator.SetBool("Move", false);
        _agent.enabled = false;
        Debug.Log($"dist {Vector3.Distance(transform.position, _targetHero.transform.position)}");
        if (Vector3.Distance(transform.position, _targetHero.transform.position) < 2f)
            ReachedHero();
        else
        {
            yield return new WaitForSeconds(0.5f);
            yield return RunEntity();
        }
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

                _targetHero.GetHit(this);

                _GFX.SetActive(false);
                StartCoroutine(Die());
            });
    }


}
