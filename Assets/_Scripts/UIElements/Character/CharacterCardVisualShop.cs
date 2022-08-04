using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardVisualShop : CharacterCardVisual
{

    Label _level;
    Label _exp;

    public List<ItemSlotVisual> ItemSlots = new();
    public List<ItemVisual> ItemVisuals = new();

    public CharacterCardVisualShop(Character character) : base(character, false)
    {
        BaseCharacterCardVisualExtended(character);
    }

    void BaseCharacterCardVisualExtended(Character character)
    {
        style.width = Length.Percent(50);
        _characteristics.Add(CreateExpGroup(character));
        Add(CreateItems(character));
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
}
