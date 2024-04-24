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
using Lis.Units.Creature;
using Lis.Units.Hero;
using Lis.Units.Hero.Ability;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    public class UnitController : MonoBehaviour, IGrabbable, IPointerDownHandler
    {
        protected GameManager GameManager; /**/
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;
        GrabManager _grabManager;
        PickupManager _pickupManager;
        FightManager _fightManager;
        ArenaManager _arenaManager;

        public List<string> UnitLog = new();

        [Header("Sounds")]
        [SerializeField] Sound _spawnSound;

        [SerializeField] Sound _levelUpSound;
        [SerializeField] Sound _isRecalledSound;

        [SerializeField] protected Sound DeathSound;
        [SerializeField] protected Sound GetHitSound;

        [Header("Effects")]
        [SerializeField] GameObject _levelUpEffect;

        [FormerlySerializedAs("_deathEffect")] [SerializeField]
        protected GameObject DeathEffect;

        protected ObjectShaders ObjectShaders;
        protected UnitPathingController UnitPathingController;
        public Collider Collider { get; private set; }
        protected GameObject Gfx;
        public Animator Animator { get; private set; }

        public Unit Unit { get; private set; }
        string BattleId { get; set; }
        public int Team { get; private set; }

        [HideInInspector] public bool IsShielded;
        protected bool IsEngaged;
        bool _isPoisoned;
        bool _isGrabbed;

        public bool IsDead { get; protected set; }
        protected bool IsDeathCoroutineStarted;

        MMF_Player _feelPlayer;

        IEnumerator _currentMainCoroutine;
        protected IEnumerator CurrentSecondaryCoroutine;

        static readonly int AnimTakeDamage = Animator.StringToHash("Take Damage");

        protected Color HealthColor;
        Color _shieldColor;

        HeroController _heroController;

        public event Action OnShieldBroken;
        public event Action<int> OnDamageTaken;
        public event Action<UnitController, UnitController> OnDeath;

        public virtual void InitializeGameObject()
        {
            GameManager = GameManager.Instance;
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            _grabManager = GrabManager.Instance;
            _pickupManager = BattleManager.GetComponent<PickupManager>();
            _fightManager = BattleManager.GetComponent<FightManager>();
            _arenaManager = BattleManager.GetComponent<ArenaManager>();

            HealthColor = GameManager.GameDatabase.GetColorByName("Health").Primary;
            _shieldColor = GameManager.GameDatabase.GetColorByName("Water").Primary;

            ObjectShaders = GetComponent<ObjectShaders>();
            UnitPathingController = GetComponent<UnitPathingController>();
            if (UnitPathingController != null)
                UnitPathingController.Initialize(new(20, 100));

            Collider = GetComponent<Collider>();
            Animator = GetComponentInChildren<Animator>();
            Gfx = Animator.gameObject;
            _feelPlayer = GetComponent<MMF_Player>();

            AddToLog($"Game Object is initialized.");
        }

        public virtual void InitializeUnit(Unit unit, int team)
        {
            AddToLog($"Unit is initialized, team: {team}");

            if (_spawnSound != null)
                AudioManager.PlaySfx(_spawnSound, transform.position);

            DeathEffect.SetActive(false);
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;
            IsEngaged = false;

            Unit = unit;
            Team = team;
            unit.OnLevelUp += OnLevelUp;
            if (team == 0)
            {
                gameObject.layer = 10;
                Collider.excludeLayers = LayerMask.GetMask("Player");
            }

            if (team == 1)
            {
                gameObject.layer = 11;
                Collider.includeLayers = LayerMask.GetMask("Player", "Ability");
            }

            BattleId = Team + "_" + Helpers.ParseScriptableObjectName(Unit.name)
                       + "_" + Helpers.GetRandomNumber(4);
            name = BattleId;

            _heroController = BattleManager.GetComponent<HeroManager>().HeroController;

            _fightManager.OnFightStarted += RunUnit;
            _fightManager.OnFightEnded += GoBackToLocker;

            if (Unit is not UnitMovement em) return;
            if (UnitPathingController != null)
                UnitPathingController.InitializeUnit(em);
        }

        void OnLevelUp()
        {
            if (_levelUpEffect == null) return;
            _levelUpEffect.SetActive(true);
            if (_levelUpSound != null)
                AudioManager.PlaySfx(_levelUpSound, transform.position);
            StartCoroutine(DisableLevelUpEffect());
        }

        IEnumerator DisableLevelUpEffect()
        {
            yield return new WaitForSeconds(2f);
            _levelUpEffect.SetActive(false);
        }

        void GoBackToLocker()
        {
            if (IsDead) return;
            if (Team == 1) return;
            UnitPathingController.SetStoppingDistance(1);
            StartCoroutine(
                UnitPathingController.PathToPosition(_arenaManager.GetRandomPositionInPlayerLockerRoom()));
        }

        public virtual void RunUnit()
        {
            AddToLog("Run unit is called");
            if (IsDead) return;

            StopUnit();

            if (!_fightManager.IsFightActive)
            {
                GoBackToLocker();
                return;
            }

            _currentMainCoroutine = RunUnitCoroutine();
            StartCoroutine(_currentMainCoroutine);
        }

        public virtual void StopUnit()
        {
            AddToLog("Stop unit is called");

            if (_currentMainCoroutine != null)
                StopCoroutine(_currentMainCoroutine);
            if (CurrentSecondaryCoroutine != null)
                StopCoroutine(CurrentSecondaryCoroutine);

            UnitPathingController.DisableAgent();
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

            DisplayFloatingText("+" + v, HealthColor);
        }

        public IEnumerator GetHit(Ability ability)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;
            AddToLog($"Unit gets attacked by {ability.name}");

            int damage = Unit.CalculateDamage(ability);
            ability.AddDamageDealt(damage);
            BaseGetHit(damage, ability.Nature.Color.Secondary);

            if (Unit.CurrentHealth.Value <= 0)
                ability.AddKill();
        }

        public virtual IEnumerator GetHit(UnitController attacker, int specialDamage = 0)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;
            AddToLog($"Unit gets attacked by {attacker.name}");

            int damage = Unit.CalculateDamage(attacker.Unit as UnitFight);
            if (specialDamage > 0) damage = specialDamage;
            if (attacker.Unit is not UnitFight attackerFight) yield break;
            attackerFight.AddDmgDealt(damage);

            BaseGetHit(damage, attackerFight.Nature.Color.Primary, attacker);

            if (Unit.CurrentHealth.Value <= 0)
                attackerFight.AddKill(Unit);
        }

        public virtual void BaseGetHit(int dmg, Color color, UnitController attacker = null)
        {
            if (IsShielded)
            {
                AddToLog($"{dmg} damage is shielded");
                BreakShield();
                return;
            }

            AddToLog($"Unit takes damage {dmg}");
            StopUnit();

            if (GetHitSound != null) AudioManager.PlaySfx(GetHitSound, transform.position);
            else AudioManager.PlaySfx("Hit", transform.position);

            Animator.SetTrigger(AnimTakeDamage);
            DisplayFloatingText(dmg.ToString(), color);

            OnDamageTaken?.Invoke(dmg);

            if (Unit == null) return;
            Unit.CurrentHealth.ApplyChange(-dmg);
            if (Unit.CurrentHealth.Value <= 0)
            {
                Die(attacker);
                return;
            }

            RunUnit();
        }

        void BreakShield()
        {
            DisplayFloatingText("Shield broken", _shieldColor);
            IsShielded = false;
            OnShieldBroken?.Invoke();
        }

        protected void Die(UnitController attacker = null)
        {
            AddToLog("Unit dies.");
            IsDead = true;
            if (gameObject.activeInHierarchy)
                StartCoroutine(DieCoroutine(attacker: attacker));
        }

        protected virtual IEnumerator DieCoroutine(UnitController attacker = null, bool hasLoot = true)
        {
            if (IsDeathCoroutineStarted) yield break;
            IsDeathCoroutineStarted = true;

            if (_isGrabbed) GrabManager.Instance.CancelGrabbing();
            Collider.enabled = false;
            DOTween.Kill(transform);

            if (DeathSound != null) AudioManager.PlaySfx(DeathSound, transform.position);
            DeathEffect.SetActive(true);

            if (hasLoot) ResolveLoot();
            OnDeath?.Invoke(this, attacker);
        }

        void ResolveLoot()
        {
            if (Team == 0) return;
            _pickupManager.SpawnExpStone(transform.position);
        }

        public IEnumerator GetPoisoned(CreatureController attacker)
        {
            if (_isPoisoned) yield break;
            if (IsDead) yield break;
            AddToLog($"Unit gets poisoned by {attacker.name}.");

            _isPoisoned = true;
            DisplayFloatingText("Poisoned", Color.green);

            // TODO: for now hardcoded
            int totalDamage = 20;
            int damageTick = 5;

            while (totalDamage > 0)
            {
                if (IsDead) break;

                // poison can't kill
                if (Unit.CurrentHealth.Value > damageTick)
                {
                    DisplayFloatingText(damageTick.ToString(), Color.green);
                    attacker.DealtDamage(damageTick);
                    OnDamageTaken?.Invoke(damageTick);
                    Unit.CurrentHealth.ApplyChange(-damageTick);
                }

                totalDamage -= damageTick;

                yield return new WaitForSeconds(1f);
            }

            _isPoisoned = false;
        }

        /* grab */
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (_spawnSound != null)
                AudioManager.PlaySfx(_spawnSound, transform.position);

            if (!CanBeGrabbed()) return;
            _grabManager.TryGrabbing(gameObject);
        }

        public virtual bool CanBeGrabbed()
        {
            if (IsDead) return false;
            return _grabManager != null;
        }

        public virtual void Grabbed()
        {
            _isGrabbed = true;
            Animator.enabled = false;
            StopUnit();
        }

        public void Released()
        {
            _isGrabbed = false;
            Animator.enabled = true;
            RunUnit();
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


        protected Vector3 GetPositionCloseToHero()
        {
            Vector3 pos = _heroController.transform.position
                          + Vector3.right * Random.Range(-10f, 10f)
                          + Vector3.forward * Random.Range(-10f, 10f);
            if (!NavMesh.SamplePosition(pos, out NavMeshHit _, 1f, NavMesh.AllAreas))
                return GetPositionCloseToHero();
            return pos;
        }

        protected void AddToLog(string s)
        {
            if (BattleManager == null) return;
            UnitLog.Add($"{BattleManager.GetTime()}: {s}.");
        }

#if UNITY_EDITOR
        [ContextMenu("Trigger Death")]
        public void TriggerDeath()
        {
            Die();
        }
#endif
    }
}