using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Items;
using UnityEngine;

namespace Lis.Camp.Building
{
    [CreateAssetMenu(menuName = "ScriptableObject/Camp/Building/Boasting Platform")]
    public class BoastingPlatform : Building
    {
        [HideInInspector] public List<Boast> AvailableBoasts = new();

        protected override void NodeCompleted()
        {
            base.NodeCompleted();
            if (!IsUnlocked) return;
        }
    }
}