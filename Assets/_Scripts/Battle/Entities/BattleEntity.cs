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
using Shapes;

public class BattleEntity : MonoBehaviour, IGrabbable, IPointerDownHandler
{
    protected GameManager _gameManager;
    protected AudioManager _audioManager;
    protected BattleManager _battleManager;
    protected BattleGrabManager _grabManager;

    protected ObjectShaders _battleEntityShaders;

    public List<string> EntityLog = new();

    [Header("Sounds")]
    [SerializeField] Sound _spawnSound;
    [SerializeField] protected Sound _deathSound;
    [SerializeField] protected Sound _getHitSound;

    public Collider Collider { get; private set; }

    public string BattleId { get; private set; }
    public int Team { get; protected set; }

    protected GameObject _GFX;
    Rigidbody _rigidbody;
    public Animator Animator { get; private set; }

    public Entity Entity { get; private set; }

    protected NavMeshAgent _agent;
    protected Vector2Int _avoidancePriorityRange = new Vector2Int(20, 100);

    [HideInInspector] public bool IsShielded;
    protected bool _isEngaged;
    bool _isPoisoned;
    bool _isGrabbed;
    public bool IsDead { get; private set; }
    bool _isDeathCoroutineStarted;

    protected bool _blockRunEntity;

    MMF_Player _feelPlayer;

    protected IEnumerator _currentMainCoroutine;
    protected IEnumerator _currentSecondaryCoroutine;
    protected IEnumerator _currentAbilityCoroutine;

    public event Action<int> OnDamageTaken;
    public event Action<BattleEntity, EntityFight> OnDeath;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    public virtual void InitializeEntity(Entity entity, int team)
    {
        EntityLog.Add($"{Time.time}: (GAME TIME) Entity is spawned, team {team}");

        Entity = entity;
        Team = team;

        _battleEntityShaders = GetComponent<ObjectShaders>();

        Collider = GetComponent<Collider>();
        if (team == 0) Collider.excludeLayers = LayerMask.GetMask("Player");

        Animator = GetComponentInChildren<Animator>();
        _GFX = Animator.gameObject;
        _feelPlayer = GetComponent<MMF_Player>();
        _agent = GetComponent<NavMeshAgent>();
        // _agent.enabled = false; // HERE: testing

        if (_audioManager == null) _audioManager = AudioManager.Instance;
        if (_spawnSound != null) _audioManager.PlaySFX(_spawnSound, transform.position);

        SetStats();
    }

    public virtual void SetStats()
    {
        if (Entity is EntityMovement em)
        {
            _agent.speed = em.Speed.GetValue();
            em.Speed.OnValueChanged += (i) => _agent.speed = i;
        }
    }

