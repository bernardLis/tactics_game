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

public class BattleEntity : MonoBehaviour
{
    protected GameManager _gameManager;

    protected AudioManager _audioManager;
    protected BattleManager _battleManager;

    protected BattleHighlightDiamond _highlightDiamond;
    protected ObjectShaders _battleEntityShaders;

    public List<string> EntityLog = new();

    [Header("Sounds")]
    [SerializeField] Sound _spawnSound;
    [SerializeField] protected Sound _deathSound;

    [Header("Prefabs")]
    [SerializeField] GameObject _healedEffect;
    BattleTooltipManager _tooltipManager;

    public Collider Collider { get; private set; }

    public string BattleId { get; private set; }
    public int Team { get; private set; }
    protected GameObject _GFX;
    protected Material _material;
    Texture2D _emissionTexture;
    Color _defaultEmissionColor;
    [SerializeField] protected Disc _teamHighlightDisc;

    public Animator Animator { get; private set; }

    public Entity Entity { get; private set; }
    public IntVariable CurrentHealth { get; private set; }

    protected NavMeshAgent _agent;

    [HideInInspector] public bool IsShielded;
    bool _isPoisoned;

    bool _isGrabbed;
    public bool IsDead { get; private set; }
    bool _isDeathCoroutineStarted;

    protected bool _blockRunEntity;

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;

    public int DamageTaken { get; private set; }

    public event Action<int> OnDamageTaken;

    public event Action<BattleEntity, BattleEntity, Ability> OnDeath;
    void Awake()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    public virtual void InitializeEntity(Entity entity)
    {
        SetEntity(entity);

        _highlightDiamond = GetComponentInChildren<BattleHighlightDiamond>();
        _highlightDiamond.gameObject.SetActive(false);

        _battleEntityShaders = GetComponent<ObjectShaders>();

        _feelPlayer = GetComponent<MMF_Player>();

        Collider = GetComponent<Collider>();

        Animator = GetComponentInChildren<Animator>();
        _GFX = Animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");
        _defaultEmissionColor = Color.black;

        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;

        if (_spawnSound != null) _audioManager.PlaySFX(_spawnSound, transform.position);
        EntityLog.Add($"{Time.time}: Entity is spawned");

        SetStats();
    }

    public void SetEntity(Entity entity)
    {
        Entity = entity;
    }

    public virtual void SetStats()
    {
        _agent.speed = Entity.Speed;

        CurrentHealth = ScriptableObject.CreateInstance<IntVariable>();
        CurrentHealth.SetValue(Entity.GetMaxHealth());
    }

    public virtual void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        Team = team;

        _teamHighlightDisc.gameObject.SetActive(true);
        Color highlightColor = Color.black;
        if (Team == 0) highlightColor = GameManager.PlayerTeamColor;
        if (Team == 1) highlightColor = GameManager.OpponentTeamColor;
        highlightColor.a = 0.25f;
        _teamHighlightDisc.Color = highlightColor;

        EntityLog.Add($"{Time.time}: Entity is initialized, team: {team}");
        if (team == 1)
        {
            // HERE: enemy team is not highlighted
            _material.SetTexture("_EmissionMap", null);
            _material.SetColor("_EmissionColor", _defaultEmissionColor);
            _material.SetFloat("_Metallic", 0.5f);
        }

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
        if (_blockRunEntity) return;

        EntityLog.Add($"{Time.time}: Start run entity coroutine is called");
        if (_runEntityCoroutine != null) StopCoroutine(_runEntityCoroutine);

        _runEntityCoroutine = RunEntity();
        StartCoroutine(_runEntityCoroutine);
    }

    public virtual void StopRunEntityCoroutine()
    {
        EntityLog.Add($"{Time.time}: Stop run entity coroutine is called");

        if (_runEntityCoroutine != null)
            StopCoroutine(_runEntityCoroutine);

        if (_agent.isActiveAndEnabled) _agent.isStopped = true;
        _agent.enabled = false;
        Animator.SetBool("Move", false);
    }

    protected virtual IEnumerator RunEntity()
    {
        yield return null;
    }

    protected virtual IEnumerator PathToTarget()
    {
        yield return null;
    }

    public bool HasFullHealth() { return CurrentHealth.Value >= Entity.MaxHealth.Value; }
    public float GetTotalHealth() { return Entity.MaxHealth.Value; }
    public float GetCurrentHealth() { return CurrentHealth.Value; }

    public int GetHealed(Ability ability)
    {
        int value = ability.GetPower();
        GetHealed(value);
        return Mathf.RoundToInt(value);
    }

    public void GetHealed(int value)
    {
        EntityLog.Add($"{Time.time}: Entity gets healed by {value}");

        CurrentHealth.ApplyChange(value);
        if (CurrentHealth.Value > Entity.GetMaxHealth())
            CurrentHealth.SetValue(Entity.GetMaxHealth());

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

        BaseGetHit(Entity.CalculateDamage(ability), ability.Element.Color);

        if (CurrentHealth.Value <= 0)
        {
            ability.IncreaseKillCount();
            yield return Die(ability: ability); // start coroutine because I call stop all coroutines in base hit
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public virtual IEnumerator GetHit(BattleTurret turret)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {turret.name}");

        BaseGetHit(Entity.CalculateDamage(turret), turret.Turret.Element.Color);

        if (CurrentHealth.Value <= 0)
        {
            turret.Turret.IncreaseKillCount();
            yield return Die(); // start coroutine because I call stop all coroutines in base hit
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    public virtual IEnumerator GetHit(BattleCreature attacker, int specialDamage = 0)
    {
        if (IsDead) yield break;
        EntityLog.Add($"{Time.time}: Entity gets attacked by {attacker.name}");

        _audioManager.PlaySFX("Hit", transform.position);

        int damage = Entity.CalculateDamage(attacker);
        if (specialDamage > 0) damage = specialDamage;

        attacker.DealtDamage(damage);

        BaseGetHit(damage, attacker.Entity.Element.Color);
        EntityLog.Add($"{Time.time}: Current health is {CurrentHealth}");

        if (CurrentHealth.Value <= 0)
        {
            attacker.IncreaseKillCount();
            yield return Die(attacker: attacker);
            yield break;
        }

        if (!_isGrabbed) StartRunEntityCoroutine();
    }

    protected void BaseGetHit(int dmg, Color color)
    {
        StopRunEntityCoroutine();

        OnDamageTaken?.Invoke(dmg);
        DamageTaken += dmg;

        CurrentHealth.ApplyChange(-dmg);
        if (CurrentHealth.Value <= 0)
        {
            IsDead = true;
            CurrentHealth.SetValue(0);
        }

        DisplayFloatingText(dmg.ToString(), color);

        Animator.SetTrigger("Take Damage");
    }

    public void TriggerDieCoroutine()
    {
        StartCoroutine(Die());
    }

    public virtual IEnumerator Die(BattleEntity attacker = null, Ability ability = null, bool hasLoot = true)
    {
        if (_isDeathCoroutineStarted) yield break;
        _isDeathCoroutineStarted = true;
        if (_isGrabbed) BattleGrabManager.Instance.CancelGrabbing();
        if (_agent.isActiveAndEnabled) _agent.isStopped = true;

        if (_deathSound != null) _audioManager.PlaySFX(_deathSound, transform.position);

        DOTween.Kill(transform);

        if (hasLoot) ResolveLoot();

        EntityLog.Add($"{Time.time}: Entity dies.");

        OnDeath?.Invoke(this, attacker, ability);

        _teamHighlightDisc.gameObject.SetActive(false);

        Animator.SetTrigger("Die");

        TurnHighlightOff();
        //StopAllCoroutines(); <- this breaks bomb exploding
    }

    void ResolveLoot()
    {
        // HERE: Loot
        if (Team == 0) return;
        //    BattleLoot bp = Instantiate(_battleLootPrefab, transform.position, Quaternion.identity).GetComponent<BattleLoot>();
        //    bp.Initialize();
    }

    public IEnumerator GetPoisoned(BattleCreature attacker)
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
        TurnHighlightOff();
    }

    protected IEnumerator Celebrate()
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

    /* highlight */
    public void TurnHighlightOff()
    {
        HideHighlightDiamond();

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
        if (showDiamond) ShowHighlightDiamond(GetHighlightColor());

        _material.SetTexture("_EmissionMap", null);
        _material.SetColor("_EmissionColor", color);
    }

    public void ShowHighlightDiamond(Color color)
    {
        _highlightDiamond.Enable(color);
    }

    public void HideHighlightDiamond()
    {
        _highlightDiamond.Disable();
    }

    public Color GetHighlightColor()
    {
        if (Team == 0)
            return Color.blue;
        if (Team == 1)
            return Color.red;
        return Color.yellow;
    }
}
