using System.Collections;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class BarracksController : BuildingController, IInteractable
    {
        GameManager _gameManager;
        HeroManager _heroManager;

        Barracks _barracks;

        public new string InteractionPrompt => "Press F To Interact!";

        protected override void Start()
        {
            base.Start();
            _gameManager = GameManager.Instance;
            _heroManager = HeroManager.Instance;

            _barracks = BattleManager.Battle.Barracks;
            Building = _barracks;

            Initialize();
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        protected override void OnFightEnded()
        {
            if (FightManager.FightNumber == 3)
                Building.Unlock();

            if (!Building.IsUnlocked) return;
            AllowInteraction();
            StartCoroutine(SpawnPeasantsCoroutine());
        }

        IEnumerator SpawnPeasantsCoroutine()
        {
            for (int i = 0; i < Building.Level + 2; i++)
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

            BarracksScreen screen = new();
            screen.InitializeBuilding(_barracks);

            return true;
        }
    }
}