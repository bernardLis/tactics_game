using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardVisual : VisualElement
{
    VisualElement _information;
    CharacterStats _stats;
    Character _character;
    VisualElement _portrait;
    Label _name;
    Label _level;
    Label _exp;

    VisualElement _characteristics;

    public ResourceBarVisual HealthBar;
    public ResourceBarVisual ManaBar;

    StatVisual _power;
    StatVisual _armor;
    StatVisual _range;

    public CharacterCardVisual(Character character, bool clickable = true)
    {
        BaseCharacterCardVisual(character, clickable);
        _character = character;
        _characteristics.Add(HandleCharacterStats(character, null));
    }

    public CharacterCardVisual(CharacterStats stats, bool clickable = true)
    {
        BaseCharacterCardVisual(stats.Character, clickable);
        _stats = stats;
        _character = stats.Character;

        List<VisualElement> elements = new(HandleStatModifiers(stats));
        elements.AddRange(HandleStatuses(stats));
        foreach (VisualElement el in elements)
            _portrait.Add(el);

        _characteristics.Add(HandleCharacterStats(null, stats));

        // delegates
        _stats.OnHealthChange += HealthBar.OnValueChange;
        _stats.OnManaChange += ManaBar.OnValueChange;
        _stats.OnModifierAdded += OnModiferAdded;
        _stats.OnStatusAdded += OnStatusAdded;
        _stats.OnStatusRemoved += OnStatusRemoved;
        _stats.OnCharacterDeath += OnCharacterDeath;

        _character.OnCharacterExpGain += OnExpGain;
        _character.OnCharacterLevelUp += OnLevelUp;
    }

    void BaseCharacterCardVisual(Character character, bool clickable)
    {
        AddToClassList("characterCard");

        // group 1
        _information = new();
        _portrait = new();
        _name = new();

        _information.style.alignItems = Align.Center;
        _information.style.flexGrow = 1;
        _information.style.flexShrink = 0;
        _information.style.width = Length.Percent(30);

        _name.AddToClassList("primaryText");
        _portrait.AddToClassList("characterCardPortrait");

        _portrait.style.backgroundImage = character.Portrait.texture;
        _name.text = character.CharacterName;

        Add(_information);
        _information.Add(_name);
        _information.Add(_portrait);

        _characteristics = new();
        _characteristics.AddToClassList("characteristicGroup");

        _characteristics.Add(CreateHealthGroup(character));
        _characteristics.Add(CreateManaGroup(character));
        _characteristics.Add(CreateExpGroup(character));
        Add(_characteristics);

        RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        if (clickable)
        {
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            Button b = new Button();
            b.clicked += DisplayCharacterScreen;
            b.text = "Details";
            b.AddToClassList("menuButton");
            b.style.width = 80;
            b.style.height = 40;
            b.style.fontSize = 18;

            _information.Add(b);
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
        _stats.OnHealthChange -= HealthBar.OnValueChange;
        _stats.OnManaChange -= ManaBar.OnValueChange;
        _stats.OnModifierAdded -= OnModiferAdded;
        _stats.OnStatusAdded -= OnStatusAdded;
        _stats.OnStatusRemoved -= OnStatusRemoved;
        _stats.OnCharacterDeath += OnCharacterDeath;

        if (_character == null)
            return;
        _character.OnCharacterExpGain -= OnExpGain;
        _character.OnCharacterLevelUp -= OnLevelUp;
    }


    VisualElement CreateHealthGroup(Character character)
    {
        VisualElement healthGroup = new();
        healthGroup.style.flexDirection = FlexDirection.Row;
        healthGroup.style.width = Length.Percent(100);

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health");
        HealthBar.SetText(character.MaxHealth + "/" + character.MaxHealth);

        healthGroup.Add(HealthBar);

        return healthGroup;
    }

    VisualElement CreateManaGroup(Character character)
    {
        VisualElement manaGroup = new();
        manaGroup.style.flexDirection = FlexDirection.Row;
        manaGroup.style.width = Length.Percent(100);

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana");
        ManaBar.SetText(character.MaxMana + "/" + character.MaxMana);

        manaGroup.Add(ManaBar);

        return manaGroup;
    }

    VisualElement CreateExpGroup(Character character)
    {
        VisualElement container = new();
        container.style.alignSelf = Align.Center;

        VisualElement el = new();
        el.style.flexDirection = FlexDirection.Row;

        _level = new();
        _exp = new();

        _level.AddToClassList("secondaryText");
        _exp.AddToClassList("secondaryText");

        _level.text = $"Level {character.Level}";
        _exp.text = $"Exp: {character.Experience}/100";

        el.Add(_level);
        el.Add(_exp);

        container.Add(el);

        return container;
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
        CharacterDatabase db = GameManager.Instance.CharacterDatabase;
        _power = new(db.GetStatIconByName("Power"), character.Power, "Power");
        _armor = new(db.GetStatIconByName("Armor"), character.Armor, "Armor");
        _range = new(db.GetStatIconByName("MovementRange"), character.MovementRange, "Movement Range");
    }

    void CreateCharacterStats(CharacterStats characterStats)
    {
        CharacterDatabase db = GameManager.Instance.CharacterDatabase;
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

    // Delegates
    void OnExpGain(int gain)
    {
        _exp.text = $"Exp: {_character.Experience}/100";
    }

    void OnLevelUp()
    {
        _level.text = $"Level {_character.Level}";
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
            new CharacterScreen(_stats, root);
        else
            new CharacterScreen(_character, root);

    }

}
