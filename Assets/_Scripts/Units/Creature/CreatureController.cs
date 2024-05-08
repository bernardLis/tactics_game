using System.Collections;
using DG.Tweening;
using Lis.Units.Creature.Ability;
using UnityEngine;

namespace Lis.Units.Creature
{
    public class CreatureController : UnitController
    {
        public Creature Creature { get; private set; }

        [SerializeField] GameObject _respawnEffect;
        Controller _abilityController;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);

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