using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using DG.Tweening;

public class BattleCreature : BattleEntity
{
    [SerializeField] protected Sound _attackSound;
    [SerializeField] protected Sound _specialAbilitySound;

    [SerializeField] GameObject _gravePrefab;

    public Creature Creature { get; private set; }

    List<BattleEntity> _opponentList = new();

    public BattleEntity Opponent { get; private set; }

    protected float _currentAttackCooldown;
    public float CurrentSpecialAbilityCooldown { get; private set; }

    public int KilledEnemiesCount { get; private set; }
    public int DamageDealt { get; private set; }

    protected bool _hasSpecialAction; // e.g. Shell's shield, can be fired at "any time"
    protected bool _hasSpecialMove;
    protected bool _hasSpecialAttack;

    public event Action<int> OnEnemyKilled;
    public event Action<int> OnDamageDealt;
    public event Action<BattleCreature> OnEvolving;
    protected virtual void Start() { }

    protected virtual void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
        if (CurrentSpecialAbilityCooldown >= 0)
            CurrentSpecialAbilityCooldown -= Time.deltaTime;
    }

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Creature = (Creature)entity;
        Creature.OnLevelUp += OnLevelUp;

        OnEnemyKilled += Creature.AddKill;
        OnDamageDealt += Creature.AddDmgDealt;
        OnDamageTaken += Creature.AddDmgTaken;

        _agent.stoppingDistance = Creature.AttackRange;
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);
        _opponentList = opponents;

        _currentAttackCooldown = 0;
        CurrentSpecialAbilityCooldown = 0;

        StartRunEntityCoroutine();

        if (team == 0) return;
        _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material.shader
                = _gameManager.GameDatabase.SepiaToneShader;
    }

    protected override IEnumerator RunEntity()
    {
        while (true)
        {
            if (IsDead) yield break;
            yield return ManageSpecialAbility();
            yield return ManagePathing();
            yield return ManageAttackCoroutine();
        }
    }

    protected virtual IEnumerator ManageSpecialAbility()
    {
        if (!_hasSpecialAction) yield break;
        if (!CanUseSpecialAbility()) yield break;

        if (_currentSpecialAbilityCoroutine != null)
            StopCoroutine(_currentSpecialAbilityCoroutine);
        _currentSpecialAbilityCoroutine = SpecialAbility();
        yield return _currentSpecialAbilityCoroutine;
    }

    protected IEnumerator ManagePathing()
    {
        if (Opponent == null || Opponent.IsDead)
            ChooseNewTarget();
        yield return new WaitForSeconds(0.1f);

        // HERE: creature could be patrolling or sth
        if (Opponent == null)
        {
            StopRunEntityCoroutine();
            Invoke(nameof(StartRunEntityCoroutine), Random.Range(0.5f, 2f));
            yield break;
        }

        if (_currentSecondaryCoroutine != null)
            StopCoroutine(_currentSecondaryCoroutine);
        _currentSecondaryCoroutine = PathToOpponent();
        yield return _currentSecondaryCoroutine;
    }

    protected IEnumerator ManageAttackCoroutine()
    {
        if (_currentSecondaryCoroutine != null)
            StopCoroutine(_currentSecondaryCoroutine);
        _currentSecondaryCoroutine = Attack();
        yield return _currentSecondaryCoroutine;
    }

    protected bool CanUseSpecialAbility()
    {
        if (Creature.CreatureAbility == null) return false;
        if (CurrentSpecialAbilityCooldown > 0) return false;
        if (!Creature.IsAbilityUnlocked()) return false;
        return true;
    }

    protected virtual IEnumerator PathToOpponent()
    {
        yield return PathToPosition(Opponent.transform.position);

        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            if (_hasSpecialAction && CanUseSpecialAbility())
            {
                yield return ManageSpecialAbility();
                yield break;
            }

            _agent.SetDestination(Opponent.transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        // reached destination
        _agent.avoidancePriority = 0;
        Animator.SetBool("Move", false);
        _agent.enabled = false;
    }

    public override void Engage(BattleEntity engager)
    {
        base.Engage(engager);
        Opponent = engager;
        StartRunEntityCoroutine();
    }

    protected virtual IEnumerator Attack()
    {
        EntityLog.Add($"{Time.time}: Entity attacked {Opponent.name}");

        // meant to be overwritten
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
        if (Opponent == null) return false;
        if (Opponent.IsDead) return false;

        // +0.5 wiggle room
        return Vector3.Distance(transform.position, Opponent.transform.position) < Creature.AttackRange + 0.5f;
    }

    protected void ChooseNewTarget()
    {
        if (_opponentList.Count == 0)
        {
            Opponent = null;
            return;
        }

        // choose a random opponent with a bias towards closer opponents
        Dictionary<BattleEntity, float> distances = new();
        foreach (BattleEntity be in _opponentList)
        {
            if (be.IsDead) continue;
            float distance = Vector3.Distance(transform.position, be.transform.position);
            distances.Add(be, distance);
        }

        var closest = distances.OrderByDescending(pair => pair.Value).Reverse().Take(10);
        float v = Random.value;

        Dictionary<BattleEntity, float> closestBiased = new();

        float sum = 0;
        foreach (KeyValuePair<BattleEntity, float> entry in closest)
        {
            // this number decides bias towards closer opponents
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
                EntityLog.Add($"{Time.time}: Choosing {entry.Key.name} as new target, #{closestNormalized.Values.ToList().IndexOf(entry.Value)} closest.");
                SetOpponent(entry.Key);
                return;
            }
            v -= entry.Value;
        }
    }

    public void SetOpponent(BattleEntity opponent)
    {
        Opponent = opponent;
        Opponent.OnDeath += (_, _) =>
        {
            if (this == null) return;
            StartRunEntityCoroutine();
        };
    }

    public override void Grabbed()
    {
        base.Grabbed();
        Opponent = null;
    }

    public void DealtDamage(int dmg)
    {
        DamageDealt += dmg;
        OnDamageDealt?.Invoke(dmg);
    }

    public void IncreaseKillCount()
    {
        KilledEnemiesCount++;
        OnEnemyKilled?.Invoke(KilledEnemiesCount);
    }

    public override IEnumerator Die(GameObject attacker = null, bool hasLoot = true)
    {
        yield return base.Die(attacker, hasLoot);

        if (Team != 0) yield break;
        GameObject g = Instantiate(_gravePrefab, transform.position, Quaternion.identity);
        g.GetComponent<BattleCreatureGrave>().Initialize(Creature);
    }

    /* EVOLUTION */
    void OnLevelUp()
    {
        DisplayFloatingText("Level Up!", Color.white);
        ResolveEvolution();
    }

    void ResolveEvolution()
    {
        int maxTier = _gameManager.SelectedBattle.Spire.StoreyTroops.CreatureTierTree.CurrentValue.Value;
        if (Creature.UpgradeTier >= maxTier) return;
        if (Creature.ShouldEvolve())
        {
            // HERE: evolution for now just evolve
            // later, I want creature to start blinking and maybe the hotkey shows something
            // and you have to click a button to evolve

            StopAllCoroutines();
            Evolve();
        }
    }

    protected virtual void Evolve()
    {
        SetDead();
        _blockRunEntity = true;
        _battleEntityHighlight.DisableHighlightFully();

        EntityLog.Add($"{Time.time}: Creature is evolving...");
        StopRunEntityCoroutine();
        OnEvolving?.Invoke(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Trigger Evolution")]
    public void TriggerEvolution()
    {
        Evolve();
    }

    [ContextMenu("Trigger Special Ability")]
    public void TriggerSpecialAction()
    {
        StartCoroutine(SpecialAbility());
    }

    [ContextMenu("Trigger Death")]
    public void TriggerDeath()
    {
        TriggerDieCoroutine();
    }

#endif

}
