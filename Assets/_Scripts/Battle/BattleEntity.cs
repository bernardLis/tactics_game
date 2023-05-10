using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using System.Linq;

public class BattleEntity : MonoBehaviour
{
    public Collider Collider { get; private set; }

    bool _isPlayer;
    protected GameObject _GFX;
    Material _material;
    Texture2D _emissionTexture;
    protected Animator _animator;

    List<BattleEntity> _opponentList = new();

    public ArmyEntity ArmyEntity { get; private set; }
    public float CurrentHealth { get; private set; }

    protected BattleEntity _opponent;
    NavMeshAgent _agent;

    protected float _currentAttackCooldown;

    public int KilledEnemiesCount { get; private set; }

    bool _stopRunEntityInWhileLoop;
    bool _gettingHit;
    bool _isGrabbed;
    public bool IsDead { get; private set; }

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;
    IEnumerator _attackCoroutine;

    public event Action<float> OnHealthChanged;
    public event Action<int> OnEnemyKilled;
    public event Action<BattleEntity, BattleEntity, Ability> OnDeath;

    void Start()
    {
        _feelPlayer = GetComponent<MMF_Player>();
    }

    void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
    }

    public void Initialize(bool isPlayer, ArmyEntity armyEntity, ref List<BattleEntity> opponents)
    {
        Collider = GetComponent<Collider>();

        _animator = GetComponentInChildren<Animator>();
        _GFX = _animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");

        _isPlayer = isPlayer;
        if (!isPlayer)
        {
            _material.SetTexture("_EmissionMap", null);
            _material.SetColor("_EmissionColor", new Color(0.5f, 0.2f, 0.2f));
            _material.SetFloat("_Metallic", 0.5f);
        }

        ArmyEntity = armyEntity;
        CurrentHealth = armyEntity.Health;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = armyEntity.AttackRange;
        _agent.speed = armyEntity.Speed;

        _opponentList = opponents;

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        // spawn animation should be playing play
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        StartRunEntityCoroutine();
    }

    public void StartRunEntityCoroutine()
    {
        Debug.Log($"start");
        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    public void StopRunEntityCoroutine()
    {
        Debug.Log($"stop");
        _agent.enabled = false;
        _animator.SetBool("Move", false);
        _stopRunEntityInWhileLoop = true;

        if (_runEntityCoroutine != null)
            StopCoroutine(_runEntityCoroutine);
        _runEntityCoroutine = null;
    }

    IEnumerator RunEntity()
    {
        _stopRunEntityInWhileLoop = false;
        Debug.Log($"run entity started");

        if (_opponentList.Count == 0)
        {
            yield return Celebrate();
            yield break;
        }

        if (_opponent == null || _opponent.IsDead)
            ChooseNewTarget();

        _agent.enabled = true;
        while (!_agent.SetDestination(_opponent.transform.position)) yield return null;
        _animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;

        // path to target
        while (_agent.remainingDistance > _agent.stoppingDistance)
        {
            if (_stopRunEntityInWhileLoop) yield break;

            _agent.SetDestination(_opponent.transform.position);
            yield return null;
        }

        // reached destination
        _animator.SetBool("Move", false);
        _agent.enabled = false;

        Debug.Log($"before attack coroutine");
        _attackCoroutine = Attack();
        yield return _attackCoroutine;
        Debug.Log($"end of run entity");
    }

    protected virtual IEnumerator Attack()
    {
        // meant to be overwritten

        // it goes at the end... is that a good idea?
        _attackCoroutine = null;
        _currentAttackCooldown = ArmyEntity.AttackCooldown;
        StartRunEntityCoroutine();

        yield break;
    }

    protected bool CanAttack()
    {
        if (_gettingHit) return false;
        return _currentAttackCooldown < 0;
    }

    protected bool IsOpponentInRange()
    {
        if (_opponent == null) return false;
        if (_opponent.IsDead) return false;

        // +0.5 wiggle room
        return Vector3.Distance(transform.position, _opponent.transform.position) < ArmyEntity.AttackRange + 0.5f;
    }

    public float GetTotalHealth() { return ArmyEntity.Health; }
    public float GetCurrentHealth() { return CurrentHealth; }

    public int GetHealed(Ability ability)
    {
        float value = ability.GetPower();
        CurrentHealth += value;
        if (CurrentHealth > ArmyEntity.Health)
            CurrentHealth = ArmyEntity.Health;

        DisplayDamageText(value.ToString(), ability.Element.Color);
        OnHealthChanged?.Invoke(CurrentHealth);

        return Mathf.RoundToInt(value);
    }

    public IEnumerator GetHit(BattleEntity attacker)
    {
        Debug.Log($"get hit attacker");
        if (IsDead) yield break;

        yield return BaseGetHit(ArmyEntity.CalculateDamage(attacker), attacker.ArmyEntity.Element.Color);

        if (CurrentHealth <= 0)
        {
            attacker.IncreaseKillCount();
            yield return Die(attacker: attacker);
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public IEnumerator GetHit(Ability ability)
    {
        Debug.Log($"get hit ability");
        if (IsDead) yield break;

        yield return BaseGetHit(ArmyEntity.CalculateDamage(ability), ability.Element.Color);

        if (CurrentHealth <= 0)
        {
            yield return Die(ability: ability);
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public IEnumerator BaseGetHit(float dmg, Color color)
    {
        StopRunEntityCoroutine();

        _gettingHit = true;
        CurrentHealth -= dmg;
        OnHealthChanged?.Invoke(CurrentHealth);

        DisplayDamageText(dmg.ToString(), color);

        _animator.SetTrigger("Take Damage");
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        _gettingHit = false;
    }

    void ChooseNewTarget()
    {
        // choose a random opponent with a bias towards closer opponents
        Dictionary<BattleEntity, float> distances = new();
        foreach (BattleEntity be in _opponentList)
        {
            if (be.IsDead) continue;
            float distance = Vector3.Distance(transform.position, be.transform.position);
            distances.Add(be, distance);
        }

        if (distances.Count == 0) return;

        var closest = distances.OrderByDescending(pair => pair.Value).Reverse().Take(10);
        float v = Random.value;

        //https://stats.stackexchange.com/questions/277298/create-a-higher-probability-to-smaller-values
        Dictionary<BattleEntity, float> closestBiased = new();
        // this number decides bias towards closer opponents
        float e = 0.91f; // range 0.9 - 0.99 I think
        float sum = 0;
        foreach (KeyValuePair<BattleEntity, float> entry in closest)
        {
            float value = Mathf.Pow(e, entry.Value);
            closestBiased.Add(entry.Key, value);
            sum += value;
        }

        Dictionary<BattleEntity, float> closestNormalized = new();
        foreach (KeyValuePair<BattleEntity, float> entry in closestBiased)
            closestNormalized.Add(entry.Key, entry.Value / sum);

        foreach (KeyValuePair<BattleEntity, float> entry in closestNormalized)
        {
            if (v < entry.Value)
            {
                _opponent = entry.Key;
                return;
            }
            v -= entry.Value;
        }

        // should never get here...
        _opponent = _opponentList[Random.Range(0, _opponentList.Count)];
    }


    IEnumerator Celebrate()
    {
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        yield return transform.DODynamicLookAt(Camera.main.transform.position, 0.2f).WaitForCompletion();
        _animator.SetBool("Celebrate", true);
    }

    public IEnumerator Die(BattleEntity attacker = null, Ability ability = null)
    {
        StopRunEntityCoroutine();
        _animator.SetBool("Celebrate", false);

        IsDead = true;
        _animator.SetTrigger("Die");
        OnDeath?.Invoke(this, attacker, ability);
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        ToggleHighlight(false);
    }

    /* grab */
    public bool CanBeGrabbed()
    {
        if (IsDead) return false;
        return true;
    }

    public void Grabbed()
    {
        _opponent = null;
        _isGrabbed = true;
        StopRunEntityCoroutine();
        _animator.enabled = false;
    }

    public void Released()
    {
        _isGrabbed = false;
        _animator.enabled = true;
        StartRunEntityCoroutine();
    }

    /* weird helpers */
    void DisplayDamageText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        _feelPlayer.PlayFeedbacks(transform.position);
    }

    public void IncreaseKillCount()
    {
        KilledEnemiesCount++;
        OnEnemyKilled?.Invoke(KilledEnemiesCount);
    }

    /* shit highlight */
    public void ToggleHighlight(bool isOn)
    {
        if (!isOn)
        {
            TurnHighlightOff();
            return;
        }

        if (IsDead) return;
        TurnHighlightOn();
    }

    void TurnHighlightOff()
    {
        if (_emissionTexture != null && _isPlayer)
        {
            _material.SetTexture("_EmissionMap", _emissionTexture);
            _material.SetColor("_EmissionColor", Color.black);
            return;
        }

        if (_isPlayer)
            _material.SetColor("_EmissionColor", Color.black);
        else
            _material.SetColor("_EmissionColor", new Color(0.5f, 0.2f, 0.2f));
    }

    void TurnHighlightOn()
    {
        _material.SetTexture("_EmissionMap", null);

        if (_isPlayer)
            _material.SetColor("_EmissionColor", Color.blue);
        else
            _material.SetColor("_EmissionColor", Color.red);
    }
}
