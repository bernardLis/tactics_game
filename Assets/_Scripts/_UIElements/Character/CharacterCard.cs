using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCard : VisualElement
{
    public Character Character;

    public CharacterPortraitElement PortraitVisualElement;
    StarRankElement _rankElement;
    GoldElement _wageGoldElement;
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

    bool _showOnlyExp;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonTextSecondary = "common__text-secondary";


    const string _ussClassName = "character-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanel = _ussClassName + "top-panel";
    const string _ussTopLeftPanel = _ussClassName + "top-left-panel";

    const string _ussBottomPanel = _ussClassName + "bottom-panel";
    const string _ussBottomLeftPanel = _ussClassName + "bottom-left-panel";
    const string _ussBottomRightPanel = _ussClassName + "bottom-right-panel";

    const string _ussElementPosition = _ussClassName + "element-position";
    const string _ussExpContainer = _ussClassName + "exp-container";
    const string _ussHealthContainer = _ussClassName + "health-container";
    const string _ussManaContainer = _ussClassName + "mana-container";

    public CharacterCard(Character character)
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
        VisualElement topLeftPanel = new();
        VisualElement topRightPanel = new();
        topPanel.Add(topLeftPanel);
        topPanel.Add(topRightPanel);

        VisualElement bottomPanel = new();
        bottomPanel.AddToClassList(_ussBottomPanel);
        VisualElement bottomLeftPanel = new();
        VisualElement bottomRightPanel = new();
        bottomPanel.Add(bottomLeftPanel);
        bottomPanel.Add(bottomRightPanel);

        PopulateTopLeftPanel(topLeftPanel);
        PopulateTopRightPanel(topRightPanel);
        PopulateBottomLeftPanel(bottomLeftPanel);
        PopulateBottomRightPanel(bottomRightPanel);

        Add(topPanel);
        Add(bottomPanel);

        Character.OnRankChanged += OnRankChanged;
        Character.OnElementChanged += OnElementChanged;
        Character.OnWageChanged += _wageGoldElement.ChangeAmount;
        SubscribeToStatChanges();

        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);
    }


    void OnRankChanged(CharacterRank rank)
    {
        _rankElement.SetRank(rank.Rank);
        _title.text = $"[{rank.Title}] {Character.CharacterName}";
    }

    void OnElementChanged(Element element) { _elementalElement.ChangeElement(element); }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        Character.OnRankChanged -= OnRankChanged;
        Character.OnElementChanged -= OnElementChanged;
    }

    void PopulateTopLeftPanel(VisualElement container)
    {
        container.AddToClassList(_ussTopLeftPanel);
        PortraitVisualElement = new(Character, this);
        container.Add(PortraitVisualElement);
        container.Add(CreateWageElement());
    }

    void PopulateTopRightPanel(VisualElement container)
    {
        VisualElement elementAndRank = new();
        elementAndRank.style.flexDirection = FlexDirection.Row;
        elementAndRank.Add(CreateElementalElement());
        elementAndRank.Add(CreateRankElement());
        container.Add(elementAndRank);

        _title = new($"[{Character.Rank.Title}] {Character.CharacterName}");
        container.Add(_title);

        container.Add(CreateHealthGroup());
        container.Add(CreateManaGroup());
        container.Add(CreateExpGroup());
    }
    void PopulateBottomLeftPanel(VisualElement container)
    {
        container.AddToClassList(_ussBottomLeftPanel);

        GameDatabase db = GameManager.Instance.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), Character.GetStatValue("Power"), "Power");
        _armor = new(db.GetStatIconByName("Armor"), Character.GetStatValue("Armor"), "Armor");
        _range = new(db.GetStatIconByName("MovementRange"), Character.GetStatValue("MovementRange"), "Movement Range");

        container.Add(_power);
        container.Add(_armor);
        container.Add(_range);
    }

    void PopulateBottomRightPanel(VisualElement container)
    {
        container.AddToClassList(_ussBottomRightPanel);
        container.Add(CreateAbilities());
        container.Add(CreateItems());
    }

    VisualElement CreateElementalElement()
    {
        _elementalElement = new ElementalElement(Character.Element);
        _elementalElement.AddToClassList(_ussElementPosition);
        return _elementalElement;
    }

    VisualElement CreateRankElement()
    {
        _rankElement = new(Character.Rank.Rank, 0.5f);
        return _rankElement;
    }

    VisualElement CreateWageElement()
    {
        _wageGoldElement = new(Character.WeeklyWage);
        _wageGoldElement.style.justifyContent = Justify.FlexEnd;
        _wageGoldElement.AddTooltip(new TooltipElement(_wageGoldElement, new Label("Weekly wage")));
        return _wageGoldElement;
    }

    VisualElement CreateExpGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussExpContainer);

        ExpBar = new(Color.black, "Experience", 100, Character.Experience, 0, true);

        _level = new Label($"Level {Character.Level}");
        _level.style.position = Position.Absolute;
        _level.AddToClassList(_ussCommonTextSecondary);
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
