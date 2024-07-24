using System.Collections;
using Lis.Arena;
using Lis.Arena.Fight;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using Lis.Units.Pawn;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class BarracksController : BuildingController, IInteractable
    {
        Barracks _barracks;
        GameManager _gameManager;
        HeroManager _heroManager;

        protected override void Start()
        {
            base.Start();
            _gameManager = GameManager.Instance;
            _heroManager = HeroManager.Instance;

            _barracks = FightManager.Campaign.Barracks;
            Building = _barracks;

            foreach (BarracksNatureUpgrade bnu in _barracks.UnlockableNatures)
                bnu.OnUpgrade += SpawnPawn;

            Initialize();
        }

        public new string InteractionPrompt => "Interact";

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is active");
                return false;
            }

            BarracksScreen screen = new();
            screen.InitializeBuilding(_barracks, this);

            return true;
        }

        protected override void OnFightEnded()
        {
            if (!Building.IsUnlocked) return;
            AllowInteraction();
            StartCoroutine(SpawnFightEndArmy());
        }

        IEnumerator SpawnFightEndArmy()
        {
            for (int i = 0; i < _barracks.GetPeasantsPerFight(); i++)
            {
                SpawnPeasant();
                yield return new WaitForSeconds(1);
            }

            // for every upgrade in barracks unlockable natures spawn a pawn with correct nature and level
            foreach (BarracksNatureUpgrade natureUpgrade in _barracks.UnlockableNatures)
            {
                if (natureUpgrade.CurrentLevel == 0) continue;
                for (int i = 1; i <= natureUpgrade.CurrentLevel; i++)
                {
                    SpawnPawn(natureUpgrade.Nature, i);
                    yield return new WaitForSeconds(1);
                }
            }
        }

        public void SpawnPeasant()
        {
            Unit u = Instantiate(_gameManager.UnitDatabase.Peasant);
            u.InitializeFight(0);
            _heroManager.Hero.AddArmy(u);
            UnitController uc = FightManager.SpawnPlayerUnit(u, transform.position);
            if (uc is PlayerUnitController puc)
                puc.GoToLocker();
        }

        void SpawnPawn(Nature n, int upgrade)
        {
            upgrade--; // coz pawn upgrade start from 0 and building at level 0 is locked

            Pawn p = Instantiate(_gameManager.UnitDatabase.GetPawnByNature(n));
            p.InitializeFight(0);
            p.SetUpgrade(upgrade);
            _heroManager.Hero.AddArmy(p);
            UnitController uc = FightManager.SpawnPlayerUnit(p, transform.position);
            if (uc is PlayerUnitController puc)
                puc.GoToLocker();
        }


        protected override void OnFightStarted()
        {
            if (!Building.IsUnlocked) return;
            ForbidInteraction();
        }
    }
}