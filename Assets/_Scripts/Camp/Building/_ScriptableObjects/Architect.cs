using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Architect")]
    public class Architect : Building
    {
        Campaign _campaign;

        public override void Initialize(Campaign campaign)
        {
            base.Initialize(campaign);
            _campaign = campaign;
        }

        public List<Building> UnlockableBuildings()
        {
            List<Building> unlockableBuildings = new();
            foreach (Building b in _campaign.GetAllBuildings())
                if (!b.IsUnlocked && b.UnlockCost > 0)
                    unlockableBuildings.Add(b);

            return unlockableBuildings;
        }
    }
}