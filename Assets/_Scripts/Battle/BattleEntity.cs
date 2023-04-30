using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class BattleEntity : MonoBehaviour
{

    public Collider Collider { get; private set; }

    GameObject _GFX;
    Material _material;
    Texture2D _emissionTexture;
    Animator _animator;

    List<BattleEntity> _opponentList = new();

    public ArmyEntity Stats { get; private set; }
    public float CurrentHealth { get; private set; }

    BattleEntity _opponent;
    NavMeshAgent _agent;

    float _currentAttackCooldown;

    public int KilledEnemiesCount { get; private set; }

    bool _isSpawned;
    bool _gettingHit;
    public bool IsDead { get; private set; }

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;

    public event Action<float> OnHealthChanged;
    public event Action<BattleEntity> OnDeath;

    void Start()
    {
        _feelPlayer = GetComponent<MMF_Player>();
    }

    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;

        /* ANIMATOR UPDATE */
        if (!_agent.isActiveAndEnabled || _agent.isStopped)
            _animator.SetBool("Move", false);
    }

    public void Initialize(bool isPlayer, ArmyEntity stats, ref List<BattleEntity> opponents)
    {
        Collider = GetComponent<Collider>();

        _animator = GetComponentInChildren<Animator>();
        _GFX = _animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");

        if (!isPlayer) _material.SetFloat("_Metallic", 0.5f);

        Stats = stats;
        CurrentHealth = stats.Health;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = stats.AttackRange;
        _agent.speed = stats.Speed;
        _agent.enabled = true;

        _opponentList = opponents;

        StartRunEntityCoroutine();
    }

    public void StopRunEntityCoroutine()
    {
        _agent.enabled = false;
        StopCoroutine(_runEntityCoroutine);
    }

    public void StartRunEntityCoroutine()
    {
        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    IEnumerator RunEntity()
    {
        if (!_isSpawned)
        {
            _isSpawned = true;
            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        }

        // HERE: something smarter
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
            yield return new WaitForSeconds(Random.Range(0.2f, 0.6f)); // otherwise agent does not move
            _animator.SetBool("Move", true);

            // path to target
            while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
            {
                if (_opponent == null) break;
                if (IsDead) break;
                _agent.destination = _opponent.transform.position;
                transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
                yield return null;
            }

            // reached destination
            _agent.enabled = false;
            if (Stats.Projectile == null)
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
        while (!CanAttack()) yield return null;
        if (!HasOpponentInRange()) yield break;

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        _animator.SetTrigger("Attack");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);

        _currentAttackCooldown = Stats.AttackCooldown;
        GameObject hitInstance = Instantiate(Stats.HitPrefab, _opponent.Collider.bounds.center, Quaternion.identity);

        yield return _opponent.GetHit(this);

        Destroy(hitInstance);
    }


    IEnumerator Shoot()
    {
        while (!CanAttack()) yield return null;
        if (!HasOpponentInRange()) yield break;

        transform.DODynamicLookAt(_opponent.transform.position, 0.2f);
        _animator.SetTrigger("Attack");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f);

        _currentAttackCooldown = Stats.AttackCooldown;
        // spawn projectile
        GameObject projectileInstance = Instantiate(Stats.Projectile, _GFX.transform.position, Quaternion.identity);
        projectileInstance.transform.LookAt(_opponent.transform);

        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        projectile.Shoot(this, _opponent, 20, Stats.Power);
    }

    bool CanAttack()
    {
        if (_gettingHit) return false;
        return _currentAttackCooldown < 0;
    }

    bool HasOpponentInRange()
    {
        if (_opponent == null) return false;
        if (_opponent.IsDead) return false;

        // +0.5 wiggle room
        return Vector3.Distance(transform.position, _opponent.transform.position) < Stats.AttackRange + 0.5f;
    }

    public float GetTotalHealth() { return Stats.Health; }
    public float GetCurrentHealth() { return CurrentHealth; }

    public int GetHealed(Ability ability)
    {
        float value = ability.GetPower();
        CurrentHealth += value;
        if (CurrentHealth > Stats.Health)
            CurrentHealth = Stats.Health;

        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = value.ToString();
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = GetDamageTextGradient(ability.Element.Color);
        _feelPlayer.PlayFeedbacks(transform.position);

        OnHealthChanged?.Invoke(CurrentHealth);

        return Mathf.RoundToInt(value);
    }

    public IEnumerator GetHit(BattleEntity attacker, Ability ability = null)
    {
        if (IsDead) yield break;
        StopRunEntityCoroutine();

        float dmg = 0;
        Color dmgColor = Color.white;
        if (ability != null)
        {
            dmg = Stats.CalculateDamage(ability);
            dmgColor = ability.Element.Color;
        }
        if (attacker != null)
        {
            dmg = Stats.CalculateDamage(attacker);
            dmgColor = attacker.Stats.Element.Color;
        }

        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = dmg.ToString();
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = GetDamageTextGradient(dmgColor);
        _feelPlayer.PlayFeedbacks(transform.position);

        _gettingHit = true;
        CurrentHealth -= dmg;
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            if (attacker != null) attacker.IncreaseKillCount();
            yield return Die(attacker, ability);
            yield break;
        }

        _animator.SetTrigger("Take Damage");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        _gettingHit = false;

        StartRunEntityCoroutine();
    }

    public Gradient GetDamageTextGradient(Color color)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.5f;
        colorKey[1].color = Color.white;
        colorKey[1].time = 1f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.5f;
        alphaKey[1].time = 1f;

        gradient.SetKeys(colorKey, alphaKey);

        return gradient;
    }

    void Celebrate()
    {
        transform.DODynamicLookAt(Camera.main.transform.position, 0.2f);
        _animator.SetBool("Celebrate", true);
    }

    public IEnumerator Die(BattleEntity attacker = null, Ability ability = null)
    {
        IsDead = true;
        _animator.SetTrigger("Die");
        OnDeath?.Invoke(this);
        yield return new WaitForSeconds(0.2f);
        ToggleHighlight(false);

        BattleLogManager logManager = BattleManager.Instance.GetComponent<BattleLogManager>();
        BattleLogEntityDeath log = ScriptableObject.CreateInstance<BattleLogEntityDeath>();
        log.Initialize(this, attacker, ability);
        logManager.AddLog(log);
    }

    public void IncreaseKillCount() { KilledEnemiesCount++; }

    public void ToggleHighlight(bool isOn)
    {
        if (!isOn)
        {
            if (_emissionTexture != null)
            {
                _material.SetTexture("_EmissionMap", _emissionTexture);
                return;
            }
            _material.SetColor("_EmissionColor", Color.black);
            return;
        }

        if (IsDead) return;

        _material.SetTexture("_EmissionMap", null);
        _material.SetColor("_EmissionColor", Color.white);

    }
}
