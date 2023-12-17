using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabletAdvancedScreen : FullScreenElement
{
    const string _ussClassName = "tablet-advanced-screen__";
    const string _ussMain = _ussClassName + "main";

    TabletAdvanced _tabletAdvanced;
    AbilityElement _abilityElementFirst;
    AbilityElement _abilityElementSecond;

    public TabletAdvancedScreen(TabletAdvanced tabletAdvanced) : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TabletAdvancedScreenStyles);
        if (ss != null) styleSheets.Add(ss);

        _tabletAdvanced = tabletAdvanced;
        _content.AddToClassList(_ussMain);

        AddElements();
        _content.Add(new HorizontalSpacerElement());
        AddAbilities();

        _content.Add(new HeroElement(BattleManager.Instance.BattleHero.Hero, true));

        DisableNavigation();
    }

    void AddElements()
    {
        Label title = new("Element Combo Unlocked!");
        title.style.fontSize = 34;
        _content.Add(title);

        VisualElement container = new VisualElement();
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

    void AddAbilities()
    {
        Label title = new("Choose one ability:");
        title.style.fontSize = 34;
        _content.Add(title);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexGrow = 1;
        _content.Add(container);

        _abilityElementFirst = new AbilityElement(_tabletAdvanced.FirstAbility, false, 200);
        _abilityElementFirst.RegisterCallback<PointerUpEvent>(OnAbilityElementFirstClick);

        _abilityElementSecond = new AbilityElement(_tabletAdvanced.SecondAbility, false, 200);
        _abilityElementSecond.RegisterCallback<PointerUpEvent>(OnAbilityElementSecondClick);

        container.Add(_abilityElementFirst);
        container.Add(_abilityElementSecond);
    }

    void OnAbilityElementFirstClick(PointerUpEvent evt)
    {
        BaseAbilityClicked();
        BattleManager.Instance.Hero.AddAbility(_tabletAdvanced.FirstAbility);
    }

    void OnAbilityElementSecondClick(PointerUpEvent evt)
    {
        BaseAbilityClicked();
        BattleManager.Instance.Hero.AddAbility(_tabletAdvanced.SecondAbility);
    }

    void BaseAbilityClicked()
    {
        _abilityElementFirst.SetEnabled(false);
        _abilityElementSecond.SetEnabled(false);
        AddContinueButton();
    }
}
