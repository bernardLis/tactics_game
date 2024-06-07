using System.Collections.Generic;
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

        public int GetPeasantsPerFight()
        {
            return Level + 3;
        }

        public int GetUpgradePrice()
        {
            return (Level + 1) * 200;
        }
    }
}