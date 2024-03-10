using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;
using Element = Lis.Units.Hero.Ability.Element;

namespace Lis.Units.Hero.Tablets
{
    public class TabletAdvancedScreen : FullScreenElement
    {
        const string _ussClassName = "tablet-advanced-screen__";
        const string _ussMain = _ussClassName + "main";

        readonly TabletAdvanced _tabletAdvanced;
        Element _element;

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

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            Content.Add(container);

            VisualElement elementContainer1 = new();
            elementContainer1.Add(new NatureElement(_tabletAdvanced.FirstNature, 100));
            elementContainer1.Add(new Label($"{_tabletAdvanced.FirstNature.NatureName}"));
            container.Add(elementContainer1);

            container.Add(new Label("+"));

            VisualElement elementContainer2 = new();
            elementContainer2.Add(new NatureElement(_tabletAdvanced.SecondNature, 100));
            elementContainer2.Add(new Label($"{_tabletAdvanced.SecondNature.NatureName}"));
            container.Add(elementContainer2);

            container.Add(new Label("="));

            VisualElement elementContainer3 = new();
            elementContainer3.Add(new NatureElement(_tabletAdvanced.Nature, 100));
            elementContainer3.Add(new Label($"{_tabletAdvanced.Nature.NatureName}"));
            container.Add(elementContainer3);
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

            _element = new(a, false, 200);

            container.Add(_element);

            AddContinueButton();
        }
    }
}