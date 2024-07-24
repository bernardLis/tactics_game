using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Pawn;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Barracks")]
    public class Barracks : Building
    {
        public List<BarracksNatureUpgrade> UnlockableNatures;

        GameManager _gameManager;

        public override void Initialize(Campaign campaign)
        {
            base.Initialize(campaign);
            _gameManager = GameManager.Instance;
            foreach (BarracksNatureUpgrade unlockableNature in UnlockableNatures)
                // HERE: save & load
                unlockableNature.CurrentLevel = 0;
        }

        public void ActivateUpgradeToken()
        {
            foreach (BarracksNatureUpgrade bnu in UnlockableNatures)
                bnu.IsTokenActive = true;
        }

        public void DisableUpgradeToken()
        {
            foreach (BarracksNatureUpgrade bnu in UnlockableNatures)
                bnu.IsTokenActive = false;
        }

        public List<Unit> GetUnitsPerFight()
        {
            List<Unit> unlockedUnits = new();
            for (int i = 0; i < GetPeasantsPerFight(); i++)
            {
                Unit u = Instantiate(_gameManager.UnitDatabase.Peasant);
                u.InitializeFight(0);
                unlockedUnits.Add(u);
            }

            foreach (BarracksNatureUpgrade unlockableNature in UnlockableNatures)
            {
                for (int i = 0; i < unlockableNature.CurrentLevel; i++)
                {
                    Pawn p = Instantiate(_gameManager.UnitDatabase.GetPawnByNature(unlockableNature.Nature));
                    p.InitializeFight(0);
                    p.SetUpgrade(i);
                    unlockedUnits.Add(p);
                }
            }

            return unlockedUnits;
        }


        public int GetPeasantsPerFight()
        {
            return Level + 3;
        }

        public int GetUpgradePrice()
        {
            return (Level + 1) * 200;
        }

        public BarracksNatureUpgrade GetNatureUpgrade(Nature nature)
        {
            return UnlockableNatures.Find(natureUpgrade => natureUpgrade.Nature == nature);
        }
    }
}