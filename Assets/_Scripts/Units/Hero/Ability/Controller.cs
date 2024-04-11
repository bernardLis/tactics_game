using System.Collections;
using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Ability
{
    public class Controller : MonoBehaviour
    {
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;
        protected HeroManager HeroManager;
        protected FightManager FightManager;
        protected ArenaManager ArenaManager;

        protected Transform AbilityObjectParent;

        [FormerlySerializedAs("_abilityObjectPrefab")] [SerializeField]
        protected GameObject AbilityObjectPrefab;

        readonly List<ObjectController> _abilityObjectPool = new();

        protected HeroController HeroController;
        protected OpponentTracker OpponentTracker;
        protected Ability Ability;
        IEnumerator _runAbilityCoroutine;
        IEnumerator _fireAbilityCoroutine;

        public virtual void Initialize(Ability ability, bool startAbility = true)
        {
            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            ArenaManager = BattleManager.GetComponent<ArenaManager>();
            HeroManager = BattleManager.GetComponent<HeroManager>();
            FightManager = BattleManager.GetComponent<FightManager>();

            AbilityObjectParent = BattleManager.AbilityHolder;

            HeroController = HeroManager.HeroController;
            OpponentTracker = HeroController.GetComponentInChildren<OpponentTracker>();

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
            // override this method in child classes
            yield return null;
        }

        protected Vector3 GetRandomEnemyDirection()
        {
            Vector3 rand;
            if (FightManager.EnemyUnits.Count == 0)
            {
                rand = Random.insideUnitCircle;
                rand = new(rand.x, 0, rand.y);
                return rand.normalized;
            }

            rand = FightManager.GetRandomEnemyPosition() - transform.position;
            rand.y = 0;
            return rand.normalized;
        }

        ObjectController InitializeAbilityObject()
        {
            GameObject instance = Instantiate(AbilityObjectPrefab, Vector3.zero,
                Quaternion.identity, AbilityObjectParent);
            ObjectController ab = instance.GetComponent<ObjectController>();
            ab.Initialize(Ability);
            _abilityObjectPool.Add(ab);
            return ab;
        }

        protected ObjectController GetInactiveAbilityObject()
        {
            foreach (ObjectController p in _abilityObjectPool)
                if (!p.gameObject.activeSelf)
                    return p;
            return InitializeAbilityObject();
        }
    }
}