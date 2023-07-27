using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureCardFull : EntityCardFull
{

    public Creature Creature;
    public CreatureCardFull(Creature creature) : base(creature)
    {
        Creature = creature;

        _entityIcon.PlayAnimationAlways();

        UpdateBasicStats();
        UpdateBattleCharacteristics();
        AddAbility();
        AddBattleStats();
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


    void AddAbility()
    {
        if (Creature.CreatureAbility != null)
            _topContainer.Add(new CreatureAbilityElement(Creature.CreatureAbility));
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

}
