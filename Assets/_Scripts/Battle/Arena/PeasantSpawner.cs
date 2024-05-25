using System.Collections;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class PeasantSpawner : BuildingController, IInteractable
    {
        GameManager _gameManager;
        HeroManager _heroManager;

        public new string InteractionPrompt => "Press F To Interact!";

        protected override void Start()
        {
            base.Start();
            _gameManager = GameManager.Instance;
            _heroManager = HeroManager.Instance;
            Building = BattleManager.Battle.PeasantSpawner;
            Initialize();
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        protected override void OnFightEnded()
        {
            if (!Building.IsUnlocked) return;
            AllowInteraction();
            StartCoroutine(SpawnPeasantsCoroutine());
        }

        IEnumerator SpawnPeasantsCoroutine()
        {
            for (int i = 0; i < Building.Level + 1; i++)
            {
                Unit u = Instantiate(_gameManager.UnitDatabase.Peasant);
                u.InitializeBattle(0);
                _heroManager.Hero.Army.Add(u); // without update to spawn at position
                UnitController uc = FightManager.SpawnPlayerUnit(u, transform.position);
                uc.GoBackToLocker();
                yield return new WaitForSeconds(1);
            }
        }

        protected override void OnFightStarted()
        {
            if (!Building.IsUnlocked) return;
            ForbidInteraction();
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is active");
                return false;
            }

            PeasantHouseScreen screen = new();
            screen.InitializeBuilding(Building);

            return true;
        }
    }
}