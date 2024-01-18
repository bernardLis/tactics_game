using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleHero : BattleEntity
    {
        public Hero Hero { get; private set; }

        BattleHeroController _thirdPersonController;
        BattleHeroHealthBar _battleHeroHealthBar;

        Dictionary<Ability, GameObject> _battleAbilities = new();

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, 0);
            _agent.enabled = true;

            Hero = (Hero)entity;

            _thirdPersonController = GetComponent<BattleHeroController>();
            _thirdPersonController.SetMoveSpeed(Hero.Speed.GetValue());
            Hero.Speed.OnValueChanged += _thirdPersonController.SetMoveSpeed;

            _battleHeroHealthBar = GetComponentInChildren<BattleHeroHealthBar>();
            _battleHeroHealthBar.Initialize(Hero);

            Animator.enabled = true;

            HandleAbilities();
        }

        void HandleAbilities()
        {
            Hero.OnAbilityAdded += AddAbility;

            foreach (Ability a in Hero.Abilities)
                AddAbility(a);
        }


        void OnDestroy()
        {
            Hero.OnAbilityAdded -= AddAbility;
        }

        void AddAbility(Ability ability)
        {
            GameObject abilityPrefab = Instantiate(ability.AbilityManagerPrefab);
            abilityPrefab.transform.SetParent(transform);
            abilityPrefab.GetComponent<BattleAbility>().Initialize(ability);
            _battleAbilities.Add(ability, abilityPrefab);
        }

        public override IEnumerator GetHit(EntityFight attacker, int specialDamage = 0)
        {
            BaseGetHit(5, default);
            yield return null;
        }

        public override IEnumerator Die(EntityFight attacker = null, bool hasLoot = true)
        {
            _thirdPersonController.enabled = false;
            _battleManager.LoseBattle();

            yield return null;
        }

        /* OVERRIDES */
        public override void StartRunEntityCoroutine()
        {
        }

        public override void StopRunEntityCoroutine()
        {
        }

        public override void GetEngaged(BattleEntity engager)
        {
        }

        [ContextMenu("Get Hit")]
        public void GetHitContextMenu()
        {
            BaseGetHit(5, default);
        }
    }
}