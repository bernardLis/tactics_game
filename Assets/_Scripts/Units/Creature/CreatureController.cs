using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Battle.Land.Building;
using Lis.Core;
using Lis.Units.Creature.Ability;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        [FormerlySerializedAs("_attackSound")] [SerializeField]
        protected Sound AttackSound;

        public Creature Creature { get; private set; }

        List<UnitController> _opponentList = new();

        public UnitController Opponent { get; private set; }

        float _currentAttackCooldown;
        static readonly int AnimAttack = Animator.StringToHash("Attack");
        static readonly int AnimDie = Animator.StringToHash("Die");

        [SerializeField] GameObject _respawnEffect;

        OpponentTracker _opponentTracker;
        Controller _controller;

        public event Action<int> OnDamageDealt;
        public event Action<CreatureController, HeroController> OnGettingCaught;
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

        PlayerUnitTracker _entityTracker;

        public void InitializeHostileCreature(PlayerUnitTracker entityTracker)
        {
            _entityTracker = entityTracker;

            _currentAttackCooldown = Creature.AttackCooldown.GetValue();
            _opponentList = _entityTracker.PlayerEntitiesWithinRange;

            EnableSelf();
            // HERE: testing
            for (int i = 0; i < 6; i++)
            {
                Creature.LevelUp();
            }
        }

        // HERE: testing
        public void DebugInitialize(int team)
        {
            _currentAttackCooldown = Creature.AttackCooldown.GetValue();

            if (team == 0)
            {
                HeroController = BattleManager.HeroController;
                _opponentTracker = HeroController.GetComponentInChildren<OpponentTracker>();
                _opponentList = BattleManager.OpponentEntities;
            }

            if (team == 1) _opponentList = BattleManager.PlayerEntities;

            EnableSelf();
        }

        protected override IEnumerator RunUnitCoroutine()
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
            UnsubscribeFromEvents();
            if (Team == 0)
                _opponentTracker.OnOpponentAdded += OpponentWasAdded;
            if (Team == 1) _entityTracker.OnEntityEnter += OpponentWasAdded;

            if (CurrentMainCoroutine != null)
                StopCoroutine(CurrentMainCoroutine);
            CurrentMainCoroutine = HangOutCoroutine();
            StartCoroutine(CurrentMainCoroutine);
        }

        void OpponentWasAdded(UnitController _)
        {
            if (this == null) return;
            RunUnit();
            UnsubscribeFromEvents();
        }

        void UnsubscribeFromEvents()
        {
            if (_opponentTracker != null)
                _opponentTracker.OnOpponentAdded -= OpponentWasAdded;
            if (_entityTracker != null)
                _entityTracker.OnEntityEnter -= OpponentWasAdded;
        }

        IEnumerator HangOutCoroutine()
        {
            while (true)
            {
                if (_opponentList.Count > 0) yield break;
                Vector3 pos = Team == 0
                    ? GetPositionCloseToHero()
                    : GetPositionAroundBuilding();
                UnitPathingController.SetStoppingDistance(0);
                yield return UnitPathingController.PathToPositionAndStop(pos);
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
        }

        Vector3 GetPositionAroundBuilding()
        {
            Vector3 pos = _entityTracker.transform.position
                          + Vector3.right * Random.Range(-10f, 10f)
                          + Vector3.forward * Random.Range(-10f, 10f);
            if (!NavMesh.SamplePosition(pos, out NavMeshHit _, 1f, NavMesh.AllAreas))
                return GetPositionAroundBuilding();
            return pos;
        }

        IEnumerator ManagePathing()
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

        IEnumerator ManageAttackCoroutine()
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
            while (!CanAttack()) yield return null;
            if (!IsOpponentInRange()) yield break;
            OnAttackReady?.Invoke();

            AddToLog($"Unit attacked {Opponent.name}");

            _currentAttackCooldown = Creature.AttackCooldown.GetValue();

            if (AttackSound != null) AudioManager.PlaySFX(AttackSound, transform.position);
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
            return _currentAttackCooldown < 0;
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
            if (opponent is not CreatureController bc) return;
            bc.OnGettingCaught += ResetOpponent;
        }

        void ResetOpponent(UnitController _, UnitController __)
        {
            if (this == null) return;
            if (Opponent == null) return;
            if (IsDead) return;
            Opponent.OnDeath -= ResetOpponent;
            Opponent = null;
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
            _controller = Instantiate(Creature.Ability.Prefab, transform)
                .GetComponent<Controller>();
            _controller.Initialize(this);
        }

        public override IEnumerator Die(UnitController attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            Creature.Die();
            UnsubscribeFromEvents();

            Animator.SetTrigger(AnimDie);
            if (Team == 0)
            {
                yield return new WaitForSeconds(3f);
                Gfx.transform.DOScale(0, 0.5f);
                transform.DOMove(HeroController.transform.position, 0.5f);
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(Respawn());

                yield break;
            }

            transform.DOMoveY(-1, 10f)
                .SetDelay(3f)
                .OnComplete(DeactivateSelf);
        }

        IEnumerator Respawn()
        {
            Animator.Rebind();
            Animator.Update(0f);
            Creature.CurrentHealth.SetValue(Creature.MaxHealth.GetValue());

            _respawnEffect.SetActive(false);
            yield return new WaitForSeconds(Creature.DeathPenaltyBase +
                                            Creature.DeathPenaltyPerLevel * Creature.Level.Value);
            transform.position = HeroController.transform.position +
                                 new Vector3(Random.Range(-2, 2), 2, Random.Range(-2, 2));

            _respawnEffect.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            transform.DOMoveY(1, 0.3f);
            Gfx.transform.DOScale(1, 0.3f)
                .OnComplete(EnableSelf);

            if (_controller != null)
                _controller.StartAbilityCooldownCoroutine();
        }

        void EnableSelf()
        {
            Collider.enabled = true;
            IsDeathCoroutineStarted = false;
            UnitPathingController.EnableAgent();
            IsDead = false;
            RunUnit();

            if (_controller != null)
                _controller.StartAbilityCooldownCoroutine();
        }

        void DisableSelf()
        {
            Collider.enabled = false;
            UnitPathingController.DisableAgent();
            StopUnit();
            StopAllCoroutines();
            transform.DOKill();
        }

        void DeactivateSelf()
        {
            StopAllCoroutines();
            transform.DOKill();
            gameObject.SetActive(false);
        }

        /* CATCHING */

        public void TryCatching(FriendBallController ballController)
        {
            IsDead = true;
            OnGettingCaught?.Invoke(this, HeroController);

            DisableSelf();
            Vector3 pos = ballController.transform.position;

            transform.DOMove(pos, 0.3f);
            transform.DOScale(0, 0.3f);
        }

        public void Caught(Vector3 spawnPos)
        {
            _opponentTracker = HeroController.GetComponentInChildren<OpponentTracker>();

            Opponent = null;
            Creature.Caught(HeroController.Hero);
            InitializeUnit(Creature, 0);
            BattleManager.OpponentEntities.Remove(this);
            BattleManager.AddPlayerArmyEntity(this);
            _opponentTracker.RemoveOpponent(this);

            _opponentList = _opponentTracker.OpponentsInRange;
            _entityTracker.OnEntityEnter -= OpponentWasAdded;

            transform.DOMove(spawnPos, 0.5f)
                .OnComplete(ReleaseFromCatching);
        }

        public void ReleaseFromCatching()
        {
            transform.DOMoveY(1, 0.3f);
            transform.DOScale(1, 0.3f)
                .OnComplete(EnableSelf);
        }


#if UNITY_EDITOR
        [ContextMenu("Level up")]
        public void LevelUp()
        {
            Creature.LevelUp();
        }


        [ContextMenu("Trigger Death")]
        public void TriggerDeath()
        {
            TriggerDieCoroutine();
        }
#endif
    }
}