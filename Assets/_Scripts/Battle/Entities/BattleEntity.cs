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
    protected AudioManager _audioManager;
    BattleManager _battleManager;

    BattleHighlightDiamond _highlightDiamond;

    public List<string> EntityLog = new();

    [Header("Sounds")]
    [SerializeField] Sound _spawnSound;
    [SerializeField] protected Sound _deathSound;
    [SerializeField] protected Sound _attackSound;
    [SerializeField] protected Sound _specialAbilitySound;

    [Header("Prefabs")]
    [SerializeField] GameObject _battlePickupPrefab;
    [SerializeField] GameObject _healedEffect;
    BattleEntityTooltipManager _tooltipManager;

    public Collider Collider { get; private set; }

    public string BattleId { get; private set; }
    public int Team { get; private set; }
    protected GameObject _GFX;
    Material _material;
    Texture2D _emissionTexture;
    Color _defaultEmissionColor;
    public Animator Animator { get; private set; }

    List<BattleEntity> _opponentList = new();

    public Creature Creature { get; private set; }
    public float CurrentHealth { get; private set; }

    protected BattleEntity _opponent;
    protected NavMeshAgent _agent;

    protected float _currentAttackCooldown;
    public float CurrentSpecialAbilityCooldown { get; private set; }

    [HideInInspector] public bool IsShielded;
    bool _isPoisoned;

    public int KilledEnemiesCount { get; private set; }
    public int DamageDealt { get; private set; }
    public int DamageTaken { get; private set; }

    bool _isGrabbed;
    public bool IsDead { get; private set; }
    bool _isDeathCoroutineStarted;

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;

    protected bool _hasSpecialAction; // e.g. Shell's shield, can be fired at "any time"
    protected bool _hasSpecialMove;
    protected bool _hasSpecialAttack;
    IEnumerator _attackCoroutine;

    public event Action<float> OnHealthChanged;
    public event Action<int> OnEnemyKilled;
    public event Action<int> OnDamageDealt;
    public event Action<int> OnDamageTaken;

    public event Action<BattleEntity, BattleEntity, Ability> OnDeath;

    void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    protected virtual void Start()
    {
        _battleManager = BattleManager.Instance;

        _currentAttackCooldown = 0;
        CurrentSpecialAbilityCooldown = 0;


        _tooltipManager = BattleEntityTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();
    }

    protected virtual void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
        if (CurrentSpecialAbilityCooldown >= 0)
            CurrentSpecialAbilityCooldown -= Time.deltaTime;
    }

    public virtual void SpawnCreature(Creature creature)
    {
        Creature = creature;
        OnEnemyKilled += creature.AddKill;
        OnDamageDealt += creature.AddDmgDealt;
        OnDamageTaken += creature.AddDmgTaken;

        _highlightDiamond = GetComponentInChildren<BattleHighlightDiamond>();
        _highlightDiamond.gameObject.SetActive(false);

        Collider = GetComponent<Collider>();

        Animator = GetComponentInChildren<Animator>();
        _GFX = Animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");
        _defaultEmissionColor = Color.black;

        _agent = GetComponent<NavMeshAgent>();
        _agent.stoppingDistance = Creature.AttackRange;
        _agent.speed = Creature.Speed;

        CurrentHealth = Creature.GetHealth();

        if (_spawnSound != null) _audioManager.PlaySFX(_spawnSound, transform.position);
        EntityLog.Add($"{Time.time}: Entity is spawned");
    }

    public virtual void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        Team = team;
        _opponentList = opponents;

        BattleId = team + "_" + Creature.name + "_" + Helpers.GetRandomNumber(4);
        name = BattleId;

        EntityLog.Add($"{Time.time}: Entity is initialized, team: {team}");
        if (team == 1)
        {
            _defaultEmissionColor = new Color(0.5f, 0.2f, 0.2f);
            _material.SetTexture("_EmissionMap", null);
            _material.SetColor("_EmissionColor", _defaultEmissionColor);
            _material.SetFloat("_Metallic", 0.5f);
        }

        StartRunEntityCoroutine();
    }

    public void StartRunEntityCoroutine()
    {
        EntityLog.Add($"{Time.time}: Start run entity coroutine is called");
        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    public void StopRunEntityCoroutine()
    {
        EntityLog.Add($"{Time.time}: Stop run entity coroutine is called");

        if (_runEntityCoroutine != null)
            StopCoroutine(_runEntityCoroutine);
        if (_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
        _agent.enabled = false;
        Animator.SetBool("Move", false);
    }

    IEnumerator RunEntity()
    {
        if (IsDead) yield break;

        while (_opponentList.Count == 0)
        {
            if (IsDead) yield break;
            if (_battleManager.BattleFinalized)
            {
                yield return Celebrate();
                yield break;
            }
            yield return new WaitForSeconds(0.5f); // TODO: lag
        }

        if (_hasSpecialAction && CurrentSpecialAbilityCooldown <= 0)
            yield return SpecialAbility();

        if (_opponent == null || _opponent.IsDead)
            ChooseNewTarget();
        yield return new WaitForSeconds(0.1f);

        yield return PathToTarget();

        _attackCoroutine = Attack();
        yield return _attackCoroutine;
    }

    protected virtual IEnumerator PathToTarget()
    {
        EntityLog.Add($"{Time.time}: Path to target is called");

        if (_hasSpecialMove && CurrentSpecialAbilityCooldown <= 0)
        {
            yield return SpecialAbility();
            PathToTarget();
            yield break;
        }

        _agent.enabled = true;
        _agent.avoidancePriority = Random.Range(1, 100);

        while (!_agent.SetDestination(_opponent.transform.position)) yield return null;
        Animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;

        // path to target
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            if (_hasSpecialAction && CurrentSpecialAbilityCooldown <= 0)
                yield return SpecialAbility();

            _agent.SetDestination(_opponent.transform.position);
            yield return null;
        }

        // reached destination
        _agent.avoidancePriority = 0;
        Animator.SetBool("Move", false);
        _agent.enabled = false;
    }

    protected virtual IEnumerator Attack()
    {
        EntityLog.Add($"{Time.time}: Entity attacked {_opponent.name}");

        // meant to be overwritten

        // it goes at the end... is that a good idea?
        _attackCoroutine = null;
        _currentAttackCooldown = Creature.AttackCooldown;
        StartRunEntityCoroutine();

        yield break;
    }

    protected virtual IEnumerator SpecialAbility()
    {
        EntityLog.Add($"{Time.time}: Entity used special ability");

        // meant to be overwritten

        // it goes at the end... is that a good idea?
        Creature.CreatureAbility.Used();
        CurrentSpecialAbilityCooldown = Creature.CreatureAbility.Cooldown;
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
        return Vector3.Distance(transform.position, _opponent.transform.position) < Creature.AttackRange + 0.5f;
    }

    public bool HasFullHealth() { return CurrentHealth >= Creature.GetHealth(); }
    public float GetTotalHealth() { return Creature.GetHealth(); }
    public float GetCurrentHealth() { return CurrentHealth; }

    public int GetHealed(Ability ability)
    {
        int value = ability.GetPower();
        GetHealed(value);
        return Mathf.RoundToInt(value);
    }

    public void GetHealed(int value)
    {
        EntityLog.Add($"{Time.time}: Entity gets healed by {value}");

        CurrentHealth += value;
        if (CurrentHealth > Creature.GetHealth())
            CurrentHealth = Creature.GetHealth();

        OnHealthChanged?.Invoke(CurrentHealth);
        DisplayFloatingText("+" + value, Color.green);

        GameObject obj = Instantiate<GameObject>(_healedEffect, transform.position, Quaternion.identity);
        obj.transform.parent = _GFX.transform;
        obj.transform.DOScale(0, 0.5f)
                .SetDelay(2f)
                .OnComplete(() => Destroy(obj));
    }

    public virtual IEnumerator GetHit(Ability ability)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {ability.name}");

        BaseGetHit(Creature.CalculateDamage(ability), ability.Element.Color);

        if (CurrentHealth <= 0)
        {
            ability.IncreaseKillCount();
            yield return Die(ability: ability); // start coroutine because I call stop all coroutines in base hit
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public virtual IEnumerator GetHit(BattleEntity attacker, int specialDamage = 0)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {attacker.name}");

        _audioManager.PlaySFX("Hit", transform.position);

        int damage = Creature.CalculateDamage(attacker);
        if (specialDamage > 0) damage = specialDamage;

        attacker.DamageDealt += damage;
        attacker.OnDamageDealt?.Invoke(damage);

        BaseGetHit(damage, attacker.Creature.Element.Color);
        EntityLog.Add($"{Time.time}: Current health is {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            attacker.IncreaseKillCount();
            yield return Die(attacker: attacker);
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    void BaseGetHit(int dmg, Color color)
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
    }

    public virtual IEnumerator Die(BattleEntity attacker = null, Ability ability = null)
    {
        if (_isDeathCoroutineStarted) yield break;
        _isDeathCoroutineStarted = true;

        if (_deathSound != null) _audioManager.PlaySFX(_deathSound, transform.position);

        DOTween.Kill(transform);
        if (Team != 0)
        {
            BattlePickup bp = Instantiate(_battlePickupPrefab, transform.position, Quaternion.identity).GetComponent<BattlePickup>();
            bp.Initialize();
        }

        EntityLog.Add($"{Time.time}: Entity dies.");

        OnDeath?.Invoke(this, attacker, ability);

        Animator.SetTrigger("Die");

        TurnHighlightOff();
        //StopAllCoroutines(); <- this breaks bomb exploding
    }

    public IEnumerator GetPoisoned(BattleEntity attacker)
    {
        if (_isPoisoned) yield break;
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets poisoned by {attacker.name}.");

        _isPoisoned = true;
        DisplayFloatingText("Poisoned", Color.green);
        TurnHighlightOn(Color.green, false);

        // TODO: for now hardcoded
        int totalDamage = 20;
        int damageTick = 5;

        while (totalDamage > 0)
        {
            // poison can't kill
            if (CurrentHealth > damageTick)
            {
                DisplayFloatingText(damageTick.ToString(), Color.green);
                attacker.DamageDealt += damageTick;
                attacker.OnDamageDealt?.Invoke(damageTick);
                OnDamageTaken?.Invoke(damageTick);
                DamageTaken += damageTick;
                CurrentHealth -= damageTick;
                OnHealthChanged?.Invoke(CurrentHealth);
            }
            totalDamage -= damageTick;

            yield return new WaitForSeconds(1f);
        }

        _isPoisoned = false;
        TurnHighlightOff();
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

    public void SetOpponent(BattleEntity opponent) { _opponent = opponent; }

    IEnumerator Celebrate()
    {
        if (IsDead) yield break;

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        yield return transform.DODynamicLookAt(Camera.main.transform.position, 0.2f).WaitForCompletion();
        Animator.SetBool("Celebrate", true);
    }

    public void SetDead() { IsDead = true; }

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
        if (_feelPlayer == null) return;
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

    /* highlight */
    public void TurnHighlightOff()
    {
        _highlightDiamond.Disable();

        if (_emissionTexture != null && Team == 0)
        {
            _material.SetTexture("_EmissionMap", _emissionTexture);
            _material.SetColor("_EmissionColor", Color.black);
            return;
        }

        _material.SetColor("_EmissionColor", _defaultEmissionColor);
    }

    public void TurnHighlightOn(Color color, bool showDiamond = true)
    {
        if (IsDead) return;
        if (showDiamond) _highlightDiamond.Enable(GetHighlightColor());

        _material.SetTexture("_EmissionMap", null);
        _material.SetColor("_EmissionColor", color);
    }

    public Color GetHighlightColor()
    {
        if (Team == 0)
            return Color.blue;
        if (Team == 1)
            return Color.red;
        return Color.yellow;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipManager.ShowInfo(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipManager.HideInfo();
    }


#if UNITY_EDITOR
    [ContextMenu("Trigger Special Ability")]
    public void TriggerSpecialAction()
    {
        StartCoroutine(SpecialAbility());

    }
#endif


}
