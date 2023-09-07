using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureCardFull : EntityFightCardFull
{

    public Creature Creature;

    public CreatureCardFull(Creature creature)
            : base(creature)
    {
        Creature = creature;
    }

    public override void Initialize()
    {
        base.Initialize();
        _entityIcon.PlayAnimationAlways();

        AddAbility();
    }

    protected override void AddOtherBasicInfo()
    {
        base.AddOtherBasicInfo();
        Label upgradeTier = new($"Tier: {Creature.UpgradeTier}");
        _otherContainer.Add(upgradeTier);
    }

    void AddAbility()
    {
        if (Creature.CreatureAbility == null) return;
        _otherContainer.Insert(0, new CreatureAbilityElement(Creature.CreatureAbility, isLocked: !Creature.IsAbilityUnlocked()));
    }
}
