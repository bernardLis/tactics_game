using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
public class BattleEntity : MonoBehaviour
{
    [SerializeField] Sound _hurtSound;
    List<BattleEntity> _opponentList = new();

    public GameObject GFX;
    Material _gfxMaterial;

    const string _tweenHighlightId = "_tweenHighlightId";

    ArmyEntity _stats;
    float _currentHealth;

    BattleEntity _opponent;
    NavMeshAgent _agent;

    float _currentAttackCooldown;

    public int KilledEnemiesCount { get; private set; }

    bool _gettingHit;
    public bool IsDead { get; private set; }
    public bool IsGrounded;

    public event Action<float> OnHealthChanged;
    public event Action<BattleEntity> OnDeath;
    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
    }

    public void Initialize(ArmyEntity stats, ref List<BattleEntity> opponents)
    {
        IsGrounded = true;
        _stats = stats;
        _currentHealth = stats.Health;
        GFX.GetComponent<MeshRenderer>().material = stats.Material;
        _gfxMaterial = GFX.GetComponent<MeshRenderer>().material;

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = stats.Speed;
        _agent.stoppingDistance = stats.AttackRange;

        _opponentList = opponents;

        StartCoroutine(RunEntity());
    }


    IEnumerator RunEntity()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f)); // random delay at the beginning
        while (!IsDead && IsGrounded)
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
                if (IsDead)
                    break;
                _agent.destination = _opponent.transform.position;
                transform.LookAt(_opponent.transform);
                yield return null;
            }

            // reached destination
            _agent.enabled = false;

            // HERE: something smarter
            if (_stats.Projectile == null)
                yield return StartCoroutine(Attack());
            else
                yield return StartCoroutine(Shoot());
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
        if (Vector3.Distance(transform.position, _opponent.transform.position) > _stats.AttackRange + 0.5f) // +0.5 wiggle room
            yield break; // target ran away

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        Vector3 punchRotation = new(45f, 0f, 0f);
        yield return GFX.transform.DOPunchRotation(punchRotation, 0.6f, 0, 0).WaitForCompletion();
        _currentAttackCooldown = _stats.AttackCooldown;
        yield return _opponent.GetHit(_stats.Power, this);
    }

    IEnumerator Shoot()
    {
        while (_currentAttackCooldown > 0)
            yield return null;
        while (_gettingHit)
            yield return null;
        if (_opponent == null || _opponent.IsDead)
            yield break;
        if (Vector3.Distance(transform.position, _opponent.transform.position) > _stats.AttackRange)
            yield break; // target ran away

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        Vector3 punchRotation = new(45f, 0f, 0f);
        GFX.transform.DOPunchRotation(punchRotation, 0.6f, 0, 0).WaitForCompletion();
        _currentAttackCooldown = _stats.AttackCooldown;

        // spawn projectile
        GameObject projectileInstance = Instantiate(_stats.Projectile, GFX.transform.position, Quaternion.identity);
        projectileInstance.transform.LookAt(_opponent.transform);

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Shoot(this, _opponent, 20, _stats.Power);
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

    public IEnumerator GetHit(float power, BattleEntity attacker)
    {
        if (IsDead)
            yield break;

        _gettingHit = true;
        _currentHealth -= power;
        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            if (attacker != null)
                attacker.IncreaseKillCount();
            yield return Die();
            yield break;
        }
        //      if (Random.value > 0.5f)
        //         AudioManager.Instance.PlaySFX(_hurtSound, transform.position);
        yield return transform.DOShakePosition(0.2f, 0.5f).WaitForCompletion();
        _gettingHit = false;
    }

    public IEnumerator Die()
    {
        //  _agent.enabled = false;

        IsDead = true;
        OnDeath?.Invoke(this);
        yield return new WaitForSeconds(0.2f);
        transform.DORotate(new Vector3(-90, 0, 0), 0.5f).SetEase(Ease.OutBounce).WaitForCompletion();
        yield return transform.DOMoveY(0, 0.5f).SetEase(Ease.OutBounce).WaitForCompletion();
        ToggleHighlight(false);
        //yield return GFX.GetComponent<MeshRenderer>().material.DOFade(0, 2f).WaitForCompletion();
        // Destroy(gameObject);
    }

    public void IncreaseKillCount() { KilledEnemiesCount++; }

    public void ToggleHighlight(bool isOn)
    {
        if (IsDead) return;

        if (isOn)
        {
            _gfxMaterial.DOColor(Color.white, 0.2f).SetLoops(-1, LoopType.Yoyo).SetId(_tweenHighlightId);
        }
        else
        {
            DOTween.Kill(_tweenHighlightId);
            _gfxMaterial.color = _stats.Material.color;
        }

    }
}
