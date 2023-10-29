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

    public Creature Creature { get; private set; }

    List<BattleEntity> _opponentList = new();

    public BattleEntity Opponent { get; private set; }

    protected float _currentAttackCooldown;
    public float CurrentAbilityCooldown { get; private set; }

    public int DamageDealt { get; private set; }

    public event Action<int> OnDamageDealt;
    protected virtual void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
        if (CurrentAbilityCooldown >= 0)
            CurrentAbilityCooldown -= Time.deltaTime;
    }

    public override void InitializeEntity(Entity entity, int team)
    {
        base.InitializeEntity(entity, team);

        if (team == 0) _battleEntityShaders.LitShader();

        Creature = (Creature)entity;
        Creature.OnLevelUp += OnLevelUp;

        OnDamageDealt += Creature.AddDmgDealt;
        OnDamageTaken += Creature.AddDmgTaken;

        _agent.stoppingDistance = Creature.AttackRange.GetValue();
        _avoidancePriorityRange = new Vector2Int(0, 20);
    }

    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _opponentList = opponents;

        _currentAttackCooldown = 0;
        CurrentAbilityCooldown = 0;

        StartRunEntityCoroutine();
    }

    protected override IEnumerator RunEntity()
    {
        while (true)
        {
            if (IsDead) yield break;
            if (_opponentList.Count == 0) StartHangOutCoroutine();

            yield return ManagePathing();
            yield return ManageAttackCoroutine();
        }
    }

    void StartHangOutCoroutine()
    {
        if (Team == 0) _battleManager.OnOpponentEntityAdded += OpponentWasAdded;
        if (Team == 1) _battleManager.OnPlayerCreatureAdded += OpponentWasAdded;

        if (_currentMainCoroutine != null)
            StopCoroutine(_currentMainCoroutine);
        _currentMainCoroutine = HangOut();
        StartCoroutine(_currentMainCoroutine);
    }

    void OpponentWasAdded(BattleEntity _)
    {
        if (this == null) return;
        StartRunEntityCoroutine();
        if (Team == 0) _battleManager.OnOpponentEntityAdded -= OpponentWasAdded;
        if (Team == 1) _battleManager.OnPlayerCreatureAdded -= OpponentWasAdded;
    }

    protected virtual IEnumerator HangOut()
    {
        if (Team == 1) yield break; // TODO: not implemented for enemies
        while (true)
        {
            // TODO: need to make sure that position is reachable
            BattleHero battleHero = _battleManager.GetComponent<BattleHeroManager>().BattleHero;
            _agent.stoppingDistance = 0;
            yield return PathToPositionAndStop(battleHero.transform.position
                                        + Vector3.right * Random.Range(-10f, 10f)
                                        + Vector3.forward * Random.Range(-10f, 10f));

            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    protected virtual IEnumerator ManageCreatureAbility()
    {
        if (!CanUseAbility()) yield break;

        if (_currentAbilityCoroutine != null)
            StopCoroutine(_currentAbilityCoroutine);
        _currentAbilityCoroutine = CreatureAbility();
        yield return _currentAbilityCoroutine;
    }

    protected bool CanUseAbility()
    {
        if (!Creature.CanUseAbility()) return false;
        if (CurrentAbilityCooldown > 0) return false;
        return true;
    }

    protected IEnumerator ManagePathing()
    {
        if (Opponent == null || Opponent.IsDead)
            ChooseNewTarget();
        yield return new WaitForSeconds(0.1f);

        if (Opponent == null) yield break;

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

    protected virtual IEnumerator PathToOpponent()
    {
        _agent.stoppingDistance = Creature.AttackRange.GetValue();
        yield return PathToTarget(Opponent.transform);
    }

    public override void Engage(BattleEntity engager)
    {
        if (_isEngaged) return;
        _isEngaged = true;

        EntityLog.Add($"{_battleManager.GetTime()}: Creature gets engaged by {engager.name}");
        Opponent = engager;
        StartRunEntityCoroutine();
    }

    protected virtual IEnumerator Attack()
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity attacked {Opponent.name}");

        while (!CanAttack()) yield return null;
        if (!IsOpponentInRange()) yield break;
        _currentAttackCooldown = Creature.AttackCooldown.GetValue();

        if (_attackSound != null) _audioManager.PlaySFX(_attackSound, transform.position);
        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Attack");

        bool isAttack = false;
        while (true)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack"))
                isAttack = true;
            bool isAttackFinished = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f;

            if (isAttack && isAttackFinished) break;

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected virtual IEnumerator CreatureAbility()
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity uses ability");

        Creature.CreatureAbility.Used();
        CurrentAbilityCooldown = Creature.CreatureAbility.Cooldown;

        Animator.SetTrigger("Creature Ability");

        if (Creature.CreatureAbility.Sound != null)
            _audioManager.PlaySFX(Creature.CreatureAbility.Sound, transform.position);


        bool isAbility = false;
        while (true)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Creature Ability"))
                isAbility = true;
            bool isAbilityFinished = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f;

            if (isAbility && isAbilityFinished) break;

            yield return new WaitForSeconds(0.1f);
        }
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
        return Vector3.Distance(transform.position, Opponent.transform.position) < Creature.AttackRange.GetValue() + 0.5f;
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
                EntityLog.Add($"{_battleManager.GetTime()}: Choosing {entry.Key.name} as new target, #{closestNormalized.Values.ToList().IndexOf(entry.Value)} closest.");
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

    public override IEnumerator Die(EntityFight attacker = null, bool hasLoot = true, bool hasGrave = true)
    {
        yield return base.Die(attacker, hasLoot);

        _battleManager.OnOpponentEntityAdded -= OpponentWasAdded;

        if (Team != 0) yield break;
    }

    void OnLevelUp()
    {
        DisplayFloatingText("Level Up!", Color.white);
        Creature.CurrentHealth.SetValue(Creature.MaxHealth.GetValue());
    }


#if UNITY_EDITOR
    [ContextMenu("Level up")]
    public void LevelUp()
    {
        Creature.LevelUp();
    }

    [ContextMenu("Trigger Ability")]
    public void TriggerAbility()
    {
        StartCoroutine(CreatureAbility());
    }


    [ContextMenu("Trigger Death")]
    public void TriggerDeath()
    {
        TriggerDieCoroutine(hasGrave: true);
    }

#endif

}
