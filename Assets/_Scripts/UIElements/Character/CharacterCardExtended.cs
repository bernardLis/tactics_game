using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardExtended : CharacterCard
{
    Label _level;
    public ResourceBarVisual ExpBar;

    public List<ItemSlotVisual> ItemSlots = new();
    public List<ItemVisual> ItemVisuals = new();

    public List<AbilitySlotVisual> AbilitySlots = new();
    public List<AbilityButton> AbilityButtons = new();

    Sprite[] _levelUpAnimationSprites;
    VisualElement _levelUpAnimationContainer;
    IVisualElementScheduledItem _levelUpAnimation;
    int _levelUpSpriteIndex = 0;


    public CharacterCardExtended(Character character) : base(character, false)
    {
        AddToClassList("uiContainer");
        style.width = 600;
        style.height = 400;

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

        AvailablityCheck(); // for armory
    }

    VisualElement CreateExpGroup(Character character)
    {
        VisualElement container = new();
        container.style.alignContent = Align.Center;
        container.style.width = Length.Percent(100);

        _level = new();
        ExpBar = new(Color.black, "Experiance", 100, character.Experience, 0, true);

        _level.AddToClassList("textPrimary");
        _level.text = $"Level {character.Level}";

        character.OnCharacterExpGain += OnExpChange;
        character.OnCharacterLevelUp += OnLevelUp;

        container.Add(_level);
        container.Add(ExpBar);

        return container;
    }

    void OnExpChange(int expGain) { ExpBar.OnValueChanged(expGain, 3000); }

    void OnLevelUp()
    {
        _level.text = $"Level {Character.Level}";
        PlayLevelUpAnimation();
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

    void AvailablityCheck()
    {
        if (!Character.IsUnavailable)
            return;

        VisualElement overlay = new VisualElement();
        Add(overlay);
        overlay.BringToFront();
        overlay.style.position = Position.Absolute;
        overlay.style.width = Length.Percent(105);
        overlay.style.height = Length.Percent(105);
        overlay.style.alignSelf = Align.Center;
        overlay.style.alignItems = Align.Center;
        overlay.style.justifyContent = Justify.Center;
        overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));

        Label text = new($"Unavailable! ({Character.UnavailabilityDuration})");
        text.AddToClassList("textPrimary");
        text.style.fontSize = 60;
        text.transform.rotation *= Quaternion.Euler(0f, 0f, 45f);
        overlay.Add(text);
    }

    public void PlayLevelUpAnimation()
    {
        _levelUpAnimationSprites = GameManager.Instance.GameDatabase.LevelUpAnimationSprites;

        _levelUpAnimationContainer = new();
        _levelUpAnimationContainer.style.position = Position.Absolute;
        _levelUpAnimationContainer.style.width = Length.Percent(100);
        _levelUpAnimationContainer.style.height = Length.Percent(100);
        Add(_levelUpAnimationContainer);

        _levelUpAnimation = _levelUpAnimationContainer.schedule.Execute(LevelUpAnimation).Every(100);
    }

    void LevelUpAnimation()
    {
        _levelUpAnimationContainer.style.backgroundImage = new StyleBackground(_levelUpAnimationSprites[_levelUpSpriteIndex]);
        _levelUpSpriteIndex++;
        if (_levelUpSpriteIndex == _levelUpAnimationSprites.Length)
        {
            _levelUpAnimation.Pause();
            Remove(_levelUpAnimationContainer);
        }
    }


}
