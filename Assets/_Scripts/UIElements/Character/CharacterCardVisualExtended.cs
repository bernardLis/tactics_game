using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardVisualExtended : CharacterCardVisual
{
    Label _level;
    Label _exp;

    public List<ItemSlotVisual> ItemSlots = new();
    public List<ItemVisual> ItemVisuals = new();

    public List<AbilitySlotVisual> AbilitySlots = new();
    public List<AbilityButton> AbilityButtons = new();

    public CharacterCardVisualExtended(Character character) : base(character, false)
    {
        AddToClassList("uiContainer");
        style.maxHeight = 400;
        style.maxWidth = 600;

        _characteristics.Add(CreateExpGroup(character));

        VisualElement wrapper = new();
        wrapper.style.flexDirection = FlexDirection.Row;
        wrapper.style.width = Length.Percent(100);
        wrapper.Add(_portrait);
        _portrait.style.minWidth = 200;
        wrapper.Add(_characteristics);
        wrapper.Add(CreateItems(character));
        Add(wrapper);

        style.flexDirection = FlexDirection.Column;
        Add(CreateAbilities(character));
    }


    VisualElement CreateExpGroup(Character character)
    {
        VisualElement container = new();
        container.style.alignSelf = Align.Center;

        VisualElement el = new();
        el.style.flexDirection = FlexDirection.Row;

        _level = new();
        _exp = new();

        _level.AddToClassList("textSecondary");
        _exp.AddToClassList("textSecondary");

        _level.text = $"Level {character.Level}";
        _exp.text = $"Exp: {character.Experience}/100";

        el.Add(_level);
        el.Add(_exp);

        container.Add(el);

        return container;
    }

    VisualElement CreateItems(Character character)
    {
        VisualElement container = new();
        for (int i = 0; i < 3; i++)
        {
            ItemSlotVisual itemSlot = new();
            itemSlot.Character = character;
            ItemSlots.Add(itemSlot);
            container.Add(itemSlot);
        }

        for (int i = 0; i < character.Items.Count; i++)
        {
            ItemVisual itemVisual = new ItemVisual(character.Items[i]);
            ItemSlots[i].AddItem(itemVisual);
            ItemVisuals.Add(itemVisual);
        }

        return container;
    }

    VisualElement CreateAbilities(Character character)
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        int slotCount = character.GetNumberOfAbilitySlots();

        if (slotCount < character.Abilities.Count)
            slotCount = character.Abilities.Count;
        
        for (int i = 0; i < slotCount; i++)
        {
            AbilitySlotVisual abilitySlot = new();
            abilitySlot.Character = character;
            AbilitySlots.Add(abilitySlot);
            container.Add(abilitySlot);
        }

        for (int i = 0; i < character.Abilities.Count; i++)
        {
            if (i > slotCount)
                break;
            AbilityButton abilityButton = new AbilityButton(character.Abilities[i], null);
            AbilitySlots[i].AddButton(abilityButton);
            AbilityButtons.Add(abilityButton);
        }

        return container;
    }
}
