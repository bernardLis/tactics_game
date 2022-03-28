using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCardVisual : VisualElement
{
    VisualElement _information;
    Image _portrait;
    Label _name;

    VisualElement _characteristics;

    public ResourceBarVisual HealthBar;
    public ResourceBarVisual ManaBar;

    StatVisual _strength;
    StatVisual _intelligence;
    StatVisual _agility;
    StatVisual _stamina;
    StatVisual _armor;
    StatVisual _range;

    public CharacterCardVisual(Character character)
    {
        AddToClassList("characterCard");

        character.UpdateDerivativeStats();

        // group 1
        _information = new();
        _portrait = new();
        _name = new();

        _information.style.alignItems = Align.Center;
        _information.style.width = Length.Percent(30);

        _name.style.color = Color.white;
        _name.style.fontSize = 18;
        _name.style.alignSelf = Align.Center;

        _portrait.sprite = character.Portrait;
        _name.text = character.CharacterName;

        Add(_information);
        _information.Add(_name);
        _information.Add(_portrait);

        // TODO: missing skull on portrait
        // TODO: missing characterCardModifierContainer

        // group2
        _characteristics = new();
        _characteristics.AddToClassList("characteristicGroup");

        _characteristics.Add(CreateHealthGroup(character));
        _characteristics.Add(CreateManaGroup(character));
        _characteristics.Add(CreateCharacterStats(character));

        Add(_characteristics);
    }

    VisualElement CreateHealthGroup(Character character)
    {
        VisualElement healthGroup = new();
        healthGroup.style.flexDirection = FlexDirection.Row;
        healthGroup.style.width = Length.Percent(100);

        Label healthLabel = new Label();
        healthLabel.AddToClassList("healthLabel");

        HealthBar = new(Helpers.GetColor("healthBarRed"));
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

        ManaBar = new(Helpers.GetColor("manaBarBlue"));
        ManaBar.SetText(character.MaxMana + "/" + character.MaxMana);

        manaGroup.Add(manaLabel);
        manaGroup.Add(ManaBar);

        return manaGroup;
    }

    VisualElement CreateCharacterStats(Character character)
    {
        VisualElement statsGroup = new();
        statsGroup.AddToClassList("statsGroup");

        // TODO: this all should be coming from characters stats not from Character 
        // TODO: actually, I should rethink how I approach stats and Character most def should know it's stats 

        CharacterDatabase db = JourneyManager.instance.CharacterDatabase;
        _strength = new(db.GetStatIconByName("Strength"), character.Strength);
        _intelligence = new(db.GetStatIconByName("Intelligence"), character.Intelligence);
        _agility = new(db.GetStatIconByName("Agility"), character.Agility);
        _stamina = new(db.GetStatIconByName("Stamina"), character.Stamina);
        _armor = new(db.GetStatIconByName("Armor"), character.Armor);
        _range = new(db.GetStatIconByName("MovementRange"), character.MovementRange);

        statsGroup.Add(_strength);
        statsGroup.Add(_intelligence);
        statsGroup.Add(_agility);
        statsGroup.Add(_stamina);
        statsGroup.Add(_armor);
        statsGroup.Add(_range);

        return statsGroup;
    }
}
