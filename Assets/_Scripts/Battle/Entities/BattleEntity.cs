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

    [Header("Prefabs")]
    [SerializeField] GameObject _healedEffect;

    public Collider Collider { get; private set; }

    public string BattleId { get; private set; }
    public int Team { get; private set; }
    protected GameObject _GFX;

    protected BattleEntityHighlight _battleEntityHighlight;

    public Animator Animator { get; private set; }

    public Entity Entity { get; private set; }
    public IntVariable CurrentHealth { get; private set; }

    protected NavMeshAgent _agent;

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

    public int DamageTaken { get; private set; }

    public event Action<int> OnDamageTaken;
    public event Action<BattleEntity, GameObject> OnDeath;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    public virtual void InitializeEntity(Entity entity)
    {
        EntityLog.Add($"{Time.time}: Entity is spawned");

        Entity = entity;

        _battleEntityShaders = GetComponent<ObjectShaders>();
        _feelPlayer = GetComponent<MMF_Player>();
        Collider = GetComponent<Collider>();
        Animator = GetComponentInChildren<Animator>();
        _GFX = Animator.gameObject;
        _battleEntityHighlight = GetComponent<BattleEntityHighlight>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;

        if (_spawnSound != null) _audioManager.PlaySFX(_spawnSound, transform.position);

        SetStats();
    }

    public virtual void SetStats()
    {
        _agent.speed = Entity.Speed;

        CurrentHealth = ScriptableObject.CreateInstance<IntVariable>();
        CurrentHealth.SetValue(Entity.GetMaxHealth());
    }

    public virtual void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        EntityLog.Add($"{Time.time}: Entity is initialized, team: {team}");

        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += () => StartCoroutine(Celebrate());
        _grabManager = BattleGrabManager.Instance;

        GetComponent<Rigidbody>().isKinematic = true;

        Team = team;
        _battleEntityHighlight.Initialize(this);
        SetBattleId();
    }

    protected void SetBattleId()
    {
        BattleId = Team + "_" + Helpers.ParseScriptableObjectName(Entity.name)
                 + "_" + Helpers.GetRandomNumber(4);
        name = BattleId;
    }

    public void StartRunEntityCoroutine()
    {
        if (!CanStartRunEntity())
        {
            Invoke(nameof(StartRunEntityCoroutine), Random.Range(1f, 2f));
            return;
        }

        StopRunEntityCoroutine();
        EntityLog.Add($"{Time.time}: Start run entity coroutine is called");

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
        EntityLog.Add($"{Time.time}: Stop run entity coroutine is called");

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
        EntityLog.Add($"{Time.time}: Path to position is called {position}");

        _agent.enabled = true;
        _agent.avoidancePriority = Random.Range(1, 100);

        while (!_agent.SetDestination(position)) yield return null;
        while (_agent.pathPending) yield return null;

        Animator.SetBool("Move", true);
    }

    public virtual void Engage(BattleEntity engager)
    {
        if (_isEngaged) return;
        _isEngaged = true;

        EntityLog.Add($"{Time.time}: Entity gets engaged by {engager.name}");
        StopRunEntityCoroutine();
        Invoke(nameof(Disengage), Random.Range(2f, 4f));
    }

    public virtual void Disengage()
    {
        _isEngaged = false;
        EntityLog.Add($"{Time.time}: Entity disengages");
        StartRunEntityCoroutine();
    }

    public bool HasFullHealth() { return CurrentHealth.Value >= Entity.MaxHealth.Value; }

    public int GetHealed(Ability ability)
    {
        int value = ability.GetPower();
        GetHealed(value);
        return value;
    }

    public void GetHealed(int value)
    {
        EntityLog.Add($"{Time.time}: Entity gets healed by {value}");

        int v = Mathf.Clamp(value, 0, Entity.GetMaxHealth() - CurrentHealth.Value);
        CurrentHealth.ApplyChange(v);

        DisplayFloatingText("+" + v, Color.green);
        _battleEntityHighlight.HealEffect();
    }

    public virtual IEnumerator GetHit(Ability ability)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {ability.name}");

        BaseGetHit(Entity.CalculateDamage(ability), ability.Element.Color);

        if (CurrentHealth.Value <= 0)
            ability.IncreaseKillCount();
    }

    public virtual IEnumerator GetHit(BattleTurret battleTurret)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {battleTurret.name}");

        BaseGetHit(Entity.CalculateDamage(battleTurret), battleTurret.Turret.Element.Color);

        if (CurrentHealth.Value <= 0)
            battleTurret.Turret.IncreaseKillCount();
    }

    public virtual IEnumerator GetHit(BattleCreature attacker, int specialDamage = 0)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {attacker.name}");

        int damage = Entity.CalculateDamage(attacker);
        if (specialDamage > 0) damage = specialDamage;

        attacker.DealtDamage(damage);

        BaseGetHit(damage, attacker.Entity.Element.Color, attacker.gameObject);

        if (CurrentHealth.Value <= 0)
            attacker.IncreaseKillCount();
    }

    protected void BaseGetHit(int dmg, Color color, GameObject attacker = null)
    {
        EntityLog.Add($"{Time.time}: Entity takes damage {dmg}");
        StopRunEntityCoroutine();
        _audioManager.PlaySFX("Hit", transform.position);
        Animator.SetTrigger("Take Damage");
        DisplayFloatingText(dmg.ToString(), color);

        OnDamageTaken?.Invoke(dmg);
        DamageTaken += dmg;

        int d = Mathf.Clamp(dmg, 0, CurrentHealth.Value);
        CurrentHealth.ApplyChange(-d);
        if (CurrentHealth.Value <= 0)
        {
            TriggerDieCoroutine(attacker);
            return;
        }

        StartRunEntityCoroutine();
    }

    public void TriggerDieCoroutine(GameObject attacker = null)
    {
        IsDead = true;
        StartCoroutine(Die(attacker: attacker));
    }

    public virtual IEnumerator Die(GameObject attacker = null, bool hasLoot = true)
    {
        if (_isDeathCoroutineStarted) yield break;
        _isDeathCoroutineStarted = true;

        StopRunEntityCoroutine();

        if (_isGrabbed) BattleGrabManager.Instance.CancelGrabbing();
        if (_agent.isActiveAndEnabled) _agent.isStopped = true;

        if (_deathSound != null) _audioManager.PlaySFX(_deathSound, transform.position);

        DOTween.Kill(transform);

        if (hasLoot) ResolveLoot();

        EntityLog.Add($"{Time.time}: Entity dies.");

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

        Loot loot = Entity.GetLoot();
        if (loot == null) return;

        BattleLoot bl = Instantiate(loot.Prefab, transform.position, Quaternion.identity).GetComponent<BattleLoot>();
        bl.Initialize(loot);
    }

    public IEnumerator GetPoisoned(BattleCreature attacker)
    {
        if (_isPoisoned) yield break;
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets poisoned by {attacker.name}.");

        _isPoisoned = true;
        DisplayFloatingText("Poisoned", Color.green);

        // TODO: for now hardcoded
        int totalDamage = 20;
        int damageTick = 5;

        while (totalDamage > 0)
        {
            // poison can't kill
            if (CurrentHealth.Value > damageTick)
            {
                DisplayFloatingText(damageTick.ToString(), Color.green);
                attacker.DealtDamage(damageTick);
                OnDamageTaken?.Invoke(damageTick);
                DamageTaken += damageTick;
                CurrentHealth.ApplyChange(-damageTick);
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

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        yield return transform.DODynamicLookAt(Camera.main.transform.position, 0.2f).WaitForCompletion();
        Animator.SetBool("Celebrate", true);
    }

    public void SetDead() { IsDead = true; }

    /* grab */
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!CanBeGrabbed()) return;

        _grabManager.TryGrabbing(gameObject);
    }

    public bool CanBeGrabbed()
    {
        if (IsDead) return false;
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
        _feelPlayer.PlayFeedbacks(transform.position);
    }

    public void TurnHighlightOn(Color c = default)
    {
        if (IsDead) return;
        if (_battleEntityHighlight == null) return;
        if (c == default) c = GetHighlightColor();

        _battleEntityHighlight.Highlight(c);
    }

    public void TurnHighlightOff()
    {
        if (_battleEntityHighlight == null) return;
        _battleEntityHighlight.ClearHighlight();
    }

    public Color GetHighlightColor()
    {
        if (Team == 0)
            return GameManager.PlayerTeamColor;
        if (Team == 1)
            return GameManager.OpponentTeamColor;
        return Color.yellow;
    }
}
