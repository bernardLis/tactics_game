using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Attack;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitScreen : FullScreenElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        protected const string USSClassName = "unit-screen__";
        const string _ussContent = USSClassName + "content";
        const string _ussInfoContainer = USSClassName + "info-container";
        const string _ussStatsContainer = USSClassName + "stats-container";
        const string _ussOtherContainer = USSClassName + "other-container";

        readonly Unit _unit;

        protected readonly ScrollView MainCardContainer;

        VisualElement _basicInfoContainer;
        protected VisualElement StatsContainer;
        protected VisualElement OtherContainer;

        protected UnitIcon UnitIcon;

        public UnitScreen(Unit unit)
        {
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitScreenStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _unit = unit;

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

        void CreateContainers()
        {
            _basicInfoContainer = new();
            StatsContainer = new();
            OtherContainer = new();

            _basicInfoContainer.AddToClassList(_ussInfoContainer);
            StatsContainer.AddToClassList(_ussStatsContainer);
            OtherContainer.AddToClassList(_ussOtherContainer);

            MainCardContainer.Add(_basicInfoContainer);

            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer);

            MainCardContainer.Add(StatsContainer);

            VisualElement spacer1 = new();
            spacer1.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer1);

            MainCardContainer.Add(OtherContainer);
        }

        void AddName()
        {
            if (_unit.UnitName.Length == 0) _unit.UnitName = Helpers.ParseScriptableObjectName(_unit.name);
            SetTitle(_unit.UnitName);
        }

        void AddIcon()
        {
            UnitIcon = new(_unit, true);
            _basicInfoContainer.Add(UnitIcon);
        }

        void AddLevel()
        {
            Label l = new($"<b>Level {_unit.Level.Value} {Helpers.ParseScriptableObjectName(_unit.name)}<b>");
            _basicInfoContainer.Add(l);
        }

        void AddElement()
        {
            NatureElement e = new(_unit.Nature);
            _basicInfoContainer.Add(e);
        }

        protected virtual void HandleStats()
        {
            StatElement maxHealth = new(_unit.MaxHealth);
            StatElement armor = new(_unit.Armor);
            StatElement speed = new(_unit.Speed);
            StatElement power = new(_unit.Power);

            StatsContainer.Add(maxHealth);
            StatsContainer.Add(armor);
            StatsContainer.Add(speed);
            StatsContainer.Add(power);
        }

        protected virtual void AddOtherBasicInfo()
        {
            Label exp = new($"Exp: {_unit.Experience.Value}/{_unit.ExpForNextLevel.Value}");
            OtherContainer.Add(exp);

            Label price = new($"Price: {_unit.Price}");
            OtherContainer.Add(price);
        }

        protected virtual void AddBattleData()
        {
            Label killCount = new($"Kill Count: {_unit.TotalKillCount}");
            Label damageDealt = new($"Damage Dealt: {_unit.GetDamageDealt()}");

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
            foreach (Attack.Attack a in _unit.Attacks)
            {
                AttackElement ae = new(a);
                container.Add(ae);
            }
        }
    }
}