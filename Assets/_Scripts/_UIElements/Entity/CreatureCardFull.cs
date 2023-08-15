using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureCardFull : EntityCardFull
{

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
        Label basePower = new($"Base Power: {Creature.BasePower}");
        _topRightContainer.Add(basePower);
        Label attackRange = new($"Attack Range: {Creature.AttackRange}");
        _topRightContainer.Add(attackRange);
        Label attackCooldown = new($"Attack Cooldown: {Creature.AttackCooldown}");
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

    void SetUpNameChange()
    {
        // HERE: bonding dotween & styles
        _nameContainer.Clear();
        Label tt = new("Name your creature: ");
        tt.style.fontSize = 34;
        _nameContainer.Add(tt);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _nameContainer.Add(container);

        TextField nameField = new();
        nameField.style.fontSize = 34;
        nameField.style.minWidth = 300;
        nameField.style.color = Color.black;
        container.Add(nameField);

        Button submit = new() { text = "Submit" };
        container.Add(submit);
        submit.clicked += () =>
        {
            Creature.EntityName = nameField.text;
            _nameContainer.Clear();
            Label name = new(Creature.EntityName);
            name.style.fontSize = 34;
            _nameContainer.Add(name);
        };
    }

    void SetUpAbilityUnlock()
    {
        MyButton unlockButton = new("Unlock Ability");

        void Unlock()
        {
            unlockButton.RemoveFromHierarchy();
            Creature.CreatureAbility.Unlock();
        }

        unlockButton.ChangeCallback(Unlock);
        _container.Insert(0, unlockButton);
    }

    void SetUpEvolution()
    {
        MyButton unlockButton = new("Evolve", callback: Hide);
        _container.Insert(0, unlockButton);
    }


}
