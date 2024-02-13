using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleEntity : MonoBehaviour, IGrabbable, IPointerDownHandler
    {
        protected GameManager GameManager;
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;
        BattleGrabManager _grabManager;
        BattlePickupManager _pickupManager;

        protected ObjectShaders BattleEntityShaders;

        public List<string> EntityLog = new();

        [Header("Sounds")] [SerializeField] Sound _spawnSound;

        [FormerlySerializedAs("_deathSound")] [SerializeField]
        protected Sound DeathSound;

        [FormerlySerializedAs("_getHitSound")] [SerializeField]
        protected Sound GetHitSound;

        [SerializeField] GameObject _levelUpEffect;
        [SerializeField] GameObject _deathEffect;

        public Collider Collider { get; private set; }

        string BattleId { get; set; }
        public int Team { get; private set; }

        protected GameObject Gfx;
        protected Animator Animator { get; private set; }

        public Entity Entity { get; private set; }

        protected NavMeshAgent Agent;
        protected Vector2Int AvoidancePriorityRange = new Vector2Int(20, 100);

        [HideInInspector] public bool IsShielded;
        protected bool IsEngaged;
        bool _isPoisoned;
        bool _isGrabbed;

        public bool IsDead { get; protected set; }
        protected bool IsDeathCoroutineStarted;

        MMF_Player _feelPlayer;

        protected IEnumerator CurrentMainCoroutine;
        protected IEnumerator CurrentSecondaryCoroutine;
        protected IEnumerator CurrentAbilityCoroutine;

        static readonly int AnimMove = Animator.StringToHash("Move");
        static readonly int AnimTakeDamage = Animator.StringToHash("Take Damage");
        private static readonly int AnimCelebrate = Animator.StringToHash("Celebrate");

        public event Action<int> OnDamageTaken;
        public event Action<BattleEntity, BattleEntity> OnDeath;

        public virtual void InitializeGameObject()
        {
            EntityLog.Add($"{Time.time}: (GAME TIME) Entity is instantiated.");

            GameManager = GameManager.Instance;
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            _grabManager = BattleGrabManager.Instance;
            _pickupManager = BattleManager.GetComponent<BattlePickupManager>();

            BattleEntityShaders = GetComponent<ObjectShaders>();
            Collider = GetComponent<Collider>();
            Animator = GetComponentInChildren<Animator>();
            Gfx = Animator.gameObject;
            _feelPlayer = GetComponent<MMF_Player>();
            Agent = GetComponent<NavMeshAgent>();

            BattleManager.OnBattleFinalized += () => StartCoroutine(Celebrate());
        }

        public virtual void InitializeEntity(Entity entity, int team)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Entity is initialized");


            if (_spawnSound != null)
                AudioManager.PlaySFX(_spawnSound, transform.position);

            _deathEffect.SetActive(false);
            Entity = entity;
            Team = team;
            entity.OnLevelUp += OnLevelUp;
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

            BattleId = Team + "_" + Helpers.ParseScriptableObjectName(Entity.name)
                       + "_" + Helpers.GetRandomNumber(4);
            name = BattleId;

            if (Entity is not EntityMovement em) return;
            Agent.speed = em.Speed.GetValue();
            em.Speed.OnValueChanged += (i) => Agent.speed = i;
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

        protected virtual void StartRunEntityCoroutine()
        {
            if (IsDead) return;

            StopRunEntityCoroutine();
            EntityLog.Add($"{BattleManager.GetTime()}: Start run entity coroutine is called");

            CurrentMainCoroutine = RunEntity();
            StartCoroutine(CurrentMainCoroutine);
        }

        protected virtual void StopRunEntityCoroutine()
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Stop run entity coroutine is called");

            if (CurrentMainCoroutine != null)
                StopCoroutine(CurrentMainCoroutine);
            if (CurrentSecondaryCoroutine != null)
                StopCoroutine(CurrentSecondaryCoroutine);
            if (CurrentAbilityCoroutine != null)
                StopCoroutine(CurrentAbilityCoroutine);

            if (Agent.isActiveAndEnabled) Agent.isStopped = true;
            Agent.enabled = false;

            Animator.SetBool(AnimMove, false);
        }

        protected virtual IEnumerator RunEntity()
        {
            yield return null;
        }

        protected IEnumerator PathToPosition(Vector3 position)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Path to position is called {position}");

            Agent.enabled = true;
            // TODO: pitiful solution for making entities push each other
            Agent.avoidancePriority = Random.Range(AvoidancePriorityRange.x, AvoidancePriorityRange.y);

            while (!Agent.SetDestination(position)) yield return null;
            while (Agent.pathPending) yield return null;

            Animator.SetBool(AnimMove, true);
        }

        protected IEnumerator PathToPositionAndStop(Vector3 position)
        {
            yield return PathToPosition(position);
            while (Agent.enabled && Agent.remainingDistance > Agent.stoppingDistance)
                yield return new WaitForSeconds(0.1f);

            StopWalking();
        }

        protected IEnumerator PathToTarget(Transform t)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Path to target is called {t}");

            yield return PathToPosition(t.position);
            while (Agent.enabled && Agent.remainingDistance > Agent.stoppingDistance)
            {
                if (t == null) yield break;
                Agent.SetDestination(t.position);
                yield return new WaitForSeconds(0.1f);
            }

            StopWalking();
        }

        void StopWalking()
        {
            Agent.avoidancePriority = 0;
            Animator.SetBool(AnimMove, false);
            Agent.enabled = false;
        }

        public virtual void GetEngaged(BattleEntity attacker)
        {
            if (IsEngaged) return;
            IsEngaged = true;

            EntityLog.Add($"{BattleManager.GetTime()}: Entity gets engaged by {attacker.name}");
            StopRunEntityCoroutine();
            Invoke(nameof(Disengage), Random.Range(2f, 4f));
        }

        public void Disengage()
        {
            if (IsDead) return;

            IsEngaged = false;
            EntityLog.Add($"{BattleManager.GetTime()}: Entity disengages");
            StartRunEntityCoroutine();
        }

        public bool HasFullHealth()
        {
            return Entity.CurrentHealth.Value >= Entity.MaxHealth.GetValue();
        }

        public void GetHealed(int value)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Entity gets healed by {value}");

            int v = Mathf.Clamp(value, 0, Entity.MaxHealth.GetValue() - Entity.CurrentHealth.Value);
            Entity.CurrentHealth.ApplyChange(v);

            DisplayFloatingText("+" + v, Color.green);
        }

        public virtual IEnumerator GetHit(Ability ability)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;
            EntityLog.Add($"{BattleManager.GetTime()}: Entity gets attacked by {ability.name}");

            BaseGetHit(Entity.CalculateDamage(ability), ability.Element.Color.Secondary);

            if (Entity.CurrentHealth.Value <= 0)
                ability.AddKill();
        }

        public virtual IEnumerator GetHit(BattleEntity attacker, int specialDamage = 0)
        {
            if (IsDead) yield break;
            if (BattleManager == null) yield break;
            EntityLog.Add($"{BattleManager.GetTime()}: Entity gets attacked by {attacker.name}");

            int damage = Entity.CalculateDamage(attacker.Entity as EntityFight);
            if (specialDamage > 0) damage = specialDamage;
            if (attacker.Entity is not EntityFight attackerFight) yield break;
            attackerFight.AddDmgDealt(damage);

            BaseGetHit(damage, attackerFight.Element.Color.Primary, attacker);

            if (Entity.CurrentHealth.Value <= 0)
                attackerFight.AddKill(Entity);
        }

        public virtual void BaseGetHit(int dmg, Color color, BattleEntity attacker = null)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Entity takes damage {dmg}");
            StopRunEntityCoroutine();

            if (GetHitSound != null) AudioManager.PlaySFX(GetHitSound, transform.position);
            else AudioManager.PlaySFX("Hit", transform.position);

            Animator.SetTrigger(AnimTakeDamage);
            DisplayFloatingText(dmg.ToString(), color);

            OnDamageTaken?.Invoke(dmg);

            if (Entity == null) return;
            int d = Mathf.Clamp(dmg, 0, Entity.CurrentHealth.Value);
            Entity.CurrentHealth.ApplyChange(-d);
            if (Entity.CurrentHealth.Value <= 0)
            {
                TriggerDieCoroutine(attacker);
                return;
            }

            StartRunEntityCoroutine();
        }

        public void TriggerDieCoroutine(BattleEntity attacker = null)
        {
            IsDead = true;
            if (gameObject.activeInHierarchy)
                StartCoroutine(Die(attacker: attacker));
        }

        public virtual IEnumerator Die(BattleEntity attacker = null, bool hasLoot = true)
        {
            if (IsDeathCoroutineStarted) yield break;
            IsDeathCoroutineStarted = true;

            StopRunEntityCoroutine();

            if (_isGrabbed) BattleGrabManager.Instance.CancelGrabbing();
            if (Agent.isActiveAndEnabled) Agent.isStopped = true;

            Collider.enabled = false;

            if (DeathSound != null) AudioManager.PlaySFX(DeathSound, transform.position);

            _deathEffect.SetActive(true);

            DOTween.Kill(transform);
            if (hasLoot) ResolveLoot();

            EntityLog.Add($"{BattleManager.GetTime()}: Entity dies.");
            OnDeath?.Invoke(this, attacker);
        }

        void ResolveLoot()
        {
            if (Team == 0) return;
            _pickupManager.SpawnExpOrb(transform.position);
        }

        public IEnumerator GetPoisoned(BattleCreature attacker)
        {
            if (_isPoisoned) yield break;
            if (IsDead) yield break;
            EntityLog.Add($"{BattleManager.GetTime()}: Entity gets poisoned by {attacker.name}.");

            _isPoisoned = true;
            DisplayFloatingText("Poisoned", Color.green);

            // TODO: for now hardcoded
            int totalDamage = 20;
            int damageTick = 5;

            while (totalDamage > 0)
            {
                if (IsDead) break;

                // poison can't kill
                if (Entity.CurrentHealth.Value > damageTick)
                {
                    DisplayFloatingText(damageTick.ToString(), Color.green);
                    attacker.DealtDamage(damageTick);
                    OnDamageTaken?.Invoke(damageTick);
                    Entity.CurrentHealth.ApplyChange(-damageTick);
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
    }
}