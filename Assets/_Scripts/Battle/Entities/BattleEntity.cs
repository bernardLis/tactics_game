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
using UnityEngine.EventSystems;

public class BattleEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    BattleEntityTooltipManager _tooltipManager;
    public Collider Collider { get; private set; }

    public int Team { get; private set; }
    // bool _isPlayer;
    protected GameObject _GFX;
    Material _material;
    Texture2D _emissionTexture;
    public Animator Animator { get; private set; }

    List<BattleEntity> _opponentList = new();

    public ArmyEntity ArmyEntity { get; private set; }
    public float CurrentHealth { get; private set; }

    protected BattleEntity _opponent;
    protected NavMeshAgent _agent;

    protected float _currentAttackCooldown;
    protected float _currentSpecialAbilityCooldown;

    public int KilledEnemiesCount { get; private set; }
    public int DamageDealt { get; private set; }
    public int DamageTaken { get; private set; }

    bool _isGrabbed;
    public bool IsDead { get; private set; }

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;
    protected bool _hasSpecialMove;
    protected bool _hasSpecialAttack;
    IEnumerator _attackCoroutine;

    public event Action<float> OnHealthChanged;
    public event Action<int> OnEnemyKilled;
    public event Action<int> OnDamageDealt;
    public event Action<int> OnDamageTaken;

    public event Action<BattleEntity, BattleEntity, Ability> OnDeath;
    protected virtual void Start()
    {
        _currentAttackCooldown = 0;
        _currentSpecialAbilityCooldown = 0;

        _tooltipManager = BattleEntityTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();
    }

    protected virtual void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
        if (_currentSpecialAbilityCooldown >= 0)
            _currentSpecialAbilityCooldown -= Time.deltaTime;
    }

    public virtual void Initialize(int team, ArmyEntity armyEntity, ref List<BattleEntity> opponents)
    {
        Collider = GetComponent<Collider>();

        Animator = GetComponentInChildren<Animator>();
        _GFX = Animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");

        Team = team;
        if (team == 1)
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
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        StartRunEntityCoroutine();
    }

    public void StartRunEntityCoroutine()
    {
        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    public void StopRunEntityCoroutine()
    {
        StopAllCoroutines();
        _agent.enabled = false;
        Animator.SetBool("Move", false);
    }

    IEnumerator RunEntity()
    {
        if (_opponentList.Count == 0)
        {
            yield return Celebrate();
            yield break;
        }

        if (_opponent == null || _opponent.IsDead)
            ChooseNewTarget();
        yield return new WaitForSeconds(0.1f);

        yield return PathToTarget();

        _attackCoroutine = Attack();
        yield return _attackCoroutine;
    }

    protected virtual IEnumerator PathToTarget()
    {
        _agent.enabled = true;
        while (!_agent.SetDestination(_opponent.transform.position)) yield return null;
        Animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;

        // path to target
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            _agent.SetDestination(_opponent.transform.position);
            yield return null;
        }

        // reached destination
        Animator.SetBool("Move", false);
        _agent.enabled = false;
    }

    protected virtual IEnumerator Attack()
    {
        Debug.Log($"in base attack");
        // meant to be overwritten

        // it goes at the end... is that a good idea?
        _attackCoroutine = null;
        _currentAttackCooldown = ArmyEntity.AttackCooldown;
        StartRunEntityCoroutine();

        yield break;
    }

    protected virtual IEnumerator SpecialAbility()
    {
        // meant to be overwritten

        // it goes at the end... is that a good idea?
        _currentSpecialAbilityCooldown = ArmyEntity.SpecialAbilityCooldown;
        yield break;
    }

    protected bool CanAttack()
    {
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

        DisplayFloatingText(value.ToString(), ability.Element.Color);
        OnHealthChanged?.Invoke(CurrentHealth);

        return Mathf.RoundToInt(value);
    }

    public IEnumerator GetHit(BattleEntity attacker)
    {
        if (IsDead) yield break;

        int damage = ArmyEntity.CalculateDamage(attacker);
        attacker.DamageDealt += damage;
        attacker.OnDamageDealt?.Invoke(damage);

        yield return BaseGetHit(damage, attacker.ArmyEntity.Element.Color);

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
        if (IsDead) yield break;

        yield return BaseGetHit(ArmyEntity.CalculateDamage(ability), ability.Element.Color);

        if (CurrentHealth <= 0)
        {
            ability.IncreaseKillCount();
            yield return Die(ability: ability);
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public IEnumerator BaseGetHit(int dmg, Color color)
    {
        StopRunEntityCoroutine();

        OnDamageTaken?.Invoke(dmg);
        DamageTaken += dmg;

        CurrentHealth -= dmg;
        if (CurrentHealth <= 0)
        {
            IsDead = true;
            CurrentHealth = 0;
        }
        OnHealthChanged?.Invoke(CurrentHealth);

        DisplayFloatingText(dmg.ToString(), color);

        Animator.SetTrigger("Take Damage");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
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

        Dictionary<BattleEntity, float> closestBiased = new();

        // this number decides bias towards closer opponents
        float sum = 0;
        foreach (KeyValuePair<BattleEntity, float> entry in closest)
        {
            float value = 1 / entry.Value; // 2 / entry.value or 0.1 / entry.value to changed bias
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
                SetOpponent(entry.Key);
                return;
            }
            v -= entry.Value;
        }

        // should never get here...
        SetOpponent(_opponentList[Random.Range(0, _opponentList.Count)]);
    }

    public void SetOpponent(BattleEntity opponent)
    {
        _opponent = opponent;
    }

    IEnumerator Celebrate()
    {
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        yield return transform.DODynamicLookAt(Camera.main.transform.position, 0.2f).WaitForCompletion();
        Animator.SetBool("Celebrate", true);
    }

    public virtual IEnumerator Die(BattleEntity attacker = null, Ability ability = null)
    {
        StopRunEntityCoroutine();
        Animator.SetBool("Celebrate", false);

        Animator.SetTrigger("Die");
        OnDeath?.Invoke(this, attacker, ability);
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
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
        Animator.enabled = false;
    }

    public void Released()
    {
        _isGrabbed = false;
        Animator.enabled = true;
        StartRunEntityCoroutine();
    }

    /* weird helpers */
    public void DisplayFloatingText(string text, Color color)
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
        if (_emissionTexture != null && Team == 0)
        {
            _material.SetTexture("_EmissionMap", _emissionTexture);
            _material.SetColor("_EmissionColor", Color.black);
            return;
        }

        if (Team == 0)
            _material.SetColor("_EmissionColor", Color.black);
        else
            _material.SetColor("_EmissionColor", new Color(0.5f, 0.2f, 0.2f));
    }

    void TurnHighlightOn()
    {
        _material.SetTexture("_EmissionMap", null);

        if (Team == 0)
            _material.SetColor("_EmissionColor", Color.blue);
        else
            _material.SetColor("_EmissionColor", Color.red);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipManager.ShowInfo(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipManager.HideInfo();
    }

}
