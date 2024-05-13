using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Attack;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitScreen : FullScreenElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "unit-screen__";
        const string _ussContent = _ussClassName + "content";
        const string _ussInfoContainer = _ussClassName + "info-container";
        const string _ussStatsContainer = _ussClassName + "stats-container";
        const string _ussOtherContainer = _ussClassName + "other-container";

        readonly Unit _unit;

        protected readonly ScrollView MainCardContainer;

        VisualElement _nameContainer;

        VisualElement _basicInfoContainer;
        VisualElement _statsContainer;
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
            AddStats();
            AddBattleData();
            AddAttacks();


            AddContinueButton();
        }

        void CreateContainers()
        {
            _basicInfoContainer = new();
            _statsContainer = new();
            OtherContainer = new();

            _basicInfoContainer.AddToClassList(_ussInfoContainer);
            _statsContainer.AddToClassList(_ussStatsContainer);
            OtherContainer.AddToClassList(_ussOtherContainer);

            MainCardContainer.Add(_basicInfoContainer);

            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer);

            MainCardContainer.Add(_statsContainer);

            VisualElement spacer1 = new();
            spacer1.AddToClassList(USSCommonHorizontalSpacer);
            MainCardContainer.Add(spacer1);

            MainCardContainer.Add(OtherContainer);
        }

        void AddName()
        {
            _nameContainer = new();
            Label n = new(_unit.UnitName);
            n.style.fontSize = 34;
            _nameContainer.Add(n);
            _basicInfoContainer.Add(_nameContainer);
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

        void AddStats()
        {
            StatElement maxHealth = new(_unit.MaxHealth);
            StatElement armor = new(_unit.Armor);
            StatElement speed = new(_unit.Speed);
            StatElement power = new(_unit.Power);

            _statsContainer.Add(maxHealth);
            _statsContainer.Add(armor);
            _statsContainer.Add(speed);
            _statsContainer.Add(power);
        }

        protected virtual void AddOtherBasicInfo()
        {
            Label exp = new($"Exp: {_unit.Experience.Value}/{_unit.ExpForNextLevel.Value}");
            OtherContainer.Add(exp);

            Label price = new($"Price: {_unit.Price}");
            OtherContainer.Add(price);
        }

        void AddBattleData()
        {
            Label killCount = new($"Kill Count: {_unit.TotalKillCount}");
            Label damageDealt = new($"Damage Dealt: {_unit.GetDamageDealt()}");

            OtherContainer.Add(killCount);
            OtherContainer.Add(damageDealt);
        }

        void AddAttacks()
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