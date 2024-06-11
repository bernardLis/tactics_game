using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Attack;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitScreen : FullScreenElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";

        protected const string USSClassName = "unit-screen__";
        private const string _ussContent = USSClassName + "content";
        private const string _ussInfoContainer = USSClassName + "info-container";
        private const string _ussStatsContainer = USSClassName + "stats-container";
        private const string _ussOtherContainer = USSClassName + "other-container";

        protected readonly ScrollView MainCardContainer;

        protected readonly Unit Unit;

        protected VisualElement BasicInfoContainer;
        protected VisualElement OtherContainer;
        protected VisualElement StatsContainer;

        protected UnitIcon UnitIcon;

        public UnitScreen(Unit unit)
        {
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitScreenStyles);
            if (ss != null)
                styleSheets.Add(ss);

            Unit = unit;

            AddToClassList(_ussCommonTextPrimary);

            MainCardContainer = new();
            MainCardContainer.AddToClassList(_ussContent);
            Content.Add(MainCardContainer);
        }

        public virtual void Initialize()
        {
            CreateContainers();
            AddName();
            AddIcon();
            AddLevel();
            AddElement();
            AddOtherBasicInfo();
            HandleStats();
            AddBattleData();
            AddAttacks();

            AddContinueButton();
        }

        private void CreateContainers()
        {
            BasicInfoContainer = new();
            StatsContainer = new();
            OtherContainer = new();

            BasicInfoContainer.AddToClassList(_ussInfoContainer);
            StatsContainer.AddToClassList(_ussStatsContainer);
            OtherContainer.AddToClassList(_ussOtherContainer);

            MainCardContainer.Add(BasicInfoContainer);

            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer);

            MainCardContainer.Add(StatsContainer);

            VisualElement spacer1 = new();
            spacer1.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer1);

            MainCardContainer.Add(OtherContainer);
        }

        private void AddName()
        {
            if (Unit.UnitName.Length == 0) Unit.UnitName = Helpers.ParseScriptableObjectName(Unit.name);
            SetTitle(Unit.UnitName);
        }

        private void AddIcon()
        {
            UnitIcon = new(Unit, true);
            BasicInfoContainer.Add(UnitIcon);
        }

        protected virtual void AddLevel()
        {
            Label l = new($"<b>Level {Unit.Level.Value} {Helpers.ParseScriptableObjectName(Unit.name)}<b>");
            BasicInfoContainer.Add(l);
        }

        private void AddElement()
        {
            NatureElement e = new(Unit.Nature);
            BasicInfoContainer.Add(e);
        }

        protected virtual void HandleStats()
        {
            StatElement maxHealth = new(Unit.MaxHealth);
            StatElement armor = new(Unit.Armor);
            StatElement speed = new(Unit.Speed);
            StatElement power = new(Unit.Power);

            StatsContainer.Add(maxHealth);
            StatsContainer.Add(armor);
            StatsContainer.Add(speed);
            StatsContainer.Add(power);
        }

        protected virtual void AddOtherBasicInfo()
        {
            Debug.Log("UnitScreen.AddOtherBasicInfo()");
            Debug.Log($"Unit {Unit}");
            Debug.Log($"Unit.Experience {Unit.Experience}");
            Debug.Log($"Unit.ExpForNextLevel {Unit.ExpForNextLevel}");

            Label exp = new($"Exp: {Unit.Experience.Value}/{Unit.ExpForNextLevel.Value}");
            OtherContainer.Add(exp);

            Label price = new($"Price: {Unit.Price}");
            OtherContainer.Add(price);
        }

        protected virtual void AddBattleData()
        {
            Label killCount = new($"Kill Count: {Unit.TotalKillCount}");
            Label damageDealt = new($"Damage Dealt: {Unit.GetDamageDealt()}");

            OtherContainer.Add(killCount);
            OtherContainer.Add(damageDealt);
        }

        protected virtual void AddAttacks()
        {
            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            OtherContainer.Add(spacer);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            OtherContainer.Add(container);
            foreach (Attack.Attack a in Unit.Attacks)
            {
                AttackElement ae = new(a);
                container.Add(ae);
            }
        }
    }
}