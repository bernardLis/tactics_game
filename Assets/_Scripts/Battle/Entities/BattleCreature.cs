using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleCreature : BattleEntity
    {
        [FormerlySerializedAs("_attackSound")] [SerializeField]
        protected Sound AttackSound;

        public Creature Creature { get; private set; }

        List<BattleEntity> _opponentList = new();

        protected BattleEntity Opponent { get; private set; }

        protected float CurrentAttackCooldown;
        static readonly int AnimAttack = Animator.StringToHash("Attack");
        static readonly int AnimAbility = Animator.StringToHash("Creature Ability");
        static readonly int AnimDie = Animator.StringToHash("Die");

        float _currentAbilityCooldown;

        [SerializeField] GameObject _respawnEffect;

        BattleHeroOpponentTracker _battleHeroOpponentTracker;

        public event Action<int> OnDamageDealt;
        public event Action<BattleCreature, BattleHero> OnGettingCaught;

        protected virtual void Update()
        {
            if (CurrentAttackCooldown >= 0)
                CurrentAttackCooldown -= Time.deltaTime;
            if (_currentAbilityCooldown >= 0)
                _currentAbilityCooldown -= Time.deltaTime;
        }

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, team);

            Opponent = null;
            if (team == 0) BattleEntityShaders.LitShader();

            Creature = (Creature)entity;
            Creature.OnLevelUp -= OnLevelUp; // just in case I initialize it twice :))))
            Creature.OnLevelUp += OnLevelUp;

            OnDamageDealt -= Creature.AddDmgDealt;
            OnDamageTaken -= Creature.AddDmgTaken;
            OnDamageDealt += Creature.AddDmgDealt;
            OnDamageTaken += Creature.AddDmgTaken;

            BattleEntityPathing.SetAvoidancePriorityRange(new(0, 20));
            BattleEntityPathing.SetStoppingDistance(Creature.AttackRange.GetValue());
        }

        BattleBuildingEntityTracker _entityTracker;

        public void InitializeHostileCreature(BattleBuildingEntityTracker entityTracker)
        {
            _entityTracker = entityTracker;

            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();
            _currentAbilityCooldown = Creature.CreatureAbility.Cooldown;

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
            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();
            _currentAbilityCooldown = Creature.CreatureAbility.Cooldown;

            if (team == 0)
            {
                BattleHero = BattleManager.BattleHero;
                _battleHeroOpponentTracker = BattleHero.GetComponentInChildren<BattleHeroOpponentTracker>();
                _opponentList = BattleManager.OpponentEntities;
            }

            if (team == 1) _opponentList = BattleManager.PlayerEntities;

            EnableSelf();
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
            UnsubscribeFromEvents();
            if (Team == 0)
                _battleHeroOpponentTracker.OnOpponentAdded += OpponentWasAdded;
            if (Team == 1) _entityTracker.OnEntityEnter += OpponentWasAdded;

            if (CurrentMainCoroutine != null)
                StopCoroutine(CurrentMainCoroutine);
            CurrentMainCoroutine = HangOutCoroutine();
            StartCoroutine(CurrentMainCoroutine);
        }

        void OpponentWasAdded(BattleEntity _)
        {
            if (this == null) return;
            StartRunEntityCoroutine();
            UnsubscribeFromEvents();
        }

        void UnsubscribeFromEvents()
        {
            if (_battleHeroOpponentTracker != null)
                _battleHeroOpponentTracker.OnOpponentAdded -= OpponentWasAdded;
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
                BattleEntityPathing.SetStoppingDistance(0);
                yield return BattleEntityPathing.PathToPositionAndStop(pos);
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


        protected IEnumerator ManageCreatureAbility()
        {
            if (!CanUseAbility()) yield break;

            if (CurrentAbilityCoroutine != null)
                StopCoroutine(CurrentAbilityCoroutine);
            CurrentAbilityCoroutine = CreatureAbility();
            yield return CurrentAbilityCoroutine;
        }

        bool CanUseAbility()
        {
            if (!Creature.CanUseAbility()) return false;
            return !(_currentAbilityCooldown > 0);
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
            BattleEntityPathing.SetStoppingDistance(Creature.AttackRange.GetValue());
            yield return BattleEntityPathing.PathToTarget(Opponent.transform);
        }

        public override void GetEngaged(BattleEntity attacker)
        {
            if (IsEngaged) return;
            IsEngaged = true;

            EntityLog.Add($"{BattleManager.GetTime()}: Creature gets engaged by {attacker.name}");
            Opponent = attacker;
            StartRunEntityCoroutine();
        }

        protected virtual IEnumerator Attack()
        {
            while (!CanAttack()) yield return null;
            if (!IsOpponentInRange()) yield break;

            EntityLog.Add($"{BattleManager.GetTime()}: Entity attacked {Opponent.name}");

            CurrentAttackCooldown = Creature.AttackCooldown.GetValue();

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

        protected virtual IEnumerator CreatureAbility()
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Entity uses ability");

            Creature.CreatureAbility.Used();
            _currentAbilityCooldown = Creature.CreatureAbility.Cooldown;

            Animator.SetTrigger(AnimAbility);

            if (Creature.CreatureAbility.Sound != null)
                AudioManager.PlaySFX(Creature.CreatureAbility.Sound, transform.position);


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

        bool CanAttack()
        {
            return CurrentAttackCooldown < 0;
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

        void ChooseNewTarget()
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
            EntityLog.Add($"{BattleManager.GetTime()}: Choosing {closest.name} as new target");

            SetOpponent(closest);
        }

        void SetOpponent(BattleEntity opponent)
        {
            Opponent = opponent;
            Opponent.OnDeath += ResetOpponent;
            if (opponent is not BattleCreature bc) return;
            bc.OnGettingCaught += ResetOpponent;
        }

        void ResetOpponent(BattleEntity _, BattleEntity __)
        {
            if (this == null) return;
            Opponent.OnDeath -= ResetOpponent;
            Opponent = null;
            StartRunEntityCoroutine();
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
        }

        public override IEnumerator Die(BattleEntity attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            Creature.Die();
            UnsubscribeFromEvents();

            Animator.SetTrigger(AnimDie);
            if (Team == 0)
            {
                yield return new WaitForSeconds(3f);
                Gfx.transform.DOScale(0, 0.5f);
                transform.DOMove(BattleHero.transform.position, 0.5f);
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
            _respawnEffect.SetActive(false);
            yield return new WaitForSeconds(Creature.DeathPenaltyBase +
                                            Creature.DeathPenaltyPerLevel * Creature.Level.Value);
            transform.position = BattleHero.transform.position +
                                 new Vector3(Random.Range(-2, 2), 2, Random.Range(-2, 2));

            _respawnEffect.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            transform.DOMoveY(1, 0.3f);
            Gfx.transform.DOScale(1, 0.3f)
                .OnComplete(EnableSelf);
        }

        void EnableSelf()
        {
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;
            BattleEntityPathing.EnableAgent();
            IsDead = false;
            StartRunEntityCoroutine();
        }

        void DisableSelf()
        {
            Collider.enabled = false;
            BattleEntityPathing.DisableAgent();
            StopRunEntityCoroutine();
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

        public void TryCatching(BattleFriendBall ball)
        {
            IsDead = true;
            OnGettingCaught?.Invoke(this, BattleHero);

            DisableSelf();
            Vector3 pos = ball.transform.position;

            transform.DOMove(pos, 0.3f);
            transform.DOScale(0, 0.3f);
        }

        public void Caught(Vector3 spawnPos)
        {
            _battleHeroOpponentTracker = BattleHero.GetComponentInChildren<BattleHeroOpponentTracker>();

            Opponent = null;
            Creature.Caught(BattleHero.Hero);
            InitializeEntity(Creature, 0);
            BattleManager.OpponentEntities.Remove(this);
            BattleManager.AddPlayerArmyEntity(this);
            _battleHeroOpponentTracker.RemoveOpponent(this);

            _opponentList = _battleHeroOpponentTracker.OpponentsInRange;
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
#endif
    }
}