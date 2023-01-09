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

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "character-card";
    const string _ussMain = _ussClassName + "__main";
    const string _ussTopPanel = _ussClassName + "__top-panel";
    const string _ussPortraitContainer = _ussClassName + "__portrait-container";
    const string _ussBottomPanel = _ussClassName + "__bottom-panel";
    const string _ussStatContainer = _ussClassName + "__stat-container";
    const string _ussExpContainer = _ussClassName + "__exp-container";
    const string _ussHealthContainer = _ussClassName + "__health-container";
    const string _ussManaContainer = _ussClassName + "__mana-container";
    const string _ussOverlay = _ussClassName + "__overlay";

    public CharacterCard(Character character, bool showExp = true, bool showAbilities = true, bool showItems = true)
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;
        Character.ResolveItems();

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        VisualElement topPanel = new();
        topPanel.AddToClassList(_ussTopPanel);

        VisualElement portraitContainer = new();
        portraitContainer.AddToClassList(_ussPortraitContainer);

        PortraitVisualElement = new(character, this);
        portraitContainer.Add(PortraitVisualElement);

        _title = new($"[{Character.Rank.Title}] {character.CharacterName}");
        portraitContainer.Add(_title);

        VisualElement rankAndElementContainer = new();
        rankAndElementContainer.style.flexDirection = FlexDirection.Row;
        rankAndElementContainer.Add(new ElementElement(Character.Element));
        rankAndElementContainer.Add(CreateRankElement());

        VisualElement barsContainer = new();
        barsContainer.Add(rankAndElementContainer);
        barsContainer.Add(CreateStatGroup());
        barsContainer.Add(CreateExpGroup());
        barsContainer.Add(CreateHealthGroup());
        barsContainer.Add(CreateManaGroup());

        topPanel.Add(portraitContainer);
        topPanel.Add(barsContainer);

        VisualElement bottomPanel = new();
        bottomPanel.AddToClassList(_ussBottomPanel);

        if (showAbilities)
            bottomPanel.Add(CreateAbilities());
        if (showItems)
            bottomPanel.Add(CreateItems());

        Add(topPanel);
        Add(bottomPanel);

        AvailabilityCheck();
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
        container.AddToClassList(_ussStatContainer);

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
        container.AddToClassList(_ussExpContainer);

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
        VisualElement container = new();
        container.AddToClassList(_ussHealthContainer);

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health", Character.GetStatValue("MaxHealth"), Character.GetStatValue("MaxHealth"));
        container.Add(HealthBar);

        return container;
    }

    VisualElement CreateManaGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussManaContainer);

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana", Character.GetStatValue("MaxMana"), Character.GetStatValue("MaxMana"));
        container.Add(ManaBar);

        return container;
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

    void OnItemRemoved(ItemElement itemElement) { Character.RemoveItem(itemElement.Item); }

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
        overlay.AddToClassList(_ussOverlay);

        Label text = new($"Unavailable! ({Character.UnavailabilityDuration})");
        text.AddToClassList(_ussCommonTextPrimary);
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
