using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbility : MonoBehaviour
    {
        [HideInInspector] public BattleCreature BattleCreature;
        protected Creature Creature;

        protected Animator Animator;
        protected Collider Collider;

        CreatureAbility _creatureAbility;

        float _currentAbilityCooldown;

        protected readonly int AnimAbility = Animator.StringToHash("Creature Ability");

        public virtual void Initialize(BattleCreature battleCreature)
        {
            BattleCreature = battleCreature;
            Creature = battleCreature.Creature;

            Animator = battleCreature.GetComponentInChildren<Animator>();
            Collider = battleCreature.GetComponent<Collider>();

            _creatureAbility = Creature.CreatureAbility;

            ResolveAbilityExecution();
        }

        void ResolveAbilityExecution()
        {
            if (_creatureAbility.ExecuteOnCooldown)
                StartCoroutine(ExecuteAbilityCoroutine());

            if (_creatureAbility.ExecuteOnAttack)
                BattleCreature.OnAttackReady += ExecuteAbility;

            if (_creatureAbility.ExecuteOnMove)
                BattleCreature.OnStartedMoving += ExecuteAbility;

            if (_creatureAbility.ExecuteOnDeath)
                BattleCreature.OnDeath += ExecuteAbilityOnDeath;
        }

        IEnumerator AbilityCooldownCoroutine()
        {
            while (_currentAbilityCooldown >= 0)
            {
                _currentAbilityCooldown -= 1;
                yield return new WaitForSeconds(1);
            }

            if (_creatureAbility.ExecuteOnCooldown)
                StartCoroutine(ExecuteAbilityCoroutine());
        }

        void ExecuteAbilityOnDeath(BattleEntity _, BattleEntity __)
        {
            StartCoroutine(ExecuteAbilityCoroutine());
        }

        protected virtual void ExecuteAbility()
        {
            if (_currentAbilityCooldown > 0) return;
            StartCoroutine(ExecuteAbilityCoroutine());
        }

        protected virtual IEnumerator ExecuteAbilityCoroutine()
        {
            // meant to be overwritten and it goes at the end of ability
            Creature.CreatureAbility.Used();
            _currentAbilityCooldown = _creatureAbility.Cooldown;
            BattleCreature.StartRunEntityCoroutine();
            StartCoroutine(AbilityCooldownCoroutine());
            yield return null;
        }
    }
}