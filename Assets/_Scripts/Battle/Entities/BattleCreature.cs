using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Lis
{
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
            if (Team == 1) InitializeOpponentEntity();

            _opponentList = opponents;

            _currentAttackCooldown = 0;
            CurrentAbilityCooldown = 0;

            StartRunEntityCoroutine();
        }

        protected virtual void InitializeOpponentEntity()
        {
            transform.localScale = Vector3.one * 0.8f;
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
                _agent.stoppingDistance = 0;
                yield return PathToPositionAndStop(GetPositionCloseToHero());

                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
        }

        protected Vector3 GetPositionCloseToHero()
        {
            BattleHero battleHero = _battleManager.GetComponent<BattleHeroManager>().BattleHero;
            Vector3 pos = battleHero.transform.position
                          + Vector3.right * Random.Range(-10f, 10f)
                          + Vector3.forward * Random.Range(-10f, 10f);
            if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return GetPositionCloseToHero();
            return pos;
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

        public override void GetEngaged(BattleEntity engager)
        {
            if (_isEngaged) return;
            _isEngaged = true;

            EntityLog.Add($"{_battleManager.GetTime()}: Creature gets engaged by {engager.name}");
            Opponent = engager;
            StartRunEntityCoroutine();
        }

        protected virtual IEnumerator Attack()
        {
            while (!CanAttack()) yield return null;
            if (!IsOpponentInRange()) yield break;

            EntityLog.Add($"{_battleManager.GetTime()}: Entity attacked {Opponent.name}");

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
            Vector3 delta = Opponent.transform.position - transform.position;
            return delta.sqrMagnitude <
                   Creature.AttackRange.GetValue() * Creature.AttackRange.GetValue() + 0.5f;
        }

        protected void ChooseNewTarget()
        {
            if (_opponentList.Count == 0)
            {
                Opponent = null;
                return;
            }

            Dictionary<BattleEntity, float> sqrtDistances = new();
            foreach (BattleEntity be in _opponentList)
            {
                if (be.IsDead) continue;
                if (sqrtDistances.ContainsKey(be)) continue;
                Vector3 delta = be.transform.position - transform.position;
                float distance = delta.sqrMagnitude;
                sqrtDistances.Add(be, distance);
            }

            if (sqrtDistances.Count == 0)
            {
                Opponent = null;
                return;
            }

            BattleEntity closest = sqrtDistances.OrderBy(pair => pair.Value).First().Key;
            EntityLog.Add($"{_battleManager.GetTime()}: Choosing {closest.name} as new target");
            SetOpponent(closest);
        }

        void SetOpponent(BattleEntity opponent)
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

        public override IEnumerator Die(EntityFight attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);

            _battleManager.OnOpponentEntityAdded -= OpponentWasAdded;
        }

        void OnLevelUp()
        {
            DisplayFloatingText("Level Up!", Color.white);
            Creature.CurrentHealth.SetValue(Creature.MaxHealth.GetValue());
        }


        // #if UNITY_EDITOR
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
            TriggerDieCoroutine();
        }

        // #endif
    }
}