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

    StatVisual _strength;
    StatVisual _intelligence;
    StatVisual _agility;
    StatVisual _stamina;
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

        HealthBar.DisplayMissingAmount(stats.MaxHealth.GetValue(), stats.CurrentHealth);
        ManaBar.DisplayMissingAmount(stats.MaxMana.GetValue(), stats.CurrentMana);
    }

    void BaseCharacterCardVisual(Character character, bool clickable)
    {
        AddToClassList("characterCard");

        character.UpdateDerivativeStats();

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

        if (clickable)
            RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    VisualElement CreateHealthGroup(Character character)
    {
        VisualElement healthGroup = new();
        healthGroup.style.flexDirection = FlexDirection.Row;
        healthGroup.style.width = Length.Percent(100);

        Label healthLabel = new Label();
        healthLabel.AddToClassList("healthLabel");

        HealthBar = new(Helpers.GetColor("healthBarRed"), "Health");
        HealthBar.SetText(character.MaxHealth + "/" + character.MaxHealth);

        healthGroup.Add(healthLabel);
        healthGroup.Add(HealthBar);

        return healthGroup;
    }

    VisualElement CreateManaGroup(Character character)
    {
        VisualElement manaGroup = new();
        manaGroup.style.flexDirection = FlexDirection.Row;
        manaGroup.style.width = Length.Percent(100);

        Label manaLabel = new Label();
        manaLabel.AddToClassList("manaLabel");

        ManaBar = new(Helpers.GetColor("manaBarBlue"), "Mana");
        ManaBar.SetText(character.MaxMana + "/" + character.MaxMana);

        manaGroup.Add(manaLabel);
        manaGroup.Add(ManaBar);

        return manaGroup;
    }

    VisualElement CreateExpGroup(Character character)
    {
        VisualElement container = new();

        VisualElement el = new();
        el.style.flexDirection = FlexDirection.Row;

        Label level = new();
        Label exp = new();

        level.AddToClassList("secondaryText");
        exp.AddToClassList("secondaryText");

        level.text = $"Level {character.Level}";
        exp.text = $"Exp: {character.Experience}/100";

        el.Add(level);
        el.Add(exp);

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

        statsGroup.Add(_strength);
        statsGroup.Add(_intelligence);
        statsGroup.Add(_agility);
        statsGroup.Add(_stamina);
        statsGroup.Add(_armor);
        statsGroup.Add(_range);

        return statsGroup;
    }

    void CreateCharacterStatsChar(Character character)
    {
        CharacterDatabase db = GameManager.Instance.CharacterDatabase;
        _strength = new(db.GetStatIconByName("Strength"), character.Strength, "Strength");
        _intelligence = new(db.GetStatIconByName("Intelligence"), character.Intelligence, "Intelligence");
        _agility = new(db.GetStatIconByName("Agility"), character.Agility, "Agility");
        _stamina = new(db.GetStatIconByName("Stamina"), character.Stamina, "Stamina");
        _armor = new(db.GetStatIconByName("Armor"), character.Armor, "Armor");
        _range = new(db.GetStatIconByName("MovementRange"), character.MovementRange, "Movement Range");
    }

    void CreateCharacterStats(CharacterStats characterStats)
    {
        CharacterDatabase db = GameManager.Instance.CharacterDatabase;
        _strength = new(db.GetStatIconByName("Strength"), characterStats.Strength);
        _intelligence = new(db.GetStatIconByName("Intelligence"), characterStats.Intelligence);
        _agility = new(db.GetStatIconByName("Agility"), characterStats.Agility);
        _stamina = new(db.GetStatIconByName("Stamina"), characterStats.Stamina);
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

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0) // only left mouse click
            return;

        var root = panel.visualTree;
        if (_stats != null)
            new CharacterScreen(_stats, root);
        else
            new CharacterScreen(_character, root);
    }
}
