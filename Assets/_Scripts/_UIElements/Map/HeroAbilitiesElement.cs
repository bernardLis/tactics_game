using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroAbilitiesElement : VisualElement
{
    Hero _hero;

    List<AbilitySlot> _abilitySlots = new();
    List<AbilityButton> _abilityButtons = new();

    public HeroAbilitiesElement(Hero hero)
    {
        _hero = hero;

        style.flexDirection = FlexDirection.Row;
        CreateAbilities();
    }

    void CreateAbilities()
    {
        int slotCount = 5;
        if (_hero.Items.Count > slotCount)
            slotCount = _hero.Abilities.Count;

        for (int i = 0; i < slotCount; i++)
        {
            AbilitySlot abilitySlot = new();
            abilitySlot.Hero = _hero;
            _abilitySlots.Add(abilitySlot);
            Add(abilitySlot);
            abilitySlot.OnAbilityAdded += OnAbilityAdded;
            abilitySlot.OnAbilityRemoved += OnAbilityRemoved;
        }

        for (int i = 0; i < _hero.Abilities.Count; i++)
            _abilitySlots[i].AddActionButton(new(_hero.Abilities[i]));
    }

    void OnAbilityAdded(Ability ability) { _hero.AddAbility(ability); }
    void OnAbilityRemoved(Ability ability) { _hero.RemoveAbility(ability); }


}
