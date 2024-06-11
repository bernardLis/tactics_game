using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Barracks")]
    public class Barracks : Building
    {
        public List<BarracksNatureUpgrade> UnlockableNatures;

        public override void Initialize()
        {
            base.Initialize();
            foreach (BarracksNatureUpgrade unlockableNature in UnlockableNatures)
            {
                // HERE: save & load
                unlockableNature.CurrentLevel = 0;
            }
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