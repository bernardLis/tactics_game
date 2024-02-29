using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleBoss : BattleEntity
    {
        [Header("Boss")]
        [SerializeField] GameObject _stunEffect;

        [Header("Attacks")] readonly List<BossAttack> _attacks = new();
        IEnumerator _attackCoroutine;

        Color _stunColor;
        bool _isStunUnlocked;
        bool _isStunned;
        [HideInInspector] public FloatVariable TotalDamageToStun;
        [HideInInspector] public FloatVariable CurrentDamageToStun;

        [HideInInspector] public FloatVariable TotalStunDuration;
        [HideInInspector] public FloatVariable CurrentStunDuration;


        public event Action OnStunStarted;
        public event Action OnStunFinished;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();

            _isStunUnlocked =
                GameManager.UpgradeBoard.GetUpgradeByName("Boss Stun").CurrentLevel >= 0;
            SetUpVariables();

            _stunColor = GameManager.GameDatabase.GetColorByName("Stun").Primary;
        }

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, team);

            BattleEntityPathing.SetAvoidancePriorityRange(new(0, 1));
            Boss b = (Boss)Entity;
            float newSpeed = b.Speed.GetValue() - GameManager.UpgradeBoard.GetUpgradeByName("Boss Slowdown").GetValue();
            if (newSpeed <= 0) newSpeed = 1f;
            BattleEntityPathing.SetSpeed(newSpeed);

            BattleManager.GetComponent<BattleTooltipManager>().ShowBossHealthBar(this);

            InitializeAttacks();
            StartRunEntityCoroutine();
            StartAttackCoroutine();
        }

        protected override IEnumerator RunEntity()
        {
            yield return new WaitForSeconds(2f);

            while (true)
            {
                if (_isStunned) yield break;
                yield return BattleEntityPathing.PathToPositionAndStop(GetPositionCloseToHero());

                yield return new WaitForSeconds(Random.Range(5, 10));
            }
        }

        /* ATTACKS */
        void InitializeAttacks()
        {
            Boss boss = (Boss)Entity;
            foreach (BossAttack original in boss.Attacks)
            {
                BossAttack attack = Instantiate(original);
                attack.Initialize(this);
                _attacks.Add(attack);
            }
        }

        void StartAttackCoroutine()
        {
            _attackCoroutine = AttackCoroutine();
            StartCoroutine(_attackCoroutine);
        }

        IEnumerator AttackCoroutine()
        {
            while (true)
            {
                foreach (BossAttack t in _attacks)
                {
                    if (_isStunned) yield break;
                    yield return AttackCooldownCoroutine(t);
                    yield return t.BattleBossAttack.Attack();
                }
            }
        }

        IEnumerator AttackCooldownCoroutine(BossAttack bossAttack)
        {
            for (int i = 0; i < bossAttack.CooldownSeconds; i++)
            {
                while (_isStunned) yield return new WaitForSeconds(1f);
                yield return new WaitForSeconds(1f);
            }
        }


        /* GET HIT */
        public override void BaseGetHit(int dmg, Color color, BattleEntity attacker = null)
        {
            if (_isStunned) dmg *= 2;

            EntityLog.Add($"{BattleManager.GetTime()}: Entity takes damage {dmg}");

            if (GetHitSound != null) AudioManager.PlaySFX(GetHitSound, transform.position);
            else AudioManager.PlaySFX("Hit", transform.position);

            DisplayFloatingText(dmg.ToString(), color);

            int d = Mathf.FloorToInt(Mathf.Clamp(dmg, 0, Entity.CurrentHealth.Value));
            Entity.CurrentHealth.ApplyChange(-d);
            if (Entity.CurrentHealth.Value <= 0)
            {
                TriggerDieCoroutine(attacker);
                return;
            }

            HandleStun(d);
        }

        void HandleStun(int dmg)
        {
            if (!_isStunUnlocked) return;
            if (_isStunned) return;

            CurrentDamageToStun.ApplyChange(dmg);
            if (CurrentDamageToStun.Value < TotalDamageToStun.Value) return;

            CurrentDamageToStun.SetValue(0);
            StartCoroutine(StunCoroutine());
        }


        IEnumerator StunCoroutine()
        {
            OnStunStarted?.Invoke();
            DisplayFloatingText("Stunned", _stunColor);
            CurrentStunDuration.SetValue(TotalStunDuration.Value);

            _stunEffect.SetActive(true);
            _isStunned = true;
            Animator.enabled = false;
            StopRunEntityCoroutine();

            for (int i = 0; i < TotalStunDuration.Value; i++)
            {
                yield return new WaitForSeconds(1f);
                CurrentStunDuration.ApplyChange(-1);
            }

            _stunEffect.SetActive(false);
            Animator.enabled = true;
            StartRunEntityCoroutine();
            _isStunned = false;
            OnStunFinished?.Invoke();
        }

        /* HELPERS */
        void SetUpVariables()
        {
            TotalDamageToStun = ScriptableObject.CreateInstance<FloatVariable>();
            TotalDamageToStun.SetValue(100); //HERE: 666
            CurrentDamageToStun = ScriptableObject.CreateInstance<FloatVariable>();
            CurrentDamageToStun.SetValue(0);

            TotalStunDuration = ScriptableObject.CreateInstance<FloatVariable>();
            TotalStunDuration.SetValue(10 + GameManager.UpgradeBoard.GetUpgradeByName("Boss Stun").GetValue());
            CurrentStunDuration = ScriptableObject.CreateInstance<FloatVariable>();
            CurrentStunDuration.SetValue(0);
        }

        /* EMPTY OVERRIDES */
        public override void GetEngaged(BattleEntity attacker)
        {
            // boss is never engaged
            // all the single bosses, all the single bosses...
        }

        public override bool CanBeGrabbed()
        {
            return false;
        }
    }
}