using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Core.Utilities;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    public class UnitController : MonoBehaviour
    {
        static readonly int AnimDie = Animator.StringToHash("Die");

        protected GameManager GameManager;
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;
        FightManager _fightManager;
        PickupManager _pickupManager;
        ArenaManager _arenaManager;

        public List<string> UnitLog = new();

        [Header("Effects")]
        [SerializeField] GameObject _levelUpEffect;

        [FormerlySerializedAs("_deathEffect")] [SerializeField]
        protected GameObject DeathEffect;

        protected UnitPathingController UnitPathingController;
        protected AttackController AttackController;

        protected ObjectShaders ObjectShaders;
        public Collider Collider { get; private set; }
        public Animator Animator { get; private set; }

        public Unit Unit { get; private set; }
        string BattleId { get; set; }
        public int Team { get; private set; }

        public UnitController Opponent { get; protected set; }

        [HideInInspector] public bool IsShielded;
        protected bool IsEngaged;
        bool _isPoisoned;

        public bool IsDead { get; private set; }
        bool _isDeathCoroutineStarted;

        MMF_Player _feelPlayer;

        IEnumerator _currentMainCoroutine;
        protected IEnumerator CurrentSecondaryCoroutine;

        static readonly int AnimTakeDamage = Animator.StringToHash("Take Damage");

        Color _healthColor;
        Color _shieldColor;

        public event Action OnShieldBroken;
        public event Action<Attack.Attack> OnHit;
        public event Action<UnitController, Attack.Attack> OnDeath;

        public virtual void InitializeGameObject()
        {
            GameManager = GameManager.Instance;
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            _pickupManager = BattleManager.GetComponent<PickupManager>();
            _fightManager = BattleManager.GetComponent<FightManager>();
            _arenaManager = BattleManager.GetComponent<ArenaManager>();

            _healthColor = GameManager.GameDatabase.GetColorByName("Health").Primary;
            _shieldColor = GameManager.GameDatabase.GetColorByName("Water").Primary;

            ObjectShaders = GetComponent<ObjectShaders>();

            Collider = GetComponent<Collider>();
            Animator = GetComponentInChildren<Animator>();
            _feelPlayer = GetComponent<MMF_Player>();

            AddToLog($"Game Object is initialized.");
        }

        public virtual void InitializeUnit(Unit unit, int team)
        {
            AddToLog($"Unit is initialized, team: {team}");

            if (unit.SpawnSound != null)
                AudioManager.PlaySfx(unit.SpawnSound, transform.position);

            EnableSelf();

            Unit = unit;
            Team = team;
            unit.OnLevelUp += OnLevelUp;

            ResolveCollisionLayers(team);

            BattleId = Team + "_" + Helpers.ParseScriptableObjectName(Unit.name)
                       + "_" + Helpers.GetRandomNumber(4);
            name = BattleId;

            _fightManager.OnFightStarted += OnFightStarted;
            _fightManager.OnFightEnded += OnFightEnded;

            InitializeControllers();
            InitializeAttacks();
        }

        void InitializeControllers()
        {
            UnitPathingController = GetComponent<UnitPathingController>();
            if (UnitPathingController != null)
            {
                UnitPathingController.Initialize(new(20, 100));
                UnitPathingController.InitializeUnit(Unit);
            }

            if (TryGetComponent(out UnitGrabController grab))
            {
                grab.Initialize(this);
                grab.OnGrabbed += OnGrabbed;
                grab.OnReleased += OnReleased;
            }

            if (TryGetComponent(out UnitHitController hit))
                hit.Initialize(this);
        }

        void InitializeAttacks()
        {
            foreach (Attack.Attack attack in Unit.Attacks)
                attack.InitializeAttack(this);
        }

        protected virtual void EnableSelf()
        {
            AddToLog("Unit enables itself.");
            Collider.enabled = true;
            DeathEffect.SetActive(false);
            _isDeathCoroutineStarted = false;
            IsDead = false;
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

        void OnLevelUp()
        {
            if (_levelUpEffect == null) return;
            _levelUpEffect.SetActive(true);
            if (Unit.LevelUpSound != null)
                AudioManager.PlaySfx(Unit.LevelUpSound, transform.position);
            StartCoroutine(DisableLevelUpEffect());
        }

        IEnumerator DisableLevelUpEffect()
        {
            yield return new WaitForSeconds(2f);
            _levelUpEffect.SetActive(false);
        }

        void OnFightStarted()
        {
            RunUnit();
        }

        protected virtual void OnFightEnded()
        {
            // meant to be overwritten
        }

        protected void GoBackToLocker()
        {
            if (IsDead) return;
            if (Team == 1) return;

            AddToLog("Going back to locker room.");
            UnitPathingController.SetStoppingDistance(0);
            _currentMainCoroutine =
                UnitPathingController.PathToPositionAndStop(_arenaManager.GetRandomPositionInPlayerLockerRoom());
            StartCoroutine(_currentMainCoroutine);
        }

        public virtual void RunUnit()
        {
            AddToLog("Run unit is called.");
            if (IsDead) return;
            StopUnit();

            _currentMainCoroutine = RunUnitCoroutine();
            StartCoroutine(_currentMainCoroutine);
        }

        public virtual void StopUnit()
        {
            AddToLog("Stop unit is called");

            if (CurrentSecondaryCoroutine != null)
                StopCoroutine(CurrentSecondaryCoroutine);
            if (_currentMainCoroutine != null)
                StopCoroutine(_currentMainCoroutine);

            UnitPathingController.DisableAgent();
            if (AttackController != null) AttackController.StopAllCoroutines();
        }

        protected virtual IEnumerator RunUnitCoroutine()
        {
            // meant to be overwritten
            yield return null;
        }


        public virtual void GetEngaged(UnitController attacker)
        {
            if (IsDead) return;
            if (IsEngaged) return;
            IsEngaged = true;

            AddToLog($"Unit gets engaged by {attacker.name}");
            StopUnit();
            StartCoroutine(UnitPathingController.PathToTarget(attacker.transform));
            Invoke(nameof(Disengage), Random.Range(2f, 4f));
        }

        public void Disengage()
        {
            if (IsDead) return;
            IsEngaged = false;
            AddToLog("Unit disengages");
            RunUnit();
        }

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
                AddToLog($"Attack is shielded");
                BreakShield();
                yield break;
            }

            int damage = Unit.CalculateDamage(attack);
            attack.AddDamageDealt(damage);
            AddToLog($"Unit takes damage {damage}");
            StopUnit();

            if (Unit.HitSound != null) AudioManager.PlaySfx(Unit.HitSound, transform.position);
            else AudioManager.PlaySfx("Hit", transform.position);
            Animator.SetTrigger(AnimTakeDamage);
            DisplayFloatingText(damage.ToString(), attack.Nature.Color.Primary);

            OnHit?.Invoke(attack);
            if (Unit == null) yield break;
            Unit.CurrentHealth.ApplyChange(-damage);
            if (Unit.CurrentHealth.Value <= 0)
            {
                Die(attack);
                yield break;
            }

            RunUnit();
        }

        void BreakShield()
        {
            DisplayFloatingText("Shield broken", _shieldColor);
            IsShielded = false;
            OnShieldBroken?.Invoke();
        }

        void Die(Attack.Attack attack = null)
        {
            AddToLog("Unit dies.");
            IsDead = true;
            if (attack != null) attack.AddKill(Unit);
            if (gameObject.activeInHierarchy)
                StartCoroutine(DieCoroutine(attack: attack));
        }

        protected virtual IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            if (_isDeathCoroutineStarted) yield break;
            _isDeathCoroutineStarted = true;

            Collider.enabled = false;
            DOTween.Kill(transform);

            if (Unit.DeathSound != null) AudioManager.PlaySfx(Unit.DeathSound, transform.position);
            DeathEffect.SetActive(true);

            if (hasLoot) ResolveLoot();

            Animator.SetTrigger(AnimDie);
            OnDeath?.Invoke(this, attack);
        }

        void ResolveLoot()
        {
            if (Team == 0) return;
            _pickupManager.SpawnExpStone(transform.position);
        }

        /* grab */
        protected virtual void OnGrabbed()
        {
            StopUnit();
        }

        void OnReleased()
        {
            if (FightManager.IsFightActive) RunUnit();
            else if (!_arenaManager.IsPositionInPlayerLockerRoom(transform.position)) GoBackToLocker();
        }

        /* weird helpers */
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

        protected void DestroySelf()
        {
            _fightManager.OnFightStarted -= OnFightStarted;
            _fightManager.OnFightEnded -= OnFightEnded;

            StopAllCoroutines();
            DOTween.Kill(transform);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        [ContextMenu("Trigger Death")]
        public void TriggerDeath() => Die();

        [ContextMenu("Level up")]
        public void LevelUp() => Unit.LevelUp();
#endif
    }
}