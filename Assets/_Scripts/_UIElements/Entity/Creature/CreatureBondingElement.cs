using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureBondingElement : FullScreenElement
{

    const string _ussClassName = "creature-bonding__";

    const string _ussTitle = _ussClassName + "title";
    const string _ussNameField = _ussClassName + "name-field";
    const string _ussUnlockAbilityButton = _ussClassName + "unlock-ability-button";

    Creature _creature;



    public CreatureBondingElement(Creature creature)
    {
        // TODO: skipping bonding for now... 
        // I should rethink how it is handled - maybe it is extremely annoying to take 
        //the player out of the loop just to click a button
        _creature = creature;
    }

    void AddLevelUpTitle()
    {
        VisualElement container = new();
        container.AddToClassList(_ussTitle);

        Label levelUp = new($"{_creature.EntityName} level {_creature.Level}");
        container.Add(levelUp);

        _content.Insert(0, container);
    }

    void SetUpNameChange()
    {
        AddLevelUpTitle();

        // HERE: bonding dotween & styles
        Label tt = new("Name your creature: ");
        tt.style.fontSize = 24;
        _content.Add(tt);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _content.Add(container);

        TextField nameField = new();
        nameField.AddToClassList(_ussNameField);
        container.Add(nameField);
        nameField.RegisterCallback<InputEvent>(ev =>
        {
            _creature.EntityName = nameField.text;
        });
    }

    void SetUpAbilityUnlock()
    {
        AddLevelUpTitle();

        MyButton unlockButton = new("Unlock Ability", _ussUnlockAbilityButton);

        void Unlock()
        {
            unlockButton.RemoveFromHierarchy();
            _creature.CreatureAbility.Unlock();
            AddContinueButton();
        }

        unlockButton.ChangeCallback(Unlock);
        _content.Add(unlockButton);
    }

    void SetUpEvolution()
    {
        MyButton evolveButton = new("Evolve", callback: Hide);
        _content.Add(evolveButton);
    }

}
