using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCard : VisualElement
{
    public Character Character;

    public CharacterPortraitElement PortraitVisualElement;
    StarRankElement _rankElement;
    ElementalElement _elementalElement;
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

    VisualElement _levelUpAnimationContainer;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "character-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussPortraitContainer = _ussClassName + "portrait-container";
    const string _ussElementPosition = _ussClassName + "element-position";
    const string _ussBottomPanel = _ussClassName + "bottom-panel";
    const string _ussStatContainer = _ussClassName + "stat-container";
    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussHealthContainer = _ussClassName + "health-container";
    const string _ussManaContainer = _ussClassName + "mana-container";
    const string _ussOverlay = _ussClassName + "overlay";

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
        _elementalElement = new ElementalElement(Character.Element);
        _elementalElement.AddToClassList(_ussElementPosition);
        portraitContainer.Add(_elementalElement);

        _title = new($"[{Character.Rank.Title}] {character.CharacterName}");
        portraitContainer.Add(CreateRankElement());

        VisualElement barsContainer = new();
        barsContainer.Add(_title);
        barsContainer.Add(CreateHealthGroup());
        barsContainer.Add(CreateManaGroup());
        barsContainer.Add(CreateExpGroup());
        barsContainer.Add(CreateStatGroup());

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

        Character.OnRankChanged += OnRankChanged;
        Character.OnElementChanged += OnElementChanged;
        SubscribeToStatChanges();

        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);
    }

    void OnRankChanged(CharacterRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"[{rank.Title}] {Character.CharacterName}";
    }

    void OnElementChanged(Element element)
    {
        _elementalElement.ChangeElement(element);
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        Character.OnRankChanged -= OnRankChanged;
        Character.OnElementChanged -= OnElementChanged;
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
        for (int i = 0; i < Character.MaxCharacterItems; i++)
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

        for (int i = 0; i < Character.MaxCharacterAbilities; i++)
        {
            AbilitySlot abilitySlot = new();
            abilitySlot.Character = Character;
            AbilitySlots.Add(abilitySlot);
            container.Add(abilitySlot);
            abilitySlot.OnAbilityAdded += OnAbilityAdded;
            abilitySlot.OnAbilityRemoved += OnAbilityRemoved;
        }

        for (int i = 0; i < Character.Abilities.Count; i++)
        {
            DraggableAbilities da = null;
            if (DeskManager.Instance != null)
                da = DeskManager.Instance.GetComponent<DraggableAbilities>();

            AbilitySlots[i].AddDraggableButtonNoDelegates(Character.Abilities[i], da);
        }

        return container;
    }

    void OnAbilityAdded(Ability ability) { Character.AddAbility(ability); }
    void OnAbilityRemoved(Ability ability) { Character.RemoveAbility(ability); }

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
