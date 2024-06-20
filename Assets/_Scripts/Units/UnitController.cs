using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Attack;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    public class UnitController : MonoBehaviour
    {
        static readonly int AnimDie = Animator.StringToHash("Die");
        static readonly int AnimTakeDamage = Animator.StringToHash("Take Damage");

        public List<string> UnitLog = new();

        [Header("Effects")]
        [SerializeField]
        GameObject _levelUpEffect;

        [SerializeField] protected GameObject DeathEffect;
        [HideInInspector] public bool IsShielded;
        protected ArenaManager ArenaManager;
        AttackController _attackController;

        protected IEnumerator CurrentMainCoroutine;
        IEnumerator _currentSecondaryCoroutine;

        MMF_Player _feelPlayer;

        Color _healthColor;

        bool _isAttackReady;
        bool _isDeathCoroutineStarted;
        bool _isEngaged;

        List<UnitController> _opponentList = new();
        protected PickupManager PickupManager;
        Color _shieldColor;
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;
        protected FightManager FightManager;

        protected GameManager GameManager;

        protected ObjectShaders ObjectShaders;

        protected UnitPathingController UnitPathingController;
        public Collider Collider { get; private set; }
        public Animator Animator { get; protected set; }

        public Unit Unit { get; private set; }
        public int Team { get; private set; }
        public UnitController Opponent { get; private set; }

        public bool IsDead { get; protected set; }

        public event Action OnShieldBroken;
        public event Action<Attack.Attack> OnHit;
        public event Action<UnitController, Attack.Attack> OnDeath;

        public virtual void InitializeGameObject()
        {
            GameManager = GameManager.Instance;
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            PickupManager = BattleManager.GetComponent<PickupManager>();
            FightManager = BattleManager.GetComponent<FightManager>();
            ArenaManager = BattleManager.GetComponent<ArenaManager>();

            _healthColor = GameManager.GameDatabase.GetColorByName("Health").Primary;
            _shieldColor = GameManager.GameDatabase.GetColorByName("Water").Primary;

            ObjectShaders = GetComponent<ObjectShaders>();
            Collider = GetComponent<Collider>();
            Animator = GetComponentInChildren<Animator>();
            _feelPlayer = GetComponent<MMF_Player>();

            AddToLog("Game Object is initialized.");
        }

        public virtual void InitializeUnit(Unit unit, int team)
        {
            AddToLog($"Unit is initialized, team: {team}");

            if (unit.SpawnSound != null)
                AudioManager.PlaySound(unit.SpawnSound, transform.position);

            Opponent = null;
            Unit = unit;
            Team = team;

            name = Team + "_" + Helpers.ParseScriptableObjectName(Unit.name)
                   + "_" + Helpers.GetRandomNumber(4);

            FightManager.OnFightStarted += OnFightStarted;
            FightManager.OnFightEnded += OnFightEnded;

            EnableSelf();
            ResolveCollisionLayers(team);
            InitializeControllers();
            InitializeAttacks();
        }

        protected virtual void InitializeControllers()
        {
            UnitPathingController = GetComponent<UnitPathingController>();
            if (UnitPathingController != null)
            {
                UnitPathingController.Initialize(new(20, 100));
                UnitPathingController.InitializeUnit(Unit);
            }

            if (TryGetComponent(out UnitHitController hit))
                hit.Initialize(this);
        }

        void InitializeAttacks()
        {
            foreach (Attack.Attack attack in Unit.Attacks)
                attack.InitializeAttack(this);

            Unit.OnAttackAdded += attack => attack.InitializeAttack(this);
            Unit.OnAttackRemoved += attack => Destroy(attack.AttackController.gameObject);
        }

        public void EnableSelf()
        {
            AddToLog("Unit enables itself.");
            FightManager.OnFightStarted -= OnFightStarted;
            FightManager.OnFightEnded -= OnFightEnded;
            FightManager.OnFightStarted += OnFightStarted;
            FightManager.OnFightEnded += OnFightEnded;

            Collider.enabled = true;
            DeathEffect.SetActive(false);
            _isDeathCoroutineStarted = false;
            IsDead = false;

            Unit.ResetAttackCooldowns();
            _isAttackReady = true;
        }

        public void SetOpponentList(ref List<UnitController> list)
        {
            _opponentList = list;
            if (FightManager.IsFightActive) RunUnit();
        }

        void ResolveCollisionLayers(int team)
        {
            switch (team)
            {
                case 0:
                    gameObject.layer = 10;
                    Collider.excludeLayers = LayerMask.GetMask("Player");
                    break;
                case 1:
                    gameObject.layer = 11;
                    Collider.includeLayers = LayerMask.GetMask("Player", "Ability");
                    break;
            }
        }

        public virtual void OnFightStarted()
        {
            if (this == null) return;
            _isAttackReady = true;
            RunUnit();
        }

        protected virtual void OnFightEnded()
        {
            if (this == null) return;
            StopUnit();
            AddToLog("Fight ended!");
        }

        protected virtual void RunUnit()
        {
            AddToLog("Run unit is called.");
            if (IsDead) return;
            StopUnit();

            CurrentMainCoroutine = RunUnitCoroutine();
            StartCoroutine(CurrentMainCoroutine);
        }

        protected virtual void StopUnit()
        {
            AddToLog("Stop unit is called");

            if (_currentSecondaryCoroutine != null)
                StopCoroutine(_currentSecondaryCoroutine);
            if (CurrentMainCoroutine != null)
                StopCoroutine(CurrentMainCoroutine);

            UnitPathingController.DisableAgent();
        }

        protected virtual IEnumerator RunUnitCoroutine()
        {
            while (true)
            {
                if (IsDead) yield break;
                while (_opponentList.Count == 0)
                    yield return new WaitForSeconds(1f);

                _attackController = Unit.ChooseAttack();
                _currentSecondaryCoroutine = ManagePathing();
                yield return _currentSecondaryCoroutine;
                while (!_isAttackReady) yield return _currentSecondaryCoroutine; // attack cooldown
                _currentSecondaryCoroutine = _attackController.AttackCoroutine();
                yield return _currentSecondaryCoroutine;
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

        IEnumerator PathToOpponent()
        {
            AddToLog($"Pathing to opponent {Opponent.name}");
            yield return UnitPathingController.PathToTarget(Opponent.transform,
                Unit.CurrentAttack.Range);
            Opponent.GetEngaged(this); // otherwise, creature can't catch up
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

        protected void ResetOpponent(UnitController _, Attack.Attack __)
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

        public virtual void GetEngaged(UnitController attacker)
        {
            if (IsDead) return;
            if (_isEngaged) return;
            _isEngaged = true;

            AddToLog($"Unit gets engaged by {attacker.name}");
            Opponent = attacker;
            RunUnit();
            Invoke(nameof(Disengage), Random.Range(2f, 4f));
        }

        public void Disengage()
        {
            if (IsDead) return;
            _isEngaged = false;
            AddToLog("Unit disengages");
        }

        public IEnumerator GlobalAttackCooldownCoroutine(float duration)
        {
            AddToLog($"Attack cooldown started: {duration}");
            _isAttackReady = false;
            yield return new WaitForSeconds(duration);
            AddToLog("Attack cooldown ended");
            _isAttackReady = true;
        }

        /* HEALTH AND DEATH */
        public bool HasFullHealth()
        {
            return Unit.CurrentHealth.Value >= Unit.MaxHealth.GetValue();
        }

        public void GetHealed(int value)
        {
            AddToLog($"Unit gets healed by {value}");

            int v = Mathf.FloorToInt(Mathf.Clamp(value, 0,
                Unit.MaxHealth.GetValue() - Unit.CurrentHealth.Value));
            Unit.CurrentHealth.ApplyChange(v);

            DisplayFloatingText("+" + v, _healthColor);
        }

        public IEnumerator GetHit(Attack.Attack attack)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;

            AddToLog($"Unit gets attacked by {attack.name}");

            if (IsShielded)
            {
                BreakShield();
                yield break;
            }

            int damage = Unit.CalculateDamage(attack);
            attack.AddDamageDealt(damage);
            AddToLog($"Unit takes damage {damage}");
            //StopUnit(); <- this stun locks the unit

            if (Unit.HitSound != null) AudioManager.PlaySound(Unit.HitSound, transform.position);
            else AudioManager.PlaySound("Hit", transform.position);
            Animator.SetTrigger(AnimTakeDamage);
            DisplayFloatingText(damage.ToString(), attack.Nature.Color.Primary);

            OnHit?.Invoke(attack);
            if (Unit == null) yield break;
            Unit.CurrentHealth.ApplyChange(-damage);
            if (Unit.CurrentHealth.Value <= 0)
                Die(attack);

            // yield return new WaitForSeconds(0.2f);
            // RunUnit();
        }

        void BreakShield()
        {
            AddToLog("Attack is shielded");
            DisplayFloatingText("Shield broken", _shieldColor);
            IsShielded = false;
            OnShieldBroken?.Invoke();
        }

        public void Die(Attack.Attack attack = null)
        {
            AddToLog("Unit dies.");
            IsDead = true;
            if (attack != null) attack.AddKill(Unit);
            if (gameObject.activeInHierarchy)
                StartCoroutine(DieCoroutine(attack));
        }

        protected virtual IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            if (_isDeathCoroutineStarted) yield break;
            _isDeathCoroutineStarted = true;

            StopUnit();

            ResetOpponent(null, null);

            Collider.enabled = false;
            DOTween.Kill(transform);

            if (Unit.DeathSound != null) AudioManager.PlaySound(Unit.DeathSound, transform.position);
            if (DeathEffect != null) DeathEffect.SetActive(true);

            if (hasLoot) ResolveLoot();

            Animator.SetTrigger(AnimDie);
            OnDeath?.Invoke(this, attack);
        }

        protected void InvokeDeathEvent()
        {
            OnDeath?.Invoke(this, null);
        }

        void ResolveLoot()
        {
            if (Team == 0) return;
            PickupManager.SpawnExpStone(Unit, transform.position);
        }

        /* LEVEL UP */
        protected virtual void OnLevelUp()
        {
            AddToLog("Level up!");
            if (_levelUpEffect != null)
                _levelUpEffect.SetActive(true);
            if (Unit.LevelUpSound != null)
                AudioManager.PlaySound(Unit.LevelUpSound, transform.position);
            StartCoroutine(DisableLevelUpEffect());
        }

        IEnumerator DisableLevelUpEffect()
        {
            yield return new WaitForSeconds(2f);
            _levelUpEffect.SetActive(false);
        }

        /* WEIRD HELPERS */
        public void DisplayFloatingText(string text, Color color)
        {
            if (_feelPlayer == null) return;
            MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
            floatingText.Value = text;
            floatingText.ForceColor = true;
            floatingText.AnimateColorGradient = Helpers.GetGradient(color);
            Transform t = transform;
            _feelPlayer.PlayFeedbacks(t.position + t.localScale.y * Vector3.up);
        }


        public void AddToLog(string s)
        {
            if (BattleManager == null) return;
            UnitLog.Add($"{BattleManager.GetTime()}: {s}.");
        }

        public void DisableSelf()
        {
            FightManager.OnFightStarted -= OnFightStarted;
            FightManager.OnFightEnded -= OnFightEnded;

            StopAllCoroutines();
            DOTween.Kill(transform);
            gameObject.SetActive(false);
        }

        protected void DestroySelf()
        {
            DisableSelf();
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        [Button]
        public void TriggerDeath()
        {
            Unit.CurrentHealth.SetValue(0);
            Die();
        }

        [Button]
        public void LevelUp()
        {
            Unit.LevelUp();
        }

#endif
    }
}