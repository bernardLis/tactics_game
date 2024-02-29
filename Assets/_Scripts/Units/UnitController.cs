using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        BattleGrabManager _grabManager;
        BattlePickupManager _pickupManager;

        public List<string> UnitLog = new();

        [Header("Sounds")]
        [SerializeField] Sound _spawnSound;

        [SerializeField] protected Sound DeathSound;
        [SerializeField] protected Sound GetHitSound;

        [Header("Effects")]
        [SerializeField] GameObject _levelUpEffect;

        [SerializeField] GameObject _deathEffect;

        protected ObjectShaders ObjectShaders;
        protected UnitPathingController UnitPathingController;
        public Collider Collider { get; private set; }
        protected GameObject Gfx;
        protected Animator Animator { get; private set; }

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

        protected IEnumerator CurrentMainCoroutine;
        protected IEnumerator CurrentSecondaryCoroutine;

        static readonly int AnimTakeDamage = Animator.StringToHash("Take Damage");
        static readonly int AnimCelebrate = Animator.StringToHash("Celebrate");

        protected Color HealthColor;
        Color _shieldColor;

        protected HeroController HeroController;

        public event Action OnShieldBroken;
        public event Action<int> OnDamageTaken;
        public event Action<UnitController, UnitController> OnDeath;

        public virtual void InitializeGameObject()
        {
            UnitLog.Add($"{Time.time}: (GAME TIME) Unit is instantiated.");

            GameManager = GameManager.Instance;
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            _grabManager = BattleGrabManager.Instance;
            _pickupManager = BattleManager.GetComponent<BattlePickupManager>();

            HealthColor = GameManager.GameDatabase.GetColorByName("Health").Primary;
            _shieldColor = GameManager.GameDatabase.GetColorByName("Water").Primary;

            ObjectShaders = GetComponent<ObjectShaders>();
            UnitPathingController = GetComponent<UnitPathingController>();
            UnitPathingController.Initialize(new(20, 100));

            Collider = GetComponent<Collider>();
            Animator = GetComponentInChildren<Animator>();
            Gfx = Animator.gameObject;
            _feelPlayer = GetComponent<MMF_Player>();

            BattleManager.OnBattleFinalized += () => StartCoroutine(Celebrate());
        }

        public virtual void InitializeEntity(Unit unit, int team)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Unit is initialized");

            if (_spawnSound != null)
                AudioManager.PlaySFX(_spawnSound, transform.position);

            _deathEffect.SetActive(false);
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

            HeroController = BattleManager.HeroController;

            if (Unit is not UnitMovement em) return;
            UnitPathingController.InitializeUnit(em);
        }

        void OnLevelUp()
        {
            if (_levelUpEffect == null) return;
            _levelUpEffect.SetActive(true);
            StartCoroutine(DisableLevelUpEffect());
        }

        IEnumerator DisableLevelUpEffect()
        {
            yield return new WaitForSeconds(2f);
            _levelUpEffect.SetActive(false);
        }

        public virtual void StartRunEntityCoroutine()
        {
            if (IsDead) return;

            StopRunEntityCoroutine();
            UnitLog.Add($"{BattleManager.GetTime()}: Start run unit coroutine is called");

            CurrentMainCoroutine = RunEntity();
            StartCoroutine(CurrentMainCoroutine);
        }

        public virtual void StopRunEntityCoroutine()
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Stop run unit coroutine is called");

            if (CurrentMainCoroutine != null)
                StopCoroutine(CurrentMainCoroutine);
            if (CurrentSecondaryCoroutine != null)
                StopCoroutine(CurrentSecondaryCoroutine);

            UnitPathingController.DisableAgent();
        }

        protected virtual IEnumerator RunEntity()
        {
            // meant to be overwritten
            yield return null;
        }


        public virtual void GetEngaged(UnitController attacker)
        {
            if (IsEngaged) return;
            IsEngaged = true;

            UnitLog.Add($"{BattleManager.GetTime()}: Unit gets engaged by {attacker.name}");
            StartCoroutine(UnitPathingController.PathToTarget(attacker.transform));

            // StopRunEntityCoroutine();
            Invoke(nameof(Disengage), Random.Range(2f, 4f));
        }

        public void Disengage()
        {
            if (IsDead) return;

            IsEngaged = false;
            UnitLog.Add($"{BattleManager.GetTime()}: Unit disengages");
            StartRunEntityCoroutine();
        }

        public bool HasFullHealth()
        {
            return Unit.CurrentHealth.Value >= Unit.MaxHealth.GetValue();
        }

        public void GetHealed(int value)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Unit gets healed by {value}");

            int v = Mathf.FloorToInt(Mathf.Clamp(value, 0,
                Unit.MaxHealth.GetValue() - Unit.CurrentHealth.Value));
            Unit.CurrentHealth.ApplyChange(v);

            DisplayFloatingText("+" + v, HealthColor);
        }

        public virtual IEnumerator GetHit(Ability ability)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;
            UnitLog.Add($"{BattleManager.GetTime()}: Unit gets attacked by {ability.name}");

            BaseGetHit(Unit.CalculateDamage(ability), ability.Element.Color.Secondary);

            if (Unit.CurrentHealth.Value <= 0)
                ability.AddKill();
        }

        public virtual IEnumerator GetHit(UnitController attacker, int specialDamage = 0)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;
            UnitLog.Add($"{BattleManager.GetTime()}: Unit gets attacked by {attacker.name}");

            int damage = Unit.CalculateDamage(attacker.Unit as UnitFight);
            if (specialDamage > 0) damage = specialDamage;
            if (attacker.Unit is not UnitFight attackerFight) yield break;
            attackerFight.AddDmgDealt(damage);

            BaseGetHit(damage, attackerFight.Element.Color.Primary, attacker);

            if (Unit.CurrentHealth.Value <= 0)
                attackerFight.AddKill(Unit);
        }

        public virtual void BaseGetHit(int dmg, Color color, UnitController attacker = null)
        {
            if (IsShielded)
            {
                UnitLog.Add($"{BattleManager.GetTime()}: {dmg} shielded damage");
                BreakShield();
                return;
            }

            UnitLog.Add($"{BattleManager.GetTime()}: Unit takes damage {dmg}");
            StopRunEntityCoroutine();

            if (GetHitSound != null) AudioManager.PlaySFX(GetHitSound, transform.position);
            else AudioManager.PlaySFX("Hit", transform.position);

            Animator.SetTrigger(AnimTakeDamage);
            DisplayFloatingText(dmg.ToString(), color);

            OnDamageTaken?.Invoke(dmg);

            if (Unit == null) return;
            Unit.CurrentHealth.ApplyChange(-dmg);
            if (Unit.CurrentHealth.Value <= 0)
            {
                TriggerDieCoroutine(attacker);
                return;
            }

            StartRunEntityCoroutine();
        }

        void BreakShield()
        {
            DisplayFloatingText("Shield broken", _shieldColor);
            IsShielded = false;
            OnShieldBroken?.Invoke();
        }

        public void TriggerDieCoroutine(UnitController attacker = null)
        {
            IsDead = true;
            if (gameObject.activeInHierarchy)
                StartCoroutine(Die(attacker: attacker));
        }

        public virtual IEnumerator Die(UnitController attacker = null, bool hasLoot = true)
        {
            if (IsDeathCoroutineStarted) yield break;
            IsDeathCoroutineStarted = true;

            StopRunEntityCoroutine();
            if (_isGrabbed) BattleGrabManager.Instance.CancelGrabbing();
            Collider.enabled = false;
            DOTween.Kill(transform);

            if (DeathSound != null) AudioManager.PlaySFX(DeathSound, transform.position);
            _deathEffect.SetActive(true);

            if (hasLoot) ResolveLoot();

            UnitLog.Add($"{BattleManager.GetTime()}: Unit dies.");
            OnDeath?.Invoke(this, attacker);
        }

        void ResolveLoot()
        {
            if (Team == 0) return;
            _pickupManager.SpawnExpOrb(transform.position);
        }

        public IEnumerator GetPoisoned(CreatureController attacker)
        {
            if (_isPoisoned) yield break;
            if (IsDead) yield break;
            UnitLog.Add($"{BattleManager.GetTime()}: Unit gets poisoned by {attacker.name}.");

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

        IEnumerator Celebrate()
        {
            if (IsDead) yield break;

            StopRunEntityCoroutine();
            Camera cam = Camera.main;
            if (cam != null)
                yield return transform.DODynamicLookAt(cam.transform.position, 0.2f).WaitForCompletion();
            Animator.SetBool(AnimCelebrate, true);
        }

        /* grab */
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            AudioManager.PlaySFX(_spawnSound, transform.position);

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
            StopRunEntityCoroutine();
        }

        public void Released()
        {
            _isGrabbed = false;
            Animator.enabled = true;
            StartRunEntityCoroutine();
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
            Vector3 pos = HeroController.transform.position
                          + Vector3.right * Random.Range(-10f, 10f)
                          + Vector3.forward * Random.Range(-10f, 10f);
            if (!NavMesh.SamplePosition(pos, out NavMeshHit _, 1f, NavMesh.AllAreas))
                return GetPositionCloseToHero();
            return pos;
        }
    }
}