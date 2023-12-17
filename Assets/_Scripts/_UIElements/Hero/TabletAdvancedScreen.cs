using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabletAdvancedScreen : FullScreenElement
{
    TabletAdvanced _tabletAdvanced;
    AbilityElement _abilityElementFirst;
    AbilityElement _abilityElementSecond;

    public TabletAdvancedScreen(TabletAdvanced tabletAdvanced) : base()
    {
        _tabletAdvanced = tabletAdvanced;

        _content.Add(new ElementalElement(tabletAdvanced.FirstElement));
        _content.Add(new ElementalElement(tabletAdvanced.SecondElement));
        _content.Add(new ElementalElement(tabletAdvanced.Element));

        _content.Add(new Label("Choose one ability!"));

        _abilityElementFirst = new AbilityElement(tabletAdvanced.FirstAbility);
        _abilityElementFirst.RegisterCallback<PointerUpEvent>(OnAbilityElementFirstClick);

        _abilityElementSecond = new AbilityElement(tabletAdvanced.SecondAbility);
        _abilityElementSecond.RegisterCallback<PointerUpEvent>(OnAbilityElementSecondClick);

        _content.Add(_abilityElementFirst);
        _content.Add(_abilityElementSecond);

        DisableNavigation();
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
