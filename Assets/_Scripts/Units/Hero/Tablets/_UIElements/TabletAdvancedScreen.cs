using Lis.Core;
using Lis.Units.Hero.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Tablets
{
    public class TabletAdvancedScreen : FullScreenElement
    {
        const string _ussClassName = "tablet-advanced-screen__";
        const string _ussMain = _ussClassName + "main";

        readonly TabletAdvanced _tabletAdvanced;
        AbilityElement _abilityElement;

        public TabletAdvancedScreen(TabletAdvanced tabletAdvanced)
        {
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TabletAdvancedScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            _tabletAdvanced = tabletAdvanced;
            Content.AddToClassList(_ussMain);

            AddElements();
            Content.Add(new HorizontalSpacerElement());
            AddAbility();
            Add(new HeroElement(BattleManager.Hero, true));

            DisableNavigation();
        }

        void AddElements()
        {
            Label title = new("Element Combo Unlocked!");
            title.style.fontSize = 34;
            Content.Add(title);

            Content.Add(new NatureComboElement((NatureAdvanced)_tabletAdvanced.Nature));
        }

        void AddAbility()
        {
            Label title = new("You Gain A New Ability:");
            title.style.fontSize = 34;
            Content.Add(title);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = 1;
            Content.Add(container);

            Ability.Ability a = ScriptableObject.Instantiate(_tabletAdvanced.Ability);
            a.InitializeBattle(BattleManager.Hero);
            BattleManager.Hero.AddAbility(a);

            _abilityElement = new(a, false, 200);

            container.Add(_abilityElement);

            AddContinueButton();
        }
    }
}