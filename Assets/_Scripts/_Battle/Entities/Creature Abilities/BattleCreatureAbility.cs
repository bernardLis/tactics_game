using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleCreatureAbility : MonoBehaviour
    {
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;

        [HideInInspector] public BattleCreature BattleCreature;
        protected Creature Creature;

        protected Animator Animator;
        protected Collider Collider;

        protected CreatureAbility CreatureAbility;

        float _currentAbilityCooldown;

        protected readonly int AnimAbility = Animator.StringToHash("Creature Ability");
        bool _isInitialized;

        IEnumerator _abilityCooldownCoroutine;
        public event Action OnCooldownEnd;

        public virtual void Initialize(BattleCreature battleCreature)
        {
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;

            BattleCreature = battleCreature;
            Creature = battleCreature.Creature;

            Animator = battleCreature.GetComponentInChildren<Animator>();
            Collider = battleCreature.GetComponent<Collider>();

            CreatureAbility = Creature.CreatureAbility;
            ResolveAbilityExecution();
            StartAbilityCooldownCoroutine();
        }

        void ResolveAbilityExecution()
        {
            if (CreatureAbility.ExecuteOnCooldown)
                OnCooldownEnd += ExecuteAbility;

            if (CreatureAbility.ExecuteOnAttack)
                BattleCreature.OnAttackReady += ExecuteAbility;

            if (CreatureAbility.ExecuteOnMove)
                BattleCreature.OnStartedMoving += ExecuteAbility;

            if (CreatureAbility.ExecuteOnDeath)
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

            _currentAbilityCooldown = CreatureAbility.Cooldown;
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