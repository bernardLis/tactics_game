using System.Collections;
using System.Collections.Generic;
using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis.Units.Hero.Ability
{
    public class Controller : MonoBehaviour
    {
        [FormerlySerializedAs("_abilityObjectPrefab")] [SerializeField]
        protected GameObject AbilityObjectPrefab;

        public Ability Ability;

        readonly List<ObjectController> _abilityObjectPool = new();
        FightManager _fightManager;
        IEnumerator _fireAbilityCoroutine;
        HeroManager _heroManager;
        IEnumerator _runAbilityCoroutine;

        protected Transform AbilityObjectParent;

        Camera _cam;
        Mouse _mouse;
        protected ArenaManager ArenaManager;
        protected AudioManager AudioManager;
        protected BattleManager BattleManager;

        protected HeroController HeroController;
        protected OpponentTracker OpponentTracker;

        public virtual void Initialize(Ability ability)
        {
            _cam = Camera.main;
            _mouse = Mouse.current;

            AudioManager = AudioManager.Instance;
            BattleManager = BattleManager.Instance;
            ArenaManager = BattleManager.GetComponent<ArenaManager>();
            _heroManager = BattleManager.GetComponent<HeroManager>();
            _fightManager = BattleManager.GetComponent<FightManager>();

            AbilityObjectParent = BattleManager.AbilityHolder;

            HeroController = _heroManager.HeroController;
            OpponentTracker = HeroController.GetComponentInChildren<OpponentTracker>();

            Ability = ability;

            _fightManager.OnFightEnded += StopAbility;
            if (FightManager.IsFightActive)
                StartAbility();
        }

        public void StartAbility()
        {
            StopAbility();
            _runAbilityCoroutine = RunAbilityCoroutine();
            StartCoroutine(_runAbilityCoroutine);
        }

        public void StopAbility()
        {
            if (_runAbilityCoroutine != null) StopCoroutine(_runAbilityCoroutine);
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
            if (_fightManager.EnemyUnits.Count == 0)
            {
                rand = Random.insideUnitCircle;
                rand = new(rand.x, 0, rand.y);
                return rand.normalized;
            }

            rand = _fightManager.GetRandomEnemyPosition() - transform.position;
            rand.y = 0;
            return rand.normalized;
        }

        protected Vector3 GetPositionTowardsCursor()
        {
            Vector3 dir = Vector3.zero;
            Ray ray = _cam.ScreenPointToRay(_mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                dir = (hit.point - transform.position).normalized;
            dir.y = 0;
            return dir;
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

        void DisableAllAbilityObjects()
        {
            foreach (ObjectController p in _abilityObjectPool)
                p.DisableSelf();
        }
    }
}