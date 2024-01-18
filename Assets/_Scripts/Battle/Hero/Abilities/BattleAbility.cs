using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace Lis
{
    public class BattleAbility : MonoBehaviour
    {
        protected BattleManager _battleManager;
        protected BattleAreaManager _battleAreaManager;

        protected Transform _abilityObjectParent;
        [SerializeField] protected GameObject _abilityObjectPrefab;
        List<BattleAbilityObject> _abilityObjectPool = new();

        protected Ability _ability;
        protected IEnumerator _runAbilityCoroutine;
        protected IEnumerator _fireAbilityCoroutine;

        public event Action<Vector3, Vector3> OnAbilityFire;
        public virtual void Initialize(Ability ability, bool startAbility = true)
        {
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
            _abilityObjectParent = _battleManager.AbilityHolder;

            _ability = ability;

            _ability.OnStart += StartAbility;
            _ability.OnStop += StopAbility;

            if (startAbility) StartAbility();
        }


        public void StartAbility()
        {
            _runAbilityCoroutine = RunAbilityCoroutine();
            StartCoroutine(_runAbilityCoroutine);
        }

        public void StopAbility()
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

                if (_ability.GetCooldown() == 0) yield break; // for continuous abilities
                _ability.StartCooldown();
                yield return new WaitForSeconds(_ability.GetCooldown());
            }
        }

        protected virtual IEnumerator ExecuteAbilityCoroutine()
        {
            OnAbilityFire?.Invoke(transform.position, transform.rotation.eulerAngles);
            // override this method in child classes
            yield return null;
        }

        protected BattleAbilityObject InitializeAbilityObject()
        {
            GameObject instance = Instantiate(_abilityObjectPrefab, Vector3.zero,
                Quaternion.identity, _abilityObjectParent);
            BattleAbilityObject ab = instance.GetComponent<BattleAbilityObject>();
            ab.Initialize(_ability);
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
