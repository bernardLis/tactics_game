using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Blacksmith")]
    public class Blacksmith : Building
    {
        [HideInInspector] public List<Armor> AvailableArmor = new();

        protected override void NodeCompleted()
        {
            base.NodeCompleted();
            if (!IsUnlocked) return;
            if (GetAssignedWorkerCount() == 0) return;

            GameManager gm = GameManager.Instance;

            // produce a piece of armor
            Armor armor = null;
            if (Campaign.Hero.VisualHero.BodyType == 0)
                armor = Instantiate(gm.UnitDatabase.GetRandomFemaleArmor());
            if (Campaign.Hero.VisualHero.BodyType == 1)
                armor = Instantiate(gm.UnitDatabase.GetRandomMaleArmor());

            AvailableArmor.Add(armor);
        }

        public void CollectArmor()
        {
            foreach (Armor a in AvailableArmor)
            {
                Campaign.Hero.AddArmor(a);
            }
            AvailableArmor.Clear();
        }
    }
}