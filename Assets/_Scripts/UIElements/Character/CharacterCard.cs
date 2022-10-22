using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCard : VisualElement
{
    VisualElement _information;
    CharacterStats _stats;
    public Character Character;
    protected VisualElement _portrait;
    Label _name;

    protected VisualElement _characteristics;

    public ResourceBarVisual HealthBar;
    public ResourceBarVisual ManaBar;

    StatVisual _power;
    StatVisual _armor;
    StatVisual _range;

    public CharacterCard(Character character, bool clickable = true)
    {
        character.ResolveItems();
        BaseCharacterCardVisual(character, clickable);
        Character = character;
        _characteristics.Add(HandleCharacterStats(character, null));

        // delegates
        character.OnMaxHealthChanged += HealthBar.OnTotalChanged;
        character.OnMaxManaChanged += ManaBar.OnTotalChanged;
        character.OnPowerChanged += _power.OnValueChanged;
        character.OnArmorChanged += _armor.OnValueChanged;
        character.OnMovementRangeChanged += _range.OnValueChanged;
    }

    public CharacterCard(CharacterStats stats, bool clickable = true)
    {
        BaseCharacterCardVisual(stats.Character, clickable);
        _stats = stats;
        Character = stats.Character;

        List<VisualElement> elements = new(HandleStatModifiers(stats));
        elements.AddRange(HandleStatuses(stats));
        foreach (VisualElement el in elements)
            _portrait.Add(el);

        _characteristics.Add(HandleCharacterStats(null, stats));

        // delegates
        _stats.OnHealthChanged += HealthBar.OnValueChanged;
        _stats.OnManaChanged += ManaBar.OnValueChanged;
        _stats.OnModifierAdded += OnModiferAdded;
        _stats.OnStatusAdded += OnStatusAdded;
        _stats.OnStatusRemoved += OnStatusRemoved;
        _stats.OnCharacterDeath += OnCharacterDeath;
    }

    void BaseCharacterCardVisual(Character character, bool clickable)
    {
        AddToClassList("characterCard");

        // group 1
        VisualElement nameContainer = new VisualElement(); // will hold name and more info button

        _information = new();
        _portrait = new();
        _name = new();

        _information.style.alignItems = Align.Center;
        _information.style.flexGrow = 1;
        _information.style.flexShrink = 0;
        _information.style.width = Length.Percent(30);

        _name.AddToClassList("textPrimary");
        _portrait.AddToClassList("characterCardPortrait");

        _portrait.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);
        _name.text = character.CharacterName;

        Add(_information);
        _information.Add(_portrait);

        _characteristics = new();
        _characteristics.AddToClassList("characteristicGroup");

        nameContainer.Add(_name);
        nameContainer.style.flexDirection = FlexDirection.Row;
        nameContainer.style.alignItems = Align.Center;
        nameContainer.style.width = Length.Percent(100);
        nameContainer.style.justifyContent = Justify.SpaceBetween;

        _characteristics.Add(nameContainer);
        _characteristics.Add(CreateHealthGroup(character));
        _characteristics.Add(CreateManaGroup(character));
        Add(_characteristics);

        RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        if (clickable)
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            ButtonWithTooltip b = new ButtonWithTooltip("characterCardDetailsButton", "See more info", DisplayCharacterScreen);
            nameContainer.Add(b);
        }
    }

    private void GeometryChangedCallback(GeometryChangedEvent evt)
    {
        //https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
        UnregisterCallback<GeometryChangedEvent>(GeometryChangedCallback);

        if (_stats == null)
            return;
        // Do what you need to do here, as geometry should be calculated.
        HealthBar.DisplayMissingAmount(_stats.MaxHealth.GetValue(), _stats.CurrentHealth);
        ManaBar.DisplayMissingAmount(_stats.MaxMana.GetValue(), _stats.CurrentMana);
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        if (_stats == null)
            return;
        _stats.OnHealthChanged -= HealthBar.OnValueChanged;
        _stats.OnManaChanged -= ManaBar.OnValueChanged;
        _stats.OnModifierAdded -= OnModiferAdded;
        _stats.OnStatusAdded -= OnStatusAdded;
        _stats.OnStatusRemoved -= OnStatusRemoved;
        _stats.OnCharacterDeath += OnCharacterDeath;

        if (Character == null)
            return;
    }

    VisualElement CreateHealthGroup(Character character)
    {
        VisualElement healthGroup = new();
        healthGroup.style.flexDirection = FlexDirection.Row;
        healthGroup.style.width = Length.Percent(100);

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health");
        HealthBar.SetText(character.GetStatValue("MaxHealth") + "/" + character.GetStatValue("MaxHealth"));

        healthGroup.Add(HealthBar);

        return healthGroup;
    }

    VisualElement CreateManaGroup(Character character)
    {
        VisualElement manaGroup = new();
        manaGroup.style.flexDirection = FlexDirection.Row;
        manaGroup.style.width = Length.Percent(100);

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana");
        ManaBar.SetText(character.GetStatValue("MaxMana") + "/" + character.GetStatValue("MaxMana"));

        manaGroup.Add(ManaBar);

        return manaGroup;
    }

    VisualElement HandleCharacterStats(Character character, CharacterStats characterStats)
    {
        VisualElement statsGroup = new();
        statsGroup.AddToClassList("statsGroup");

        if (characterStats == null)
            CreateCharacterStatsChar(character);
        else
            CreateCharacterStats(characterStats);

        statsGroup.Add(_power);
        statsGroup.Add(_armor);
        statsGroup.Add(_range);

        return statsGroup;
    }

    void CreateCharacterStatsChar(Character character)
    {
        GameDatabase db = GameManager.Instance.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), character.GetStatValue("Power"), "Power");
        _armor = new(db.GetStatIconByName("Armor"), character.GetStatValue("Armor"), "Armor");
        _range = new(db.GetStatIconByName("MovementRange"), character.GetStatValue("MovementRange"), "Movement Range");
    }

    void CreateCharacterStats(CharacterStats characterStats)
    {
        GameDatabase db = GameManager.Instance.GameDatabase;
        _power = new(db.GetStatIconByName("Power"), characterStats.Power);
        _armor = new(db.GetStatIconByName("Armor"), characterStats.Armor);
        _range = new(db.GetStatIconByName("MovementRange"), characterStats.MovementRange);
    }

    public List<ModifierVisual> HandleStatuses(CharacterStats stats)
    {
        List<ModifierVisual> els = new();
        if (stats.Statuses.Count == 0)
            return els;

        foreach (Status s in stats.Statuses)
        {
            ModifierVisual mElement = new ModifierVisual(s);
            els.Add(mElement);
        }
        return els;
    }

    public List<ModifierVisual> HandleStatModifiers(CharacterStats stats)
    {
        List<ModifierVisual> els = new();
        foreach (Stat s in stats.Stats)
        {
            List<StatModifier> modifiers = s.GetActiveModifiers();
            if (modifiers.Count == 0)
                continue;

            foreach (StatModifier m in modifiers)
            {
                ModifierVisual mElement = new ModifierVisual(m);
                els.Add(mElement);
            }
        }
        return els;
    }

    void OnModiferAdded(StatModifier mod)
    {
        ModifierVisual mElement = new ModifierVisual(mod);
        _portrait.Add(mElement);
    }

    void OnStatusAdded(Status status)
    {
        ModifierVisual mElement = new ModifierVisual(status);
        _portrait.Add(mElement);
    }

    void OnStatusRemoved(Status status)
    {
        ModifierVisual elToRemove = null;
        foreach (ModifierVisual el in _portrait.Children())
        {
            if (el.Status == null) // some elements are stat modifier elements... duh...
                continue;
            if (el.Status.ReferenceID == status.ReferenceID)
                elToRemove = el;
        }

        if (elToRemove != null)
            elToRemove.RemoveSelf(_portrait);
    }

    void OnCharacterDeath(GameObject obj)
    {
        // TODO: destroy the card in a cool way
        InfoCardUI.Instance.HideCharacterCard();
    }

    // clicks
    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0) // only left mouse click
            return;
        DisplayCharacterScreen();
    }

    void DisplayCharacterScreen()
    {
        var root = panel.visualTree;
        if (_stats != null)
            BattleUI.Instance.ShowCharacterScreen(_stats.Character);
        else
            BattleUI.Instance.ShowCharacterScreen(Character);
    }

}
