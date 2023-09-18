using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleCreatureCard : BattleEntityCard
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "battle-creature__";

    BattleCreature _battleCreature;

    CreatureAbilityElement _abilityElement;

    public BattleCreatureCard(BattleCreature battleCreature) : base(battleCreature)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleCreatureCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleCreature = battleCreature;
        HandleCreatureAbility();
    }

    void HandleCreatureAbility()
    {
        if (_battleCreature.Creature.CreatureAbility == null) return;

        _abilityElement = new(_battleCreature.Creature.CreatureAbility,
                            _battleCreature.CurrentAbilityCooldown,
                            !_battleCreature.Creature.IsAbilityUnlocked());
        _rightContainer.Add(_abilityElement);
    }
}
