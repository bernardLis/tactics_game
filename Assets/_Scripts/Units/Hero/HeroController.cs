using System.Collections;
using Lis.Units.Hero.Ability;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class HeroController : UnitController
    {
        BattleAreaManager _battleAreaManager;

        public Hero Hero { get; private set; }

        MovementController _thirdPersonMovementController;
        HealthBarDisplayer _healthBarDisplayer;

        [Header("Hero")]
        [SerializeField] GameObject _tileSecuredMarkerPrefab;


        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _thirdPersonMovementController = GetComponent<MovementController>();
            _healthBarDisplayer = GetComponentInChildren<HealthBarDisplayer>();
            _battleAreaManager = BattleManager.GetComponent<BattleAreaManager>();

            GetComponent<CreatureCatcher>().Initialize();

            _battleAreaManager.OnTileUnlocked += OnTileUnlocked;
        }

        void OnTileUnlocked(BattleTile tile)
        {
            if (tile == _battleAreaManager.HomeTile) return;

            Vector3 heroPos = transform.position;
            Vector3 tilePos = tile.transform.position;
            Vector3 dir = (tilePos - heroPos).normalized;
            Vector3 pos = heroPos + dir * 2;
            pos.y = 2f;

            GameObject instance = Instantiate(_tileSecuredMarkerPrefab);
            instance.transform.position = pos;
            instance.transform.rotation = Quaternion.LookRotation(heroPos - tilePos);
            instance.SetActive(true);

            Destroy(instance, 8f);
        }

        public override void InitializeEntity(Unit unit, int team)
        {
            base.InitializeEntity(unit, 0);
            gameObject.layer = 8;
            UnitPathingController.EnableAgent();

            Hero = (Hero)unit;

            _thirdPersonMovementController.SetMoveSpeed(Hero.Speed.GetValue());
            Hero.Speed.OnValueChanged += _thirdPersonMovementController.SetMoveSpeed;

            _healthBarDisplayer.Initialize(Hero);

            Animator.enabled = true;

            HandleAbilities();
        }

        void HandleAbilities()
        {
            Hero.OnAbilityAdded += AddAbility;

            foreach (Ability.Ability a in Hero.Abilities)
                AddAbility(a);
        }

        void OnDestroy()
        {
            Hero.OnAbilityAdded -= AddAbility;
        }

        void AddAbility(Ability.Ability ability)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Hero adds an ability {ability.name}");
            GameObject abilityPrefab = Instantiate(ability.AbilityManagerPrefab, transform);
            abilityPrefab.GetComponent<Controller>().Initialize(ability);
        }

        public override IEnumerator GetHit(UnitController attacker, int specialDamage = 0)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Hero gets hit by {attacker.name}");
            int damage = Unit.CalculateDamage(attacker.Unit as UnitFight);
            BaseGetHit(damage, HealthColor);
            yield return null;
        }

        public IEnumerator GetHit(Minion.Minion minion)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Hero gets hit by a minion {minion.name}");
            BaseGetHit(Hero.CalculateDamage(minion), HealthColor);
            yield return null;
        }

        public override IEnumerator Die(UnitController attacker = null, bool hasLoot = true)
        {
            _thirdPersonMovementController.enabled = false;
            BattleManager.LoseBattle();

            yield return null;
        }

        /* OVERRIDES */
        public override void StartRunEntityCoroutine()
        {
        }

        public override void StopRunEntityCoroutine()
        {
        }

        public override void GetEngaged(UnitController attacker)
        {
        }

        [ContextMenu("Get Hit")]
        public void GetHitContextMenu()
        {
            BaseGetHit(5, default);
        }
    }
}