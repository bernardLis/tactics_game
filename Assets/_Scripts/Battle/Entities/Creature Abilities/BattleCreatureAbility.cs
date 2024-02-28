using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbility : MonoBehaviour
    {
        protected BattleManager BattleManager;

        [HideInInspector] public BattleCreature BattleCreature;
        protected Creature Creature;

        protected Animator Animator;
        protected Collider Collider;

        CreatureAbility _creatureAbility;

        float _currentAbilityCooldown;

        protected readonly int AnimAbility = Animator.StringToHash("Creature Ability");
        bool _isInitialized;

        IEnumerator _abilityCooldownCoroutine;
        public event Action OnCooldownEnd;

        public virtual void Initialize(BattleCreature battleCreature)
        {
            BattleManager = BattleManager.Instance;

            BattleCreature = battleCreature;
            Creature = battleCreature.Creature;

            Animator = battleCreature.GetComponentInChildren<Animator>();
            Collider = battleCreature.GetComponent<Collider>();

            _creatureAbility = Creature.CreatureAbility;
            ResolveAbilityExecution();
            StartAbilityCooldownCoroutine();
        }

        void ResolveAbilityExecution()
        {
            if (_creatureAbility.ExecuteOnCooldown)
                OnCooldownEnd += ExecuteAbility;

            if (_creatureAbility.ExecuteOnAttack)
                BattleCreature.OnAttackReady += ExecuteAbility;

            if (_creatureAbility.ExecuteOnMove)
                BattleCreature.OnStartedMoving += ExecuteAbility;

            if (_creatureAbility.ExecuteOnDeath)
                BattleCreature.OnDeath += ExecuteAbilityOnDeath;
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
            Debug.Log("Ability cooldown started");

            _currentAbilityCooldown = _creatureAbility.Cooldown;
            while (_currentAbilityCooldown > 0)
            {
                _currentAbilityCooldown -= 1;
                yield return new WaitForSeconds(1);
            }

            Debug.Log($"Ability cooldown ended {_currentAbilityCooldown}");
            OnCooldownEnd?.Invoke();
        }

        void ExecuteAbilityOnDeath(BattleEntity _, BattleEntity __)
        {
            StartCoroutine(ExecuteAbilityCoroutine());
        }

        protected virtual void ExecuteAbility()
        {
            Debug.Log($"Execute ability {_currentAbilityCooldown}");
            if (_currentAbilityCooldown > 0) return;
            StartCoroutine(ExecuteAbilityCoroutine());
        }

        protected virtual IEnumerator ExecuteAbilityCoroutine()
        {
            // meant to be overwritten and it goes at the end of ability
            Creature.CreatureAbility.Used();
            BattleCreature.StartRunEntityCoroutine();
            StartAbilityCooldownCoroutine();
            yield return null;
        }

        protected List<BattleEntity> GetOpponentsInRadius(float radius)
        {
            List<BattleEntity> opponents = new List<BattleEntity>();
            Collider[] colliders = new Collider[25];
            Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);
            foreach (Collider c in colliders)
            {
                if (c == null) continue;
                if (!c.TryGetComponent(out BattleEntity entity)) continue;
                if (entity.Team == Creature.Team) continue;
                if (entity.IsDead) continue;
                opponents.Add(entity);
            }

            return opponents;
        }
    }
}