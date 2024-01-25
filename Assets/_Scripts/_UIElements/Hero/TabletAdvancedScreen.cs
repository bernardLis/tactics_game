using UnityEngine.UIElements;
using UnityEngine;

namespace Lis
{
    public class TabletAdvancedScreen : FullScreenElement
    {
        const string _ussClassName = "tablet-advanced-screen__";
        const string _ussMain = _ussClassName + "main";

        readonly TabletAdvanced _tabletAdvanced;
        AbilityElement _abilityElement;

        public TabletAdvancedScreen(TabletAdvanced tabletAdvanced)
        {
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TabletAdvancedScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            _tabletAdvanced = tabletAdvanced;
            _content.AddToClassList(_ussMain);

            AddElements();
            _content.Add(new HorizontalSpacerElement());
            AddAbility();
            Add(new HeroElement(_battleManager.Hero, true));

            DisableNavigation();
        }

        void AddElements()
        {
            Label title = new("Element Combo Unlocked!");
            title.style.fontSize = 34;
            _content.Add(title);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            _content.Add(container);

            VisualElement elementContainer1 = new();
            elementContainer1.Add(new ElementalElement(_tabletAdvanced.FirstElement, 100));
            elementContainer1.Add(new Label($"{_tabletAdvanced.FirstElement.ElementName}"));
            container.Add(elementContainer1);

            container.Add(new Label("+"));

            VisualElement elementContainer2 = new();
            elementContainer2.Add(new ElementalElement(_tabletAdvanced.SecondElement, 100));
            elementContainer2.Add(new Label($"{_tabletAdvanced.SecondElement.ElementName}"));
            container.Add(elementContainer2);

            container.Add(new Label("="));

            VisualElement elementContainer3 = new();
            elementContainer3.Add(new ElementalElement(_tabletAdvanced.Element, 100));
            elementContainer3.Add(new Label($"{_tabletAdvanced.Element.ElementName}"));
            container.Add(elementContainer3);
        }

        void AddAbility()
        {
            Label title = new("You Gain A New Ability:");
            title.style.fontSize = 34;
            _content.Add(title);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = 1;
            _content.Add(container);

            Ability a = ScriptableObject.Instantiate(_tabletAdvanced.Ability);
            a.InitializeBattle(_battleManager.Hero);
            _battleManager.Hero.AddAbility(a);

            _abilityElement = new(a, false, 200);

            container.Add(_abilityElement);

            AddContinueButton();
        }
    }
}