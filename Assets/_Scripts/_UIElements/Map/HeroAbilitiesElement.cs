using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroAbilitiesElement : VisualElement
{
    MapHero _mapHero;

    List<AbilitySlot> _abilitySlots = new();
    List<AbilityButton> _abilityButtons = new();

    public HeroAbilitiesElement(MapHero mapHero)
    {
        _mapHero = mapHero;

        style.flexDirection = FlexDirection.Row;
        CreateAbilities();
    }

    void CreateAbilities()
    {
        int slotCount = 5;
        if (_mapHero.Hero.Items.Count > slotCount)
            slotCount = _mapHero.Hero.Abilities.Count;

        for (int i = 0; i < slotCount; i++)
        {
            AbilitySlot abilitySlot = new();
            abilitySlot.Hero = _mapHero.Hero;
            _abilitySlots.Add(abilitySlot);
            Add(abilitySlot);
            abilitySlot.OnAbilityAdded += OnAbilityAdded;
            abilitySlot.OnAbilityRemoved += OnAbilityRemoved;
        }

        for (int i = 0; i < _mapHero.Hero.Abilities.Count; i++)
            _abilitySlots[i].AddActionButton(new(_mapHero.Hero.Abilities[i]));
    }

    void OnAbilityAdded(Ability ability) { _mapHero.Hero.AddAbility(ability); }
    void OnAbilityRemoved(Ability ability) { _mapHero.Hero.RemoveAbility(ability); }


}
