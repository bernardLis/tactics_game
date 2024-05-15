using System;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Peasant
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Peasant")]
    public class Peasant : Unit
    {
        public event Action<Nature> OnUpgraded;
        public void Upgrade(Nature nature)
        {
            OnUpgraded?.Invoke(nature);
        }
    }
}