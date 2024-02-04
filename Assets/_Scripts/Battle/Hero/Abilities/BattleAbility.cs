using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleAbility : MonoBehaviour
    {
        protected BattleManager BattleManager;
        protected BattleAreaManager BattleAreaManager;

        protected Transform AbilityObjectParent;

        [FormerlySerializedAs("_abilityObjectPrefab")] [SerializeField]
        protected GameObject AbilityObjectPrefab;

        readonly List<BattleAbilityObject> _abilityObjectPool = new();

        protected Ability Ability;
        IEnumerator _runAbilityCoroutine;
        IEnumerator _fireAbilityCoroutine;

        public event Action<Vector3, Vector3> OnAbilityFire;

        public virtual void Initialize(Ability ability, bool startAbility = true)
        {
            BattleManager = BattleManager.Instance;
            BattleAreaManager = BattleManager.GetComponent<BattleAreaManager>();
            AbilityObjectParent = BattleManager.AbilityHolder;

            Ability = ability;

            Ability.OnStart += StartAbility;
            Ability.OnStop += StopAbility;

            if (startAbility) StartAbility();
        }

        void StartAbility()
        {
            _runAbilityCoroutine = RunAbilityCoroutine();
            StartCoroutine(_runAbilityCoroutine);
        }

        void StopAbility()
        {
            StopCoroutine(_runAbilityCoroutine);
        }

        IEnumerator RunAbilityCoroutine()
        {
            yield return new WaitForSeconds(0.1f); // time to initialize button
            while (true)
            {
                _fireAbilityCoroutine = ExecuteAbilityCoroutine();
                StartCoroutine(_fireAbilityCoroutine);

                if (Ability.GetCooldown() == 0) yield break; // for continuous abilities
                Ability.StartCooldown();
                yield return new WaitForSeconds(Ability.GetCooldown());
            }
        }

        protected virtual IEnumerator ExecuteAbilityCoroutine()
        {
            Transform t = transform;
            OnAbilityFire?.Invoke(t.position, t.rotation.eulerAngles);
            // override this method in child classes
            yield return null;
        }

        protected Vector3 GetRandomEnemyDirection()
        {
            Vector3 rand = BattleManager.GetRandomEnemyPosition() - transform.position;
            rand.y = 0;
            Vector3 dir = rand.normalized;
            
            if (rand != Vector3.zero) return dir;
            rand = Random.insideUnitCircle;
            dir = new(rand.x, 0, rand.y);

            return dir;
        }

        BattleAbilityObject InitializeAbilityObject()
        {
            GameObject instance = Instantiate(AbilityObjectPrefab, Vector3.zero,
                Quaternion.identity, AbilityObjectParent);
            BattleAbilityObject ab = instance.GetComponent<BattleAbilityObject>();
            ab.Initialize(Ability);
            _abilityObjectPool.Add(ab);
            return ab;
        }

        protected BattleAbilityObject GetInactiveAbilityObject()
        {
            foreach (BattleAbilityObject p in _abilityObjectPool)
                if (!p.gameObject.activeSelf)
                    return p;
            return InitializeAbilityObject();
        }
    }
}