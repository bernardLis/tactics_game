using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
public class BattleEntity : MonoBehaviour
{
    List<BattleEntity> _opponentList = new();

    [SerializeField] GameObject _gfx;
    Stats _stats;
    float _currentHealth;

    BattleEntity _opponent;
    NavMeshAgent _agent;

    float _currentAttackCooldown;

    bool _gettingHit;
    public bool IsDead { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action<BattleEntity> OnDeath;

    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
    }

    public void Initialize(Stats stats, ref List<BattleEntity> opponents)
    {
        _stats = stats;
        _currentHealth = stats.Health;

        _gfx.GetComponent<MeshRenderer>().material = stats.Material;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = stats.AttackRange;

        _opponentList = opponents;

        StartCoroutine(RunEntity());
    }


    IEnumerator RunEntity()
    {
        while (!IsDead)
        {
            if (_opponentList.Count == 0)
            {
                Celebrate();
                yield break;
            }
            if (_opponent == null || _opponent.IsDead)
                ChooseNewTarget();

            _agent.enabled = true;
            _agent.destination = _opponent.transform.position;
            yield return new WaitForSeconds(0.2f);

            // path to target
            while (_agent.remainingDistance > _agent.stoppingDistance)
            {
                if (_opponent == null)
                    break;
                _agent.destination = _opponent.transform.position;
                transform.LookAt(_opponent.transform);
                yield return null;
            }

            // reached destination
            _agent.enabled = false;

            yield return StartCoroutine(Attack());
        }
    }

    void ChooseNewTarget()
    {
        _opponent = _opponentList[Random.Range(0, _opponentList.Count)];
    }

    IEnumerator Attack()
    {
        while (_currentAttackCooldown > 0)
            yield return null;
        while (_gettingHit)
            yield return null;
        if (_opponent == null || _opponent.IsDead)
            yield break;
        if (Vector3.Distance(transform.position, _opponent.transform.position) > _stats.AttackRange)
            yield break; // target ran away

        transform.LookAt(_opponent.transform.position);
        Vector3 punchRotation = new(45f, 0f, 0f);
        yield return _gfx.transform.DOPunchRotation(punchRotation, 0.6f, 0, 0).WaitForCompletion();
        _currentAttackCooldown = _stats.AttackCooldown;
        yield return _opponent.GetHit(_stats.Power);
    }

    void Celebrate()
    {
        transform.LookAt(Camera.main.transform);
        transform.DOJump(transform.position + Vector3.up * Random.Range(0.2f, 1.5f), 1, 1, 1f)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .SetDelay(Random.Range(0.5f, 1f));
    }

    public float GetTotalHealth() { return _stats.Health; }
    public float GetCurrentHealth() { return _currentHealth; }

    public IEnumerator GetHit(float power)
    {
        if (IsDead)
            yield break;

        _gettingHit = true;
        _currentHealth -= power;
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            yield return Die();
            yield break;
        }
        yield return transform.DOShakePosition(0.2f, 0.5f).WaitForCompletion();
        _gettingHit = false;
    }

    public IEnumerator Die()
    {
        IsDead = true;
        OnDeath?.Invoke(this);
        yield return new WaitForSeconds(0.2f);
        yield return transform.DORotate(new Vector3(90, 0, 0), 0.5f).SetEase(Ease.OutBounce).WaitForCompletion();
        Destroy(gameObject);
    }
}
