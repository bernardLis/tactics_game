using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCard : VisualElement
{
    GameManager _gameManager;

    public Character Character;

    public CharacterPortraitElement PortraitVisualElement;
    StarRankElement _rankElement;
    GoldElement _wageGoldElement;
    ElementalElement _elementalElement;
    Label _title;
    Label _level;

    StatElement _power;
    StatElement _armor;
    StatElement _speed;

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
        _gameManager = GameManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;

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

        GameDatabase db = _gameManager.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), Character.Power);
        _armor = new(db.GetStatIconByName("Armor"), Character.Armor);
        _speed = new(db.GetStatIconByName("Speed"), Character.Speed);

        container.Add(_power);
        container.Add(_armor);
        container.Add(_speed);
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

        // TODO: this should be handled differently.
        IntVariable totalExp = ScriptableObject.CreateInstance<IntVariable>();
        totalExp.SetValue(100);
        ExpBar = new(Color.black, "Experience", Character.Experience, totalExp, null, 0, true);

        _level = new Label($"Level {Character.Level.Value}");
        _level.style.position = Position.Absolute;
        _level.AddToClassList(_ussCommonTextSecondary);
        ExpBar.Add(_level);

        container.Add(ExpBar);
        return container;
    }

    VisualElement CreateHealthGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussHealthContainer);

        // TODO: this should be handled differently.
        IntVariable currentHealth = ScriptableObject.CreateInstance<IntVariable>();
        currentHealth.SetValue(Character.Health.GetValue());
        Character.Health.OnValueChanged += currentHealth.SetValue;

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health", currentHealth, null, Character.Health);
        container.Add(HealthBar);

        return container;
    }

    VisualElement CreateManaGroup()
    {
        VisualElement container = new();
        container.AddToClassList(_ussManaContainer);

        // TODO: this should be handled differently.
        IntVariable currentMana = ScriptableObject.CreateInstance<IntVariable>();
        currentMana.SetValue(Character.Health.GetValue());
        Character.Mana.OnValueChanged += currentMana.SetValue;

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana", currentMana, null, Character.Mana);
        container.Add(ManaBar);

        return container;
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
}
