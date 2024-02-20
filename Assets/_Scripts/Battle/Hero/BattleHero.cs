using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleHero : BattleEntity
    {
        public Hero Hero { get; private set; }

        BattleHeroController _thirdPersonController;
        BattleHeroHealthBar _battleHeroHealthBar;

        BattleAreaManager _battleAreaManager;
        [SerializeField] GameObject _tileSecuredEffectPrefab;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _thirdPersonController = GetComponent<BattleHeroController>();
            _battleHeroHealthBar = GetComponentInChildren<BattleHeroHealthBar>();
            _battleAreaManager = BattleManager.GetComponent<BattleAreaManager>();

            GetComponent<BattleCreatureCatcher>().Initialize();
        }

        public override void InitializeEntity(Entity entity, int team)
        {
            base.InitializeEntity(entity, 0);
            gameObject.layer = 8;
            BattleEntityPathing.EnableAgent();

            Hero = (Hero)entity;

            _thirdPersonController.SetMoveSpeed(Hero.Speed.GetValue());
            Hero.Speed.OnValueChanged += _thirdPersonController.SetMoveSpeed;

            _battleHeroHealthBar.Initialize(Hero);

            Animator.enabled = true;

            HandleAbilities();

            _battleAreaManager.OnTileUnlocked += OnTileUnlocked;
        }

        void OnTileUnlocked(BattleTile tile)
        {
            Vector3 pos = transform.position + Vector3.up * 1;
            Quaternion rot = Quaternion.LookRotation(tile.transform.position - pos);
            GameObject tileSecuredEffect =
                Instantiate(_tileSecuredEffectPrefab, pos, rot);
            tileSecuredEffect.SetActive(true);
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
            EntityLog.Add($"{BattleManager.GetTime()}: Hero adds an ability {ability.name}");
            GameObject abilityPrefab = Instantiate(ability.AbilityManagerPrefab, transform);
            abilityPrefab.GetComponent<BattleAbility>().Initialize(ability);
        }

        public override IEnumerator GetHit(BattleEntity attacker, int specialDamage = 0)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Hero gets hit by {attacker.name}");
            int damage = Entity.CalculateDamage(attacker.Entity as EntityFight);
            BaseGetHit(damage, HealthColor);
            yield return null;
        }

        public IEnumerator GetHit(Minion minion)
        {
            EntityLog.Add($"{BattleManager.GetTime()}: Hero gets hit by a minion {minion.name}");
            BaseGetHit(Hero.CalculateDamage(minion), HealthColor);
            yield return null;
        }

        public override IEnumerator Die(BattleEntity attacker = null, bool hasLoot = true)
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