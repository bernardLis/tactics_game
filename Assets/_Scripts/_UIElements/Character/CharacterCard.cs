using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCard : VisualElement
{
    public Character Character;

    public CharacterPortraitElement PortraitVisualElement;
    StarRankElement _rankElement;
    Label _title;
    Label _level;
    StatElement _power;
    StatElement _armor;
    StatElement _range;

    public ResourceBarElement ExpBar;
    public ResourceBarElement HealthBar;
    public ResourceBarElement ManaBar;

    public List<ItemSlot> ItemSlots = new();
    public List<ItemElement> ItemElements = new();

    public List<AbilitySlot> AbilitySlots = new();
    public List<AbilityButton> AbilityButtons = new();

    VisualElement _levelUpAnimationContainer;

    public CharacterCard(Character character, bool showExp = true, bool showAbilities = true, bool showItems = true)
    {
        Character = character;

        AddToClassList("characterCard");
        AddToClassList("textPrimary");

        VisualElement topPanel = new();
        topPanel.style.flexDirection = FlexDirection.Row;
        topPanel.style.alignItems = Align.Center;
        topPanel.style.justifyContent = Justify.Center;

        VisualElement portraitContainer = new();
        portraitContainer.style.alignItems = Align.Center;
        portraitContainer.style.marginRight = 20;

        PortraitVisualElement = new(character, this);
        Label name = new($"{character.CharacterName}");

        _title = new($"{Character.Rank.Title}");

        portraitContainer.Add(PortraitVisualElement);
        portraitContainer.Add(name);
        portraitContainer.Add(_title);

        VisualElement barsContainer = new();
        barsContainer.Add(CreateRankElement());
        barsContainer.Add(CreateStatGroup());
        barsContainer.Add(CreateExpGroup());
        barsContainer.Add(CreateHealthGroup());
        barsContainer.Add(CreateManaGroup());

        topPanel.Add(portraitContainer);
        topPanel.Add(barsContainer);

        VisualElement bottomPanel = new();
        bottomPanel.style.flexDirection = FlexDirection.Row;
        bottomPanel.style.alignItems = Align.Center;
        bottomPanel.style.justifyContent = Justify.Center;

        if (showAbilities)
            bottomPanel.Add(CreateAbilities());
        if (showItems)
            bottomPanel.Add(CreateItems());

        Add(topPanel);
        Add(bottomPanel);

        AvailabilityCheck(); // for armory
        Character.OnRankChanged += OnRankChanged;
        SubscribeToStatChanges();
    }

    void OnRankChanged(CharacterRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"{rank.Title}";
    }

    VisualElement CreateRankElement()
    {
        _rankElement = new(Character.Rank.Rank, 0.5f);

        return _rankElement;
    }

    VisualElement CreateStatGroup()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignContent = Align.Center;
        container.style.width = Length.Percent(100);

        GameDatabase db = GameManager.Instance.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), Character.GetStatValue("Power"), "Power");
        _armor = new(db.GetStatIconByName("Armor"), Character.GetStatValue("Armor"), "Armor");
        _range = new(db.GetStatIconByName("MovementRange"), Character.GetStatValue("MovementRange"), "Movement Range");

        container.Add(_power);
        container.Add(_armor);
        container.Add(_range);

        return container;
    }



    VisualElement CreateExpGroup()
    {
        VisualElement container = new();
        container.style.alignContent = Align.Center;
        container.style.width = Length.Percent(100);

        ExpBar = new(Color.black, "Experience", 100, Character.Experience, 0, true);
        _level = new Label($"Level {Character.Level}");
        _level.style.position = Position.Absolute;
        _level.AddToClassList("textSecondary");
        ExpBar.Add(_level);
        Character.OnCharacterExpGain += OnExpChange;
        Character.OnCharacterLevelUp += OnLevelUp;

        container.Add(ExpBar);
        return container;
    }

    VisualElement CreateHealthGroup()
    {
        VisualElement healthGroup = new();
        healthGroup.style.flexDirection = FlexDirection.Row;
        healthGroup.style.width = Length.Percent(100);

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health", Character.GetStatValue("MaxHealth"), Character.GetStatValue("MaxHealth"));
        healthGroup.Add(HealthBar);

        return healthGroup;
    }

    VisualElement CreateManaGroup()
    {
        VisualElement manaGroup = new();
        manaGroup.style.flexDirection = FlexDirection.Row;
        manaGroup.style.width = Length.Percent(100);

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana", Character.GetStatValue("MaxMana"), Character.GetStatValue("MaxMana"));
        manaGroup.Add(ManaBar);

        return manaGroup;
    }

    void OnExpChange(int expGain) { ExpBar.OnValueChanged(expGain, 3000); }

    void OnLevelUp()
    {
        _level.text = $"Level {Character.Level}";
        PlayLevelUpAnimation();
    }

    VisualElement CreateItems()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        for (int i = 0; i < 2; i++)
        {
            ItemSlot itemSlot = new();
            itemSlot.OnItemAdded += OnItemAdded;
            itemSlot.OnItemRemoved += OnItemRemoved;

            itemSlot.Character = Character;
            ItemSlots.Add(itemSlot);
            container.Add(itemSlot);
        }

        for (int i = 0; i < Character.Items.Count; i++)
        {
            ItemElement itemVisual = new ItemElement(Character.Items[i]);
            ItemSlots[i].AddItemNoDelegates(itemVisual);
            ItemElements.Add(itemVisual);
        }

        return container;
    }

    void OnItemAdded(ItemElement itemElement) { Character.AddItem(itemElement.Item); }

    void OnItemRemoved(ItemElement itemElement)
    {
        Debug.Log($"item removed {itemElement.Item.ItemName}");

        Character.RemoveItem(itemElement.Item);
    }


    VisualElement CreateAbilities()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        int slotCount = Character.GetNumberOfAbilitySlots();

        if (slotCount < Character.Abilities.Count)
            slotCount = Character.Abilities.Count;

        for (int i = 0; i < slotCount; i++)
        {
            AbilitySlot abilitySlot = new();
            abilitySlot.Character = Character;
            AbilitySlots.Add(abilitySlot);
            container.Add(abilitySlot);
        }

        for (int i = 0; i < Character.Abilities.Count; i++)
        {
            if (i > slotCount)
                break;
            AbilityButton abilityButton = new AbilityButton(Character.Abilities[i], null);
            AbilitySlots[i].AddButton(abilityButton);
            AbilityButtons.Add(abilityButton);
        }

        return container;
    }

    void AvailabilityCheck()
    {
        if (!Character.IsUnavailable)
            return;

        VisualElement overlay = new VisualElement();
        Add(overlay);
        overlay.BringToFront();
        overlay.style.position = Position.Absolute;
        overlay.style.width = Length.Percent(100);
        overlay.style.height = Length.Percent(100);
        overlay.style.alignSelf = Align.Center;
        overlay.style.alignItems = Align.Center;
        overlay.style.justifyContent = Justify.Center;
        overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));

        Label text = new($"Unavailable! ({Character.UnavailabilityDuration})");
        text.AddToClassList("textPrimary");
        text.style.fontSize = 60;
        text.transform.rotation *= Quaternion.Euler(0f, 0f, 30f);
        overlay.Add(text);
    }

    public void PlayLevelUpAnimation()
    {
        Sprite[] animationSprites = GameManager.Instance.GameDatabase.LevelUpAnimationSprites;

        _levelUpAnimationContainer = new();
        _levelUpAnimationContainer.style.position = Position.Absolute;
        _levelUpAnimationContainer.style.width = Length.Percent(100);
        _levelUpAnimationContainer.style.height = Length.Percent(100);
        _levelUpAnimationContainer.Add(new AnimationElement(animationSprites, 100, false));
        Add(_levelUpAnimationContainer);
    }

    void SubscribeToStatChanges()
    {
        Character.OnMaxHealthChanged += OnMaxHealthChanged;
        Character.OnMaxManaChanged += OnMaxManaChanged;

        Character.OnPowerChanged += _power.UpdateBaseValue;
        Character.OnArmorChanged += _armor.UpdateBaseValue;
        Character.OnMovementRangeChanged += _range.UpdateBaseValue;
    }

    void OnMaxHealthChanged(int value) { HealthBar.UpdateBarValues(value, value); }

    void OnMaxManaChanged(int value) { ManaBar.UpdateBarValues(value, value); }
}
