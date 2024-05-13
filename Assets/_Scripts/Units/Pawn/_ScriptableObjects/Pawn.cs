using System;
using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Pawn")]
    public class Pawn : Unit
    {
        [Header("Pawn")]
        public PawnUpgrade[] Upgrades;

        public int CurrentUpgrade;

        public event Action OnUpgraded;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            CurrentUpgrade = 0;

            SetUnitBasics();
        }

        public void Upgrade()
        {
            CurrentUpgrade++;
            SetUnitBasics();

            // TODO: increase stats

            OnUpgraded?.Invoke();
        }

        void SetUnitBasics()
        {
            UnitName = Upgrades[CurrentUpgrade].Name;
            Icon = Upgrades[CurrentUpgrade].Icon;
            Price = Upgrades[CurrentUpgrade].Price;
        }

        public PawnUpgrade GetCurrentUpgrade()
        {
            return Upgrades[CurrentUpgrade];
        }

        public PawnUpgrade GetNextUpgrade()
        {
            return CurrentUpgrade < Upgrades.Length - 1 ? Upgrades[CurrentUpgrade + 1] : null;
        }
    }
}