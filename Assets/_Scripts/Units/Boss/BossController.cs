using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Arena;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units.Boss
{
    public class BossController : UnitController
    {
        [Header("Boss")]
        [SerializeField]
        GameObject _stunEffect;

        [SerializeField] Sound _stunStart;
        [SerializeField] Sound _stunDuration;
        [HideInInspector] public FloatVariable TotalDamageToStun;
        [HideInInspector] public FloatVariable CurrentDamageToStun;

        [HideInInspector] public FloatVariable TotalStunDuration;
        [HideInInspector] public FloatVariable CurrentStunDuration;

        [Header("Attacks")] readonly List<Attack.Attack> _attacks = new();
        IEnumerator _attackCoroutine;
        bool _isStunned;
        bool _isStunUnlocked;
        SoundEmitter _stunAudioSource;

        Color _stunColor;

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

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            UnitPathingController.SetAvoidancePriorityRange(new(0, 1));
            Boss b = (Boss)Unit;
            float newSpeed = b.Speed.GetValue() - GameManager.UpgradeBoard.GetUpgradeByName("Boss Slowdown").GetValue();
            if (newSpeed <= 0) newSpeed = 1f;
            UnitPathingController.SetSpeed(newSpeed);

            TooltipManager.Instance.ShowBossHealthBar(this);

            InitializeAttacks();
            RunUnit();
            // StartAttackCoroutine();
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            yield return new WaitForSeconds(2f);

            while (true)
            {
                if (_isStunned) yield break;
                // yield return UnitPathingController.PathToPositionAndStop(GetPositionCloseToHero());

                yield return new WaitForSeconds(Random.Range(5, 10));
            }
        }

        // protected Vector3 GetPositionCloseToHero()
        // {
        //     Vector3 pos = HeroManager.transform.position
        //                   + Vector3.right * Random.Range(-10f, 10f)
        //                   + Vector3.forward * Random.Range(-10f, 10f);
        //     if (!NavMesh.SamplePosition(pos, out NavMeshHit _, 1f, NavMesh.AllAreas))
        //         return GetPositionCloseToHero();
        //     return pos;
        // }


        /* ATTACKS */
        void InitializeAttacks()
        {
            Boss boss = (Boss)Unit;
            foreach (Attack.Attack original in boss.Attacks)
            {
                Attack.Attack attack = Instantiate(original);
                // attack.InitializeAttack(this.Unit);
                _attacks.Add(attack);
            }
        }

        // void StartAttackCoroutine()
        // {
        //     _attackCoroutine = AttackCoroutine();
        //     StartCoroutine(_attackCoroutine);
        // }

        // IEnumerator AttackCoroutine()
        // {
        //     while (true)
        //     {
        //         foreach (Units.Attack.Attack t in _attacks)
        //         {
        //             if (_isStunned) yield break;
        //             yield return AttackCooldownCoroutine(t);
        //             yield return t.AttackController.Execute();
        //         }
        //     }
        // }
        //
        // IEnumerator AttackCooldownCoroutine(Attack.Attack attack)
        // {
        //     for (int i = 0; i < attack.CooldownSeconds; i++)
        //     {
        //         while (_isStunned) yield return new WaitForSeconds(1f);
        //         yield return new WaitForSeconds(1f);
        //     }
        // }


        /* GET HIT
        public override void BaseGetHit(int dmg, Color color, UnitController attacker = null)
        {
            if (_isStunned) dmg *= 2;

            UnitLog.Add($"{BattleManager.GetTime()}: Unit takes damage {dmg}");

            AudioManager.PlaySound("Hit", transform.position);

            DisplayFloatingText(dmg.ToString(), color);

            int d = Mathf.FloorToInt(Mathf.Clamp(dmg, 0, Unit.CurrentHealth.Value));
            Unit.CurrentHealth.ApplyChange(-d);
            if (Unit.CurrentHealth.Value <= 0)
            {
                Die(attacker);
                return;
            }

            HandleStun(d);
        }
        */

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
            Vector3 pos = transform.position;

            AudioManager.CreateSound()
                .WithSound(_stunStart)
                .WithPosition(pos)
                .Play();
            _stunAudioSource = AudioManager.CreateSound()
                .WithSound(_stunDuration)
                .WithPosition(pos)
                .Play();

            DisplayFloatingText("Stunned", _stunColor);
            CurrentStunDuration.SetValue(TotalStunDuration.Value);

            _stunEffect.SetActive(true);
            _isStunned = true;
            Animator.enabled = false;
            StopUnit();

            for (int i = 0; i < TotalStunDuration.Value; i++)
            {
                yield return new WaitForSeconds(1f);
                CurrentStunDuration.ApplyChange(-1);
            }

            if (_stunAudioSource != null)
            {
                _stunAudioSource.Stop();
                _stunAudioSource = null;
            }

            _stunEffect.SetActive(false);
            Animator.enabled = true;
            RunUnit();
            _isStunned = false;
            OnStunFinished?.Invoke();
        }

        /* HELPERS */
        void SetUpVariables()
        {
            TotalDamageToStun = ScriptableObject.CreateInstance<FloatVariable>();
            TotalDamageToStun.SetValue(666); //HERE: 666
            CurrentDamageToStun = ScriptableObject.CreateInstance<FloatVariable>();
            CurrentDamageToStun.SetValue(0);

            TotalStunDuration = ScriptableObject.CreateInstance<FloatVariable>();
            TotalStunDuration.SetValue(10 + GameManager.UpgradeBoard.GetUpgradeByName("Boss Stun").GetValue());
            CurrentStunDuration = ScriptableObject.CreateInstance<FloatVariable>();
            CurrentStunDuration.SetValue(0);
        }

        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            StopUnit();
            FightManager.WinArena();

            yield return null;
        }


        /* EMPTY OVERRIDES */
        public override void GetEngaged(UnitController attacker)
        {
            // boss is never engaged
            // all the single bosses, all the single bosses...
        }
    }
}