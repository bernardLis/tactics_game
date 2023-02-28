using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
public class BattleEntity : MonoBehaviour
{
    [SerializeField] GameObject _gfx;
    [SerializeField] float _health;
    float _currentHealth;
    [SerializeField] float _power;

    [SerializeField] BattleEntity _enemy;
    NavMeshAgent _agent;

    [SerializeField] float _attackCooldown;
    float _currentAttackCooldown;
    float _attackRange = 2f;

    bool _isDestinationReached;
    public bool IsDead { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    // Start is called before the first frame update

    void Awake()
    {
        _currentHealth = _health;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        _enemy.OnDeath += OnEnemyDeath;

        StartCoroutine(RunEntity());
    }

    void OnEnemyDeath() { transform.LookAt(Camera.main.transform); }

    void Celebrate()
    {
        transform.DOJump(transform.position + Vector3.up * Random.Range(0.2f, 1.5f), 1, 1, 1f)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .SetDelay(Random.Range(0.5f, 1f));
    }

    IEnumerator RunEntity()
    {
        Debug.Log($"run entity");
        while (!IsDead)
        {
            Debug.Log($"after dead check");
            if (_enemy == null || _enemy.IsDead)
            {
                Celebrate();
                yield break;
            }

            Debug.Log($"after celebrate check");

            // path to target
            while (Vector3.Distance(transform.position, _enemy.transform.position) > _attackRange)
            {
                _agent.enabled = true;
                _agent.destination = _enemy.transform.position;
                transform.LookAt(_enemy.transform);
                yield return null;
            }

            Debug.Log($"after path");

            // reached destination
            _agent.enabled = false;

            yield return StartCoroutine(Attack());

            Debug.Log($"end of while loop in run entity");
        }
    }

    IEnumerator Attack()
    {
        Debug.Log($"in attack");

        while (_currentAttackCooldown > 0)
            yield return null;

        Debug.Log($"After waited for attack cooldown");

        Vector3 punchRotation = new(45f, 0f, 0f);
        yield return _gfx.transform.DOPunchRotation(punchRotation, 0.6f, 0, 0).WaitForCompletion();
        _currentAttackCooldown = _attackCooldown;
        _enemy.GetHit(_power);

        Debug.Log($"After attack in attack");
    }


    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
    }

    public float GetTotalHealth() { return _health; }
    public float GetCurrentHealth() { return _currentHealth; }

    public void GetHit(float power)
    {
        _currentHealth -= power;
        OnHealthChanged?.Invoke(_currentHealth);
        Debug.Log($"{name} got hit for {power}, health left: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            StartCoroutine(Die());
            return;
        }
        transform.DOShakePosition(0.2f, 0.5f);

    }

    public IEnumerator Die()
    {
        _agent.enabled = false;
        IsDead = true;
        OnDeath?.Invoke();
        yield return new WaitForSeconds(0.2f);
        transform.DORotate(new Vector3(90, 0, 0), 0.5f).SetEase(Ease.OutBounce);
    }
}