    public virtual void InitializeBattle(ref List<BattleEntity> opponents)
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += () => StartCoroutine(Celebrate());
        _grabManager = BattleGrabManager.Instance;

        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null) _rigidbody.isKinematic = true;

        SetBattleId();

        EntityLog.Add($"{_battleManager.GetTime()}: Entity is initialized");
    }

    protected void SetBattleId()
    {
        BattleId = Team + "_" + Helpers.ParseScriptableObjectName(Entity.name)
                 + "_" + Helpers.GetRandomNumber(4);
        name = BattleId;
    }

    public void StartRunEntityCoroutine()
    {
        if (IsDead) return;

        StopRunEntityCoroutine();
        EntityLog.Add($"{_battleManager.GetTime()}: Start run entity coroutine is called");

        _currentMainCoroutine = RunEntity();
        StartCoroutine(_currentMainCoroutine);
    }

    bool CanStartRunEntity()
    {
        if (this == null) return false;
        if (_blockRunEntity) return false;
        if (!isActiveAndEnabled) return false;
        if (_isGrabbed) return false;
        if (IsDead) return false;

        return true;
    }

    public virtual void StopRunEntityCoroutine()
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Stop run entity coroutine is called");

        if (_currentMainCoroutine != null)
            StopCoroutine(_currentMainCoroutine);
        if (_currentSecondaryCoroutine != null)
            StopCoroutine(_currentSecondaryCoroutine);
        if (_currentAbilityCoroutine != null)
            StopCoroutine(_currentAbilityCoroutine);

        if (_agent.isActiveAndEnabled) _agent.isStopped = true;
        _agent.enabled = false;

        Animator.SetBool("Move", false);
    }

    protected virtual IEnumerator RunEntity()
    {
        yield return null;
    }

    protected virtual IEnumerator PathToPosition(Vector3 position)
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Path to position is called {position}");

        _agent.enabled = true;
        // TODO: pitiful solution for making entities push each other
        _agent.avoidancePriority = Random.Range(_avoidancePriorityRange.x, _avoidancePriorityRange.y);

        while (!_agent.SetDestination(position)) yield return null;
        while (_agent.pathPending) yield return null;

        Animator.SetBool("Move", true);
    }

    protected virtual IEnumerator PathToPositionAndStop(Vector3 position)
    {
        yield return PathToPosition(position);
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
            yield return new WaitForSeconds(0.1f);

        // reached destination
        StopWalking();
    }

    protected virtual IEnumerator PathToTarget(Transform transform)
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Path to target is called {transform}");

        yield return PathToPosition(transform.position);
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            if (transform == null) yield break;
            _agent.SetDestination(transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        // reached destination
        StopWalking();
    }

    void StopWalking()
    {
        _agent.avoidancePriority = 0;
        Animator.SetBool("Move", false);
        _agent.enabled = false;
    }

    public virtual void GetEngaged(BattleEntity engager)
    {
        if (_isEngaged) return;
        _isEngaged = true;

        EntityLog.Add($"{_battleManager.GetTime()}: Entity gets engaged by {engager.name}");
        StopRunEntityCoroutine();
        Invoke(nameof(Disengage), Random.Range(2f, 4f));
    }

    public virtual void Disengage()
    {
        if (IsDead) return;

        _isEngaged = false;
        EntityLog.Add($"{_battleManager.GetTime()}: Entity disengages");
        StartRunEntityCoroutine();
    }

    public bool HasFullHealth() { return Entity.CurrentHealth.Value >= Entity.MaxHealth.GetValue(); }

    public int GetHealed(Ability ability)
    {
        int value = ability.GetPower();
        GetHealed(value);
        return value;
    }

    public void GetHealed(int value)
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity gets healed by {value}");

        int v = Mathf.Clamp(value, 0, Entity.MaxHealth.GetValue() - Entity.CurrentHealth.Value);
        Entity.CurrentHealth.ApplyChange(v);

        DisplayFloatingText("+" + v, Color.green);
    }

    public virtual IEnumerator GetHit(Ability ability)
    {
        if (IsDead) yield break;
        if (_battleManager == null) yield break;
        EntityLog.Add($"{_battleManager.GetTime()}: Entity gets attacked by {ability.name}");

        BaseGetHit(Entity.CalculateDamage(ability), ability.Element.Color.Color);

        if (Entity.CurrentHealth.Value <= 0)
            ability.IncreaseKillCount();
    }

    public virtual IEnumerator GetHit(EntityFight attacker, int specialDamage = 0)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{_battleManager.GetTime()}: Entity gets attacked by {attacker.name}");

        int damage = Entity.CalculateDamage(attacker);
        if (specialDamage > 0) damage = specialDamage;

        attacker.AddDmgDealt(damage);

        BaseGetHit(damage, attacker.Element.Color.Color, attacker);

        if (Entity.CurrentHealth.Value <= 0)
            attacker.AddKill(Entity);
    }

    public virtual void BaseGetHit(int dmg, Color color, EntityFight attacker = null)
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity takes damage {dmg}");
        StopRunEntityCoroutine();

        if (_getHitSound != null) _audioManager.PlaySFX(_getHitSound, transform.position);
        else _audioManager.PlaySFX("Hit", transform.position);

        Animator.SetTrigger("Take Damage");
        DisplayFloatingText(dmg.ToString(), color);

        OnDamageTaken?.Invoke(dmg);

        int d = Mathf.Clamp(dmg, 0, Entity.CurrentHealth.Value);
        Entity.CurrentHealth.ApplyChange(-d);
        if (Entity.CurrentHealth.Value <= 0)
        {
            TriggerDieCoroutine(attacker, true);
            return;
        }

        StartRunEntityCoroutine();
    }

    public void TriggerDieCoroutine(EntityFight attacker = null, bool hasGrave = false)
    {
        IsDead = true;
        StartCoroutine(Die(attacker: attacker, hasGrave: hasGrave));
    }

    public virtual IEnumerator Die(EntityFight attacker = null, bool givesExp = true, bool hasGrave = true)
    {
        if (_isDeathCoroutineStarted) yield break;
        _isDeathCoroutineStarted = true;

        StopRunEntityCoroutine();

        if (_isGrabbed) BattleGrabManager.Instance.CancelGrabbing();
        if (_agent.isActiveAndEnabled) _agent.isStopped = true;

        Collider.enabled = false;

        if (_deathSound != null) _audioManager.PlaySFX(_deathSound, transform.position);
        DOTween.Kill(transform);
        if (givesExp) ResolveLoot();

        EntityLog.Add($"{_battleManager.GetTime()}: Entity dies.");
        OnDeath?.Invoke(this, attacker);
        Animator.SetTrigger("Die");
        transform.DOMoveY(-1, 10f)
                .SetDelay(3f)
                .OnComplete(() =>
                {
                    transform.DOKill();
                    Destroy(gameObject);
                });

        //StopAllCoroutines(); <- this breaks bomb exploding
    }

    void ResolveLoot()
    {
        if (Team == 0) return;

        ExperienceOrb expOrb = Entity.GetExpOrb();
        if (expOrb == null) return;

        BattleExperienceOrb bl = Instantiate(expOrb.Prefab, transform.position, Quaternion.identity)
                                .GetComponent<BattleExperienceOrb>();
        bl.Initialize(expOrb);
    }

    public IEnumerator GetPoisoned(BattleCreature attacker)
    {
        if (_isPoisoned) yield break;
        if (IsDead) yield break;
        EntityLog.Add($"{_battleManager.GetTime()}: Entity gets poisoned by {attacker.name}.");

        _isPoisoned = true;
        DisplayFloatingText("Poisoned", Color.green);

        // TODO: for now hardcoded
        int totalDamage = 20;
        int damageTick = 5;

        while (totalDamage > 0)
        {
            // poison can't kill
            if (Entity.CurrentHealth.Value > damageTick)
            {
                DisplayFloatingText(damageTick.ToString(), Color.green);
                attacker.DealtDamage(damageTick);
                OnDamageTaken?.Invoke(damageTick);
                Entity.CurrentHealth.ApplyChange(-damageTick);
            }
            totalDamage -= damageTick;

            yield return new WaitForSeconds(1f);
        }

        _isPoisoned = false;
    }

    protected IEnumerator Celebrate()
    {
        if (IsDead) yield break;

        StopRunEntityCoroutine();

        yield return transform.DODynamicLookAt(Camera.main.transform.position, 0.2f).WaitForCompletion();
        Animator.SetBool("Celebrate", true);
    }

    public void SetDead() { IsDead = true; }

    /* grab */
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _audioManager.PlaySFX(_spawnSound, transform.position);

        if (!CanBeGrabbed()) return;

        _grabManager.TryGrabbing(gameObject);
    }

    public virtual bool CanBeGrabbed()
    {
        if (IsDead) return false;
        if (_grabManager == null) return false;
        return true;
    }

    public virtual void Grabbed()
    {
        _isGrabbed = true;
        Animator.enabled = false;
        StopRunEntityCoroutine();
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
        _feelPlayer.PlayFeedbacks(transform.position + transform.localScale.y * Vector3.up);
    }
}
