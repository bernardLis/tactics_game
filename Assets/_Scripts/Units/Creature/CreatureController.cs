using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Battle.Fight;
using Lis.Units.Creature.Ability;
using UnityEngine;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        public Creature Creature { get; private set; }

        List<UnitController> _opponentList = new();

        [SerializeField] GameObject _respawnEffect;
        Controller _abilityController;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

            Opponent = null;
            if (team == 0) ObjectShaders.LitShader();

            Creature = (Creature)unit;
            Creature.OnLevelUp -= OnLevelUp; // just in case I initialize it twice :))))
            Creature.OnLevelUp += OnLevelUp;
        }

        protected override void EnableSelf()
        {
            base.EnableSelf();

            if (_abilityController != null)
                _abilityController.StartAbilityCooldownCoroutine();
        }


        public void SetOpponentList(ref List<UnitController> list)
        {
            _opponentList = list;
        }

        protected override void OnFightEnded()
        {
            if (this == null) return;
            if (Team == 1 && IsDead)
            {
                transform.DOMoveY(0f, 5f)
                    .OnComplete(DestroySelf);
                return;
            }

            StartCoroutine(OnFightEndedCoroutine());
        }

        IEnumerator OnFightEndedCoroutine()
        {
            StopUnit();
            AddToLog("Fight ended!");
            if (IsDead) yield return Respawn();
            Creature.CurrentHealth.SetValue(Creature.MaxHealth.GetValue());
            GoBackToLocker();
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            while (true)
            {
                if (IsDead) yield break;
                while (_opponentList.Count == 0)
                    yield return new WaitForSeconds(1f);

                Unit.ChooseAttack();
                CurrentSecondaryCoroutine = ManagePathing();
                yield return CurrentSecondaryCoroutine;
                CurrentSecondaryCoroutine = UnitAttackController.AttackCoroutine();
                yield return CurrentSecondaryCoroutine;
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
                Creature.CurrentAttack.Range);
            Opponent.GetEngaged(this); // otherwise, creature can't catch up
        }

        public override void GetEngaged(UnitController attacker)
        {
            if (IsEngaged) return;
            IsEngaged = true;

            AddToLog($"Creature gets engaged by {attacker.name}");
            Opponent = attacker;
            RunUnit();
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

        void ResetOpponent(UnitController _, Attack.Attack __)
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

        protected override void OnGrabbed()
        {
            base.OnGrabbed();
            ResetOpponent(default, default);
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

        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attack, hasLoot);
            StopUnit();
            UnitPathingController.DisableAgent();
            _respawnEffect.SetActive(false);

            Creature.Die();
            ResetOpponent(null, null);
        }

        IEnumerator Respawn()
        {
            AddToLog("Respawning...");
            Animator.Rebind();
            Animator.Update(0f);
            _respawnEffect.SetActive(true);
            EnableSelf();
            yield return new WaitForSeconds(1);
        }
    }
}