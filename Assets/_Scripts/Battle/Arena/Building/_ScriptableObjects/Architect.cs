using System.Collections.Generic;
using UnityEngine;

namespace Lis.Battle.Arena.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Building/Architect")]
    public class Architect : Building
    {
        Battle _battle;

        public override void Initialize(Battle battle)
        {
            base.Initialize(battle);
            _battle = battle;
        }

        public List<Building> UnlockableBuildings()
        {
            List<Building> unlockableBuildings = new();
            foreach (Building b in _battle.GetAllBuildings())
                if (!b.IsUnlocked && b.UnlockCost > 0)
                    unlockableBuildings.Add(b);

            return unlockableBuildings;
        }
    }
}