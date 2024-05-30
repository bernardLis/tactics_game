using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lis.Units.Pawn
{
    [CreateAssetMenu(menuName = "ScriptableObject/Units/Pawn/Pawn")]
    public class Pawn : Unit
    {
        [Header("Pawn")]
        public PawnUpgrade[] Upgrades;

        [HideInInspector] public int CurrentUpgrade;
        [HideInInspector] public PawnMission CurrentMission;

        public event Action OnUpgraded;

        public override void InitializeBattle(int team)
        {
            base.InitializeBattle(team);
            CurrentUpgrade = 0;

            InitializePawnMission();
            SetUnitBasics();
            SetPawnStats();
            SetPawnAttacks();
        }

        public void SetUpgrade(int upgrade)
        {
            CurrentUpgrade = upgrade;
            InitializePawnMission();
            SetUnitBasics();
            SetPawnStats();
            SetPawnAttacks();
        }

        public override void LevelUp()
        {
            if (Level.Value == GetCurrentUpgrade().LevelLimit) return;
            Level.ApplyChange(1);
            InvokeLevelUp();
        }

        public void Upgrade()
        {
            if (CurrentUpgrade >= Upgrades.Length - 1) return;
            CurrentUpgrade++;

            InitializePawnMission();
            SetUnitBasics();
            SetPawnStats();

            SetPawnAttacks();

            OnUpgraded?.Invoke();
        }

        void InitializePawnMission()
        {
            if (CurrentUpgrade >= Upgrades.Length) return;

            CurrentMission = Instantiate(Upgrades[CurrentUpgrade].Mission);
            CurrentMission.Initialize(this);
        }

        void SetPawnStats()
        {
            MaxHealth.SetBaseValue(Upgrades[CurrentUpgrade].BaseHealth);
            CurrentHealth.SetValue(MaxHealth.GetValue());
            Armor.SetBaseValue(Upgrades[CurrentUpgrade].BaseArmor);
            Speed.SetBaseValue(Upgrades[CurrentUpgrade].BaseSpeed);
            Power.SetBaseValue(Upgrades[CurrentUpgrade].BasePower);
        }

        void SetPawnAttacks()
        {
            List<Attack.Attack> copy = new(Attacks);
            foreach (Attack.Attack a in copy)
                RemoveAttack(a);

            foreach (Attack.Attack attack in GetCurrentUpgrade().Attacks)
                AddAttack(Instantiate(attack));

            ChooseAttack();
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