using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core;
using Lis.Units.Creature.Ability;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        [FormerlySerializedAs("_attackSound")] [SerializeField]
        protected Sound AttackSound;

        [SerializeField] Sound _respawnSound;

        public Creature Creature { get; private set; }

        List<UnitController> _opponentList = new();

        public UnitController Opponent { get; private set; }

        float _currentAttackCooldown;
        static readonly int AnimAttack = Animator.StringToHash("Attack");
        static readonly int AnimDie = Animator.StringToHash("Die");

        [SerializeField] GameObject _respawnEffect;
        Controller _abilityController;

        public event Action<int> OnDamageDealt;
        public event Action OnAttackReady;
        public event Action OnStartedMoving;

        protected virtual void Update()
        {
            if (_currentAttackCooldown >= 0)
                _currentAttackCooldown -= Time.deltaTime;
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            Opponent = null;
            if (team == 0) ObjectShaders.LitShader();

            Creature = (Creature)unit;
            Creature.OnLevelUp -= OnLevelUp; // just in case I initialize it twice :))))
            Creature.OnLevelUp += OnLevelUp;

            OnDamageDealt -= Creature.AddDmgDealt;
            OnDamageTaken -= Creature.AddDmgTaken;
            OnDamageDealt += Creature.AddDmgDealt;
            OnDamageTaken += Creature.AddDmgTaken;

            UnitPathingController.SetAvoidancePriorityRange(new(0, 20));
            UnitPathingController.SetStoppingDistance(Creature.AttackRange.GetValue());
        }

        protected override void EnableSelf()
        {
            base.EnableSelf();

            if (_abilityController != null)
                _abilityController.StartAbilityCooldownCoroutine();
        }


        public void SetOpponentList(ref List<UnitController> list)
        {
            _opponentList = list;
        }

        protected virtual void InitializeHostileCreature()
        {
            // TODO:
        }

        protected override void OnFightEnded()
        {
            if (this == null) return;
            if (Team == 1 && IsDead)
            {
                transform.DOMoveY(0f, 5f)
                    .OnComplete(DestroySelf);
                return;
            }

            StartCoroutine(OnFightEndedCoroutine());
        }

        IEnumerator OnFightEndedCoroutine()
        {
            StopUnit();
            AddToLog("Fight ended!");
            if (IsDead) yield return Respawn();
            Creature.CurrentHealth.SetValue(Creature.MaxHealth.GetValue());
            GoBackToLocker();
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            while (true)
            {
                if (IsDead) yield break;
                while (_opponentList.Count == 0)
                    yield return new WaitForSeconds(1f);

                CurrentSecondaryCoroutine = ManagePathing();
                yield return CurrentSecondaryCoroutine;
                CurrentSecondaryCoroutine = AttackCoroutine();
                yield return CurrentSecondaryCoroutine;
            }
        }

        IEnumerator ManagePathing()
        {
            if (Opponent == null || Opponent.IsDead)
                ChooseNewTarget();
            yield return new WaitForSeconds(0.1f);

            if (Opponent == null) yield break;
            yield return PathToOpponent();
        }

        protected virtual IEnumerator PathToOpponent()
        {
            AddToLog($"Pathing to opponent {Opponent.name}");
            OnStartedMoving?.Invoke();
            UnitPathingController.SetStoppingDistance(Creature.AttackRange.GetValue());
            yield return UnitPathingController.PathToTarget(Opponent.transform);
        }

        public override void GetEngaged(UnitController attacker)
        {
            if (IsEngaged) return;
            IsEngaged = true;

            AddToLog($"Creature gets engaged by {attacker.name}");
            Opponent = attacker;
            RunUnit();
        }

        protected virtual IEnumerator AttackCoroutine()
        {
            while (!CanAttack()) yield return new WaitForSeconds(0.1f);
            if (!IsOpponentInRange()) yield break;
            OnAttackReady?.Invoke();

            AddToLog($"Unit attacks {Opponent.name}");

            _currentAttackCooldown = Creature.AttackCooldown.GetValue();

            if (AttackSound != null) AudioManager.PlaySfx(AttackSound, transform.position);
            yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
            Animator.SetTrigger(AnimAttack);

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

        bool CanAttack()
        {
            return _currentAttackCooldown <= 0;
        }

        public bool IsOpponentInRange()
        {
            if (Opponent == null) return false;
            if (Opponent.IsDead) return false;

            // +0.5 wiggle room
            Vector3 delta = Opponent.transform.position - transform.position;
            float distanceSqrt = delta.sqrMagnitude;
            float attackRangeSqrt = (Creature.AttackRange.GetValue() + 0.5f) * (Creature.AttackRange.GetValue() + 0.5f);
            return distanceSqrt <= attackRangeSqrt;
        }

        void ChooseNewTarget()
        {
            if (_opponentList.Count == 0)
            {
                Opponent = null;
                return;
            }

            Dictionary<UnitController, float> sqrtDistances = new();
            foreach (UnitController be in _opponentList)
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

            UnitController closest = sqrtDistances.OrderBy(pair => pair.Value).First().Key;
            AddToLog($"Choosing {closest.name} as new target");

            SetOpponent(closest);
        }

        void SetOpponent(UnitController opponent)
        {
            Opponent = opponent;
            Opponent.OnDeath += ResetOpponent;
        }

        void ResetOpponent(UnitController _, UnitController __)
        {
            AddToLog("Resetting opponent");
            if (this == null) return;
            if (Opponent == null) return;
            Opponent.OnDeath -= ResetOpponent;
            Opponent = null;
            if (!FightManager.IsFightActive) return;
            if (IsDead) return;
            RunUnit();
        }

        public override void Grabbed()
        {
            base.Grabbed();
            Opponent = null;
        }

        public void DealtDamage(int dmg)
        {
            OnDamageDealt?.Invoke(dmg);
        }

        void OnLevelUp()
        {
            DisplayFloatingText("Level Up!", Color.white);
            Creature.CurrentHealth.SetValue(Mathf.FloorToInt(Creature.MaxHealth.GetValue()));

            if (Creature.Ability is null ||
                Creature.Level.Value != Creature.Ability.UnlockLevel) return;
            Creature.Ability.Unlock();
            DisplayFloatingText("Ability Unlocked!", Color.white);
            _abilityController = Instantiate(Creature.Ability.Prefab, transform)
                .GetComponent<Controller>();
            _abilityController.Initialize(this);
        }

        protected override IEnumerator DieCoroutine(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attacker, hasLoot);
            StopUnit();
            UnitPathingController.DisableAgent();
            _respawnEffect.SetActive(false);

            Creature.Die();
            ResetOpponent(null, null);

            Animator.SetTrigger(AnimDie);
        }

        IEnumerator Respawn()
        {
            AddToLog("Respawning...");
            Animator.Rebind();
            Animator.Update(0f);
            _respawnEffect.SetActive(true);
            EnableSelf();
            yield return new WaitForSeconds(1);
        }


#if UNITY_EDITOR
        [ContextMenu("Level up")]
        public void LevelUp()
        {
            Creature.LevelUp();
        }
#endif
    }
}