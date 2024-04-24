using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core;
using Lis.Units.Creature.Ability;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        [FormerlySerializedAs("_attackSound")] [SerializeField]
        protected Sound AttackSound;

        [SerializeField] Sound _respawnSound;

        public Creature Creature { get; private set; }

        List<UnitController> _opponentList = new();

        public UnitController Opponent { get; protected set; }

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

        public void SetOpponentList(ref List<UnitController> list)
        {
            _opponentList = list;
        }

        public virtual void InitializeHostileCreature()
        {
            // TODO:
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            while (true)
            {
                if (IsDead) yield break;
                while (_opponentList.Count == 0)
                {
                    yield return new WaitForSeconds(1f);
                }

                yield return ManagePathing();
                yield return ManageAttackCoroutine();
            }
        }

        void OpponentWasAdded(UnitController _)
        {
            if (this == null) return;
            RunUnit();
            UnsubscribeFromEvents();
        }

        void UnsubscribeFromEvents()
        {
        }

        protected IEnumerator ManagePathing()
        {
            if (Opponent == null || Opponent.IsDead)
                ChooseNewTarget();
            yield return new WaitForSeconds(0.1f);

            if (Opponent == null) yield break;

            if (CurrentSecondaryCoroutine != null)
                StopCoroutine(CurrentSecondaryCoroutine);
            CurrentSecondaryCoroutine = PathToOpponent();
            yield return CurrentSecondaryCoroutine;
        }

        protected IEnumerator ManageAttackCoroutine()
        {
            if (CurrentSecondaryCoroutine != null)
                StopCoroutine(CurrentSecondaryCoroutine);
            CurrentSecondaryCoroutine = Attack();
            yield return CurrentSecondaryCoroutine;
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

        protected virtual IEnumerator Attack()
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
            float distanceSqr = delta.sqrMagnitude;
            float attackRangeSqr = (Creature.AttackRange.GetValue() + 0.5f) * (Creature.AttackRange.GetValue() + 0.5f);
            return distanceSqr <= attackRangeSqr;
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
            if (this == null) return;
            if (Opponent == null) return;
            Opponent.OnDeath -= ResetOpponent;
            Opponent = null;
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

        public override IEnumerator DieCoroutine(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attacker, hasLoot);
            Creature.Die();
            UnsubscribeFromEvents();
            ResetOpponent(null, null);

            Animator.SetTrigger(AnimDie);

            transform.DOMoveY(-1, 10f)
                .SetDelay(3f)
                .OnComplete(DeactivateSelf);
        }

        public void Respawn()
        {
            Animator.Rebind();
            Animator.Update(0f);
            Creature.CurrentHealth.SetValue(Creature.MaxHealth.GetValue());

            transform.DOMoveY(1, 0.3f);
            Gfx.transform.DOScale(1, 0.3f)
                .OnComplete(EnableSelf);
        }

        void EnableSelf()
        {
            AddToLog("Unit enables itself");
            Collider.enabled = true;
            DeathEffect.SetActive(false);
            IsDeathCoroutineStarted = false;
            UnitPathingController.EnableAgent();
            IsDead = false;
            RunUnit();

            if (_abilityController != null)
                _abilityController.StartAbilityCooldownCoroutine();
        }

        void DeactivateSelf()
        {
            StopUnit();
            StopAllCoroutines();

            Collider.enabled = false;
            UnitPathingController.DisableAgent();

            transform.DOKill();
            gameObject.SetActive(false);
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