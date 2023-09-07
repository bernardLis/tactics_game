using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureCardFull : EntityCardFull
{
    const string _ussTitle = _ussClassName + "title";
    const string _ussNameField = _ussClassName + "name-field";
    const string _ussUnlockAbilityButton = _ussClassName + "unlock-ability-button";

    public Creature Creature;
    public CreatureCardFull(Creature creature, bool isChangingName = false,
            bool isUnlockingAbility = false, bool isEvolving = false)
            : base(creature)
    {
        Creature = creature;

        _entityIcon.PlayAnimationAlways();

        UpdateBasicStats();
        UpdateBattleCharacteristics();
        AddAbility(isUnlockingAbility);
        AddBattleStats();

        if (isChangingName) SetUpNameChange();
        if (isUnlockingAbility) SetUpAbilityUnlock();
        if (isEvolving) SetUpEvolution();
    }


    void UpdateBasicStats()
    {
        Label upgradeTier = new($"Tier: {Creature.UpgradeTier}");
        _topMiddleContainer.Add(upgradeTier);
    }

    void UpdateBattleCharacteristics()
    {
        StatElement power = new(Creature.Power);
        _topRightContainer.Add(power);
        StatElement attackRange = new(Creature.AttackRange);
        _topRightContainer.Add(attackRange);
        StatElement attackCooldown = new(Creature.AttackCooldown);
        _topRightContainer.Add(attackCooldown);
    }

    void AddAbility(bool isUnlockingAbility)
    {
        if (Creature.CreatureAbility == null) return;

        bool isLocked = !Creature.IsAbilityUnlocked();
        if (isUnlockingAbility) isLocked = true; // force it to "play effect" (that does not exist atm)
        _topContainer.Add(new CreatureAbilityElement(Creature.CreatureAbility, isLocked: isLocked));
    }

    void AddBattleStats()
    {
        VisualElement container = new();
        _topContainer.Add(container);

        Label killCount = new($"Kill Count: {Creature.TotalKillCount}");
        Label damageDealt = new($"Damage Dealt: {Creature.TotalDamageDealt}");
        Label damageTaken = new($"Damage Taken: {Creature.TotalDamageTaken}");

        container.Add(killCount);
        container.Add(damageDealt);
        container.Add(damageTaken);
    }

    void AddLevelUpTitle()
    {
        VisualElement container = new();
        container.AddToClassList(_ussTitle);

        Label levelUp = new($"{Creature.EntityName} level {Creature.Level}");
        container.Add(levelUp);

        _mainCardContainer.Insert(0, container);
    }

    void SetUpNameChange()
    {
        AddLevelUpTitle();
        // HERE: bonding dotween & styles
        _nameContainer.Clear();
        Label tt = new("Name your creature: ");
        tt.style.fontSize = 24;
        _nameContainer.Add(tt);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _nameContainer.Add(container);

        TextField nameField = new();
        nameField.AddToClassList(_ussNameField);
        container.Add(nameField);
        nameField.RegisterCallback<InputEvent>(ev =>
        {
            Creature.EntityName = nameField.text;
        });
    }

    void SetUpAbilityUnlock()
    {
        AddLevelUpTitle();

        _content.Remove(_continueButton);

        MyButton unlockButton = new("Unlock Ability", _ussUnlockAbilityButton);

        void Unlock()
        {
            unlockButton.RemoveFromHierarchy();
            Creature.CreatureAbility.Unlock();
            AddContinueButton();
        }

        unlockButton.ChangeCallback(Unlock);
        _mainCardContainer.Add(unlockButton);

        //   _mainCardContainer.Insert(0, unlockButton);
    }

    void SetUpEvolution()
    {
        MyButton unlockButton = new("Evolve", callback: Hide);
        _mainCardContainer.Insert(0, unlockButton);
    }
}
