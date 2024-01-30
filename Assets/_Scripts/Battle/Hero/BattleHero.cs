using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleHero : BattleEntity
    {
        public Hero Hero { get; private set; }

        BattleHeroController _thirdPersonController;
        BattleHeroHealthBar _battleHeroHealthBar;

        Color _healthColor;

        BattleAreaManager _battleAreaManager;
        [SerializeField] GameObject _tileSecuredEffectPrefab;

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, 0);
            Agent.enabled = true;

            Hero = (Hero)entity;

            _thirdPersonController = GetComponent<BattleHeroController>();
            _thirdPersonController.SetMoveSpeed(Hero.Speed.GetValue());
            Hero.Speed.OnValueChanged += _thirdPersonController.SetMoveSpeed;

            _battleHeroHealthBar = GetComponentInChildren<BattleHeroHealthBar>();
            _battleHeroHealthBar.Initialize(Hero);

            Animator.enabled = true;

            HandleAbilities();

            _healthColor = GameManager.GameDatabase.GetColorByName("Health").Primary;

            _battleAreaManager = BattleManager.Instance.GetComponent<BattleAreaManager>();
            _battleAreaManager.OnTileSecured += OnTileSecured;
        }

        void OnTileSecured(BattleTile tile)
        {
            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.LookRotation(tile.transform.position - pos);
            GameObject tileSecuredEffect =
                Instantiate(_tileSecuredEffectPrefab, pos, rot);

            Destroy(tileSecuredEffect, 15f);
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
            GameObject abilityPrefab = Instantiate(ability.AbilityManagerPrefab, transform);
            abilityPrefab.GetComponent<BattleAbility>().Initialize(ability);
        }

        public override IEnumerator GetHit(EntityFight attacker, int specialDamage = 0)
        {
            BaseGetHit(5, default);
            yield return null;
        }

        public IEnumerator GetHit(Minion minion)
        {
            BaseGetHit(Hero.CalculateDamage(minion), _healthColor);
            yield return null;
        }

        public override IEnumerator Die(EntityFight attacker = null, bool hasLoot = true)
        {
            _thirdPersonController.enabled = false;
            BattleManager.LoseBattle();

            yield return null;
        }

        /* OVERRIDES */
        protected override void StartRunEntityCoroutine()
        {
        }

        protected override void StopRunEntityCoroutine()
        {
        }

        public override void GetEngaged(BattleEntity attacker)
        {
        }

        [ContextMenu("Get Hit")]
        public void GetHitContextMenu()
        {
            BaseGetHit(5, default);
        }
    }
}