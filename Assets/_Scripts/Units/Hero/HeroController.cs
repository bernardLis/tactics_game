using System.Collections;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class HeroController : UnitController
    {
        public Hero Hero { get; private set; }

        MovementController _movementController;
        HealthBarDisplayer _healthBarDisplayer;

        private static readonly int AnimGrounded = Animator.StringToHash("Grounded");

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _movementController = GetComponent<MovementController>();
            _healthBarDisplayer = GetComponentInChildren<HealthBarDisplayer>();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, 0);
            gameObject.layer = 8;

            Hero = (Hero)unit;

            _movementController.SetMoveSpeed(Hero.Speed.GetValue());
            Hero.Speed.OnValueChanged += _movementController.SetMoveSpeed;

            _healthBarDisplayer.Initialize(Hero);

            Animator.enabled = true;
            Animator.SetBool(AnimGrounded, true);

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
            abilityPrefab.GetComponent<Ability.Controller>().Initialize(ability);
        }

        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            _movementController.enabled = false;
            BattleManager.LoseBattle();

            yield return null;
        }

        /* OVERRIDES */
        public override void RunUnit()
        {
        }

        public override void StopUnit()
        {
        }

        public override void GetEngaged(UnitController attacker)
        {
        }

        [ContextMenu("Add Advanced Tablet")]
        public void AddAdvancedTablet()
        {
            Hero.AddAdvancedTablet(
                GameManager.UnitDatabase.GetAdvancedTabletByNatureNames(NatureName.Earth, NatureName.Fire));
        }
    }
}