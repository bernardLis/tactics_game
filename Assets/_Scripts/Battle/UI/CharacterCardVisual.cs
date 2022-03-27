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

    VisualElement _healthBar;
    VisualElement _missingHealth;
    VisualElement _healthBarInteractionResult;
    Label _healhtText;

    VisualElement _manaBar;
    VisualElement _missingMana;
    VisualElement _manaBarInteractionResult;
    Label _manaText;

    public CharacterCardVisual(Character character)
    {
        AddToClassList("characterCard");

        // group 1
        _information = new();
        _portrait = new();
        _name = new();

        _information.style.alignContent = Align.Center;
        _information.style.justifyContent = Justify.Center;
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

        Add(_characteristics);
    }


    VisualElement CreateHealthGroup(Character character)
    {
        VisualElement healthGroup = new();
        healthGroup.style.flexDirection = FlexDirection.Row;
        healthGroup.style.width = Length.Percent(100);

        Label healthLabel = new Label();
        healthLabel.AddToClassList("healthLabel");

        _healthBar = new();
        _missingHealth = new();
        _healthBarInteractionResult = new();
        _healhtText = new();

        _healthBar.AddToClassList("resourceBar");
        _missingHealth.AddToClassList("barMissingAmount");
        _healthBarInteractionResult.AddToClassList("barInteractionResult");
        _healhtText.AddToClassList("barText");

        int maxHealth = 100 + character.Stamina * 5;        
        _healhtText.text = maxHealth + "/" + maxHealth;

        _healthBar.Add(_missingHealth);
        _healthBar.Add(_healthBarInteractionResult);
        _healthBar.Add(_healhtText);

        healthGroup.Add(healthLabel);
        healthGroup.Add(_healthBar);

        return healthGroup;
    }

    VisualElement CreateManaGroup(Character character)
    {
        VisualElement manaGroup = new();
        manaGroup.style.flexDirection = FlexDirection.Row;
        manaGroup.style.width = Length.Percent(100);

        Label manaLabel = new Label();
        manaLabel.AddToClassList("manaLabel");

        _manaBar = new();
        _missingMana = new();
        _manaBarInteractionResult = new();
        _manaText = new();

        _manaBar.AddToClassList("resourceBar");
        _manaBar.style.backgroundColor = new Color(0.168f, 0.149f, 0.85f);
        _missingMana.AddToClassList("barMissingAmount");
        _manaBarInteractionResult.AddToClassList("barInteractionResult");
        _manaText.AddToClassList("barText");

        int maxMana = 50 + character.Intelligence * 5;
        _manaText.text = maxMana + "/" + maxMana;

        _manaBar.Add(_missingMana);
        _manaBar.Add(_manaBarInteractionResult);
        _manaBar.Add(_manaText);

        manaGroup.Add(manaLabel);
        manaGroup.Add(_manaBar);

        return manaGroup;
    }

}
