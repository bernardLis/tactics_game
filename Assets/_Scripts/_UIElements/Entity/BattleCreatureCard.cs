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

        OverrideExpBar();
        OverrideHealthBar();
        HandleCreatureAbility();
    }

    void OverrideExpBar()
    {
        _expBar.UpdateTrackedVariables(_battleCreature.Creature.Experience, _battleCreature.Creature.ExpForNextLevel);

        _battleCreature.Creature.OnLevelUp += () => _levelLabel.text = $"Level {_battleCreature.Creature.Level}";
    }

    void OverrideHealthBar()
    {
        _healthBar.UpdateTrackedVariables(_battleCreature.CurrentHealth, _battleCreature.Creature.MaxHealth);
    }

    void HandleCreatureAbility()
    {
        if (_battleCreature.Creature.CreatureAbility == null) return;

        _abilityElement = new(_battleCreature.Creature.CreatureAbility,
                            _battleCreature.CurrentSpecialAbilityCooldown,
                            !_battleCreature.Creature.IsAbilityUnlocked());
        _battleCreature.Creature.OnLevelUp += () =>
        {
            if (_battleCreature.Creature.IsAbilityUnlocked())
                _abilityElement.Unlock();
        };
        _rightContainer.Add(_abilityElement);
    }
}
