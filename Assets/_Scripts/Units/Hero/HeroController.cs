using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Units.Hero.Ability;
using NaughtyAttributes;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class HeroController : PlayerUnitController
    {
        static readonly int AnimGrounded = Animator.StringToHash("Grounded");

        readonly List<Controller> _abilityControllers = new();

        MovementController _movementController;
        public Hero Hero { get; private set; }

        public event Action OnTeleportedToBase;


        void OnDestroy()
        {
            Hero.OnAbilityAdded -= AddAbility;
        }

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _movementController = GetComponent<MovementController>();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, 0);
            gameObject.layer = 8;

            Hero = (Hero)unit;

            Hero.OnLevelUp += () => DisplayFloatingText("Level Up!", Color.yellow);

            _movementController.SetMoveSpeed(Hero.Speed.GetValue());
            Hero.Speed.OnValueChanged += _movementController.SetMoveSpeed;

            GetComponentInChildren<HealthBarDisplayer>().Initialize(Hero);

            Animator.enabled = true;
            Animator.SetBool(AnimGrounded, true);

            HandleAbilities();
        }

        void HandleAbilities()
        {
            Hero.OnAbilityAdded += AddAbility;
            Hero.OnAbilityRemoved += RemoveAbility;

            foreach (Ability.Ability a in Hero.Abilities)
                AddAbility(a);
        }

        void AddAbility(Ability.Ability ability)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Hero adds an ability {ability.name}");
            GameObject abilityPrefab = Instantiate(ability.AbilityManagerPrefab, transform);
            Controller abilityController = abilityPrefab.GetComponent<Controller>();
            abilityController.Initialize(ability);
            _abilityControllers.Add(abilityPrefab.GetComponent<Controller>());
        }

        void RemoveAbility(Ability.Ability ability)
        {
            UnitLog.Add($"{BattleManager.GetTime()}: Hero removes an ability {ability.name}");
            Controller abilityController = _abilityControllers.Find(a => a.Ability == ability);
            abilityController.StopAbility();
            _abilityControllers.Remove(abilityController);
            Destroy(abilityController.gameObject);
        }


        protected override IEnumerator DieCoroutine(Attack.Attack attack = null, bool hasLoot = true)
        {
            yield return base.DieCoroutine(attack, hasLoot);
            _movementController.DisableMovement();

            yield return new WaitForSeconds(5f);
            BattleManager.LoseBattle();
        }

        /* OVERRIDES */
        protected override void RunUnit()
        {
        }

        protected override void StopUnit()
        {
        }

        public override void GetEngaged(UnitController attacker)
        {
        }

        public override void TeleportToBase()
        {
            StartCoroutine(TeleportToBaseCoroutine());
        }

        IEnumerator TeleportToBaseCoroutine()
        {
            StartTeleport();
            yield return new WaitForSeconds(0.5f);
            transform.position = ArenaManager.GetRandomPositionInPlayerLockerRoom();
            yield return new WaitForSeconds(0.5f);
            EndTeleport();
            OnTeleportedToBase?.Invoke();
        }

        public override void TeleportToArena()
        {
            StartCoroutine(TeleportToArenaCoroutine());
        }

        IEnumerator TeleportToArenaCoroutine()
        {
            StartTeleport();
            yield return new WaitForSeconds(0.5f);
            transform.position = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            EndTeleport();
        }

        void StartTeleport()
        {
            if (Hero.TeleportStartSound != null)
            {
                AudioManager.CreateSound()
                    .WithSound(Hero.TeleportStartSound)
                    .WithParent(transform)
                    .Play();
            }

            _movementController.DisableMovement();
            _movementController.enabled = false;
            TeleportEffect.transform.localScale = Vector3.zero;
            TeleportEffect.SetActive(true);
            TeleportEffect.transform.DOScale(1, 1f);
        }

        void EndTeleport()
        {
            if (Hero.TeleportEndSound != null)
            {
                AudioManager.CreateSound()
                    .WithSound(Hero.TeleportEndSound)
                    .WithPosition(transform.position)
                    .Play();
            }
            _movementController.enabled = true;
            _movementController.EnableMovement();
            TeleportEffect.transform.DOScale(0, 1f)
                .OnComplete(() => TeleportEffect.SetActive(false));
        }

        [Button("Stop All Abilities")]
        public void StopAllAbilities()
        {
            foreach (Controller abilityController in _abilityControllers)
                abilityController.StopAbility();
        }

        [Button("Start All Abilities")]
        public void StartAllAbilities()
        {
            foreach (Controller abilityController in _abilityControllers)
                abilityController.StartAbility();
        }


        [Button("Add Advanced Tablet")]
        public void AddAdvancedTablet()
        {
            Hero.AddAdvancedTablet(
                GameManager.UnitDatabase.GetAdvancedTabletByNatureNames(NatureName.Earth, NatureName.Fire));
        }
    }
}