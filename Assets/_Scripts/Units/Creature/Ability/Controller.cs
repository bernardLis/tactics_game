using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Units.Attack;
using UnityEngine;

namespace Lis.Units.Creature.Ability
{
    public class Controller : MonoBehaviour
    {
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;
        protected FightManager FightManager;

        [HideInInspector] public CreatureController CreatureController;
        protected AttackController AttackController;
        protected UnitPathingController UnitPathingController;

        protected Creature Creature;

        protected Animator Animator;
        protected Collider Collider;

        protected Ability Ability;

        float _currentAbilityCooldown;

        protected readonly int AnimAbility = Animator.StringToHash("Creature Ability");
        bool _isInitialized;

        IEnumerator _abilityCooldownCoroutine;
        public event Action OnCooldownEnd;

        public virtual void Initialize(CreatureController creatureController)
        {
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            FightManager = BattleManager.GetComponent<FightManager>();

            CreatureController = creatureController;
            AttackController = creatureController.GetComponent<AttackController>();
            UnitPathingController = creatureController.GetComponent<UnitPathingController>();
            Creature = creatureController.Creature;

            Animator = creatureController.GetComponentInChildren<Animator>();
            Collider = creatureController.GetComponent<Collider>();

            Ability = Creature.Ability;
            ResolveAbilityExecution();
            StartAbilityCooldownCoroutine();
        }

        void ResolveAbilityExecution()
        {
            if (Ability.ExecuteOnCooldown)
                OnCooldownEnd += ExecuteAbility;

            if (Ability.ExecuteOnAttack)
                AttackController.OnAttackReady += ExecuteAbility;

            if (Ability.ExecuteOnMove)
                UnitPathingController.OnStartedMoving += ExecuteAbility;

            if (Ability.ExecuteOnDeath)
                CreatureController.OnDeath += ExecuteAbilityOnDeath;
        }

        public void StartAbilityCooldownCoroutine()
        {
            if (_abilityCooldownCoroutine != null)
                StopCoroutine(_abilityCooldownCoroutine);

            _abilityCooldownCoroutine = AbilityCooldownCoroutine();
            StartCoroutine(_abilityCooldownCoroutine);
        }

        IEnumerator AbilityCooldownCoroutine()
        {
            _currentAbilityCooldown = Ability.Attack.Cooldown;
            while (_currentAbilityCooldown > 0)
            {
                _currentAbilityCooldown -= 1;
                yield return new WaitForSeconds(1);
            }

            OnCooldownEnd?.Invoke();
        }

        void ExecuteAbilityOnDeath(UnitController _, Attack.Attack __)
        {
            StartCoroutine(ExecuteAbilityCoroutine());
        }

        void ExecuteAbility()
        {
            if (_currentAbilityCooldown > 0) return;
            StartCoroutine(ExecuteAbilityCoroutine());
        }

        protected virtual IEnumerator ExecuteAbilityCoroutine()
        {
            // meant to be overwritten and it goes at the end of ability
            Creature.Ability.Used();
            CreatureController.RunUnit();
            StartAbilityCooldownCoroutine();
            yield return null;
        }

        protected List<UnitController> GetOpponentsInRadius(float radius)
        {
            List<UnitController> opponents = new List<UnitController>();
            Collider[] colliders = new Collider[25];
            Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
            foreach (Collider c in colliders)
            {
                if (c == null) continue;
                if (c.TryGetComponent(out BreakableVaseController bbv))
                {
                    bbv.TriggerBreak();
                    continue;
                }

                if (!c.TryGetComponent(out UnitController entity)) continue;
                if (entity.Team == Creature.Team) continue;
                if (entity.IsDead) continue;
                opponents.Add(entity);
            }

            return opponents;
        }
    }
}