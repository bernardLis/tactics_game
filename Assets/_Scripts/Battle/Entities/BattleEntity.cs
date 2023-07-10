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
    protected BattleManager _battleManager;

    protected BattleHighlightDiamond _highlightDiamond;

    public List<string> EntityLog = new();

    [Header("Sounds")]
    [SerializeField] Sound _spawnSound;
    [SerializeField] protected Sound _deathSound;

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

    public Entity Entity { get; private set; }
    public float CurrentHealth { get; private set; }

    protected NavMeshAgent _agent;

    [HideInInspector] public bool IsShielded;
    bool _isPoisoned;

    bool _isGrabbed;
    public bool IsDead { get; private set; }
    bool _isDeathCoroutineStarted;

    MMF_Player _feelPlayer;

    IEnumerator _runEntityCoroutine;

    public int DamageTaken { get; private set; }

    public event Action<float> OnHealthChanged;
    public event Action<int> OnDamageTaken;

    public event Action<BattleEntity, BattleEntity, Ability> OnDeath;

    void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    public virtual void InitializeEntity(Entity entity)
    {
        Entity = entity;
        _highlightDiamond = GetComponentInChildren<BattleHighlightDiamond>();
        _highlightDiamond.gameObject.SetActive(false);
        _feelPlayer = GetComponent<MMF_Player>();

        Collider = GetComponent<Collider>();

        Animator = GetComponentInChildren<Animator>();
        _GFX = Animator.gameObject;
        _material = _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material;
        _emissionTexture = _material.GetTexture("_EmissionMap") as Texture2D;
        _material.EnableKeyword("_EMISSION");
        _defaultEmissionColor = Color.black;

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = Entity.Speed;

        CurrentHealth = Entity.GetHealth();

        if (_spawnSound != null) _audioManager.PlaySFX(_spawnSound, transform.position);
        EntityLog.Add($"{Time.time}: Entity is spawned");
    }

    public virtual void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleEntityTooltipManager.Instance;

        Team = team;

        BattleId = team + "_" + Entity.name + "_" + Helpers.GetRandomNumber(4);
        name = BattleId;

        EntityLog.Add($"{Time.time}: Entity is initialized, team: {team}");
        if (team == 1)
        {
            // HERE: enemy team is not highlighted
            _material.SetTexture("_EmissionMap", null);
            _material.SetColor("_EmissionColor", _defaultEmissionColor);
            _material.SetFloat("_Metallic", 0.5f);
        }
    }

    public void StartRunEntityCoroutine()
    {
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

    public bool HasFullHealth() { return CurrentHealth >= Entity.GetHealth(); }
    public float GetTotalHealth() { return Entity.GetHealth(); }
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
        if (CurrentHealth > Entity.GetHealth())
            CurrentHealth = Entity.GetHealth();

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

        BaseGetHit(Entity.CalculateDamage(ability), ability.Element.Color);

        if (CurrentHealth <= 0)
        {
            ability.IncreaseKillCount();
            yield return Die(ability: ability); // start coroutine because I call stop all coroutines in base hit
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

        if (CurrentHealth <= 0)
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
            if (CurrentHealth > damageTick)
            {
                DisplayFloatingText(damageTick.ToString(), Color.green);
                attacker.DealtDamage(damageTick);
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

    /* tooltip */
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }
}
