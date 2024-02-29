using System.Collections;
using Lis.Battle.Land;
using Lis.Units.Hero.Ability;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class HeroController : UnitController
    {
        AreaManager _areaManager;

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
            _areaManager = BattleManager.GetComponent<AreaManager>();

            GetComponent<CreatureCatcher>().Initialize();

            _areaManager.OnTileUnlocked += OnTileUnlocked;
        }

        void OnTileUnlocked(TileController tileController)
        {
            if (tileController == _areaManager.HomeTileController) return;

            Vector3 heroPos = transform.position;
            Vector3 tilePos = tileController.transform.position;
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