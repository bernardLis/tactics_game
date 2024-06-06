using System.Collections.Generic;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Barracks")]
    public class Barracks : Building
    {
        public List<UnlockableNature> UnlockableNatures;

        public override void Initialize()
        {
            base.Initialize();
            foreach (UnlockableNature unlockableNature in UnlockableNatures)
            {
                // HERE: save & load
                unlockableNature.IsUnlocked = false;
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