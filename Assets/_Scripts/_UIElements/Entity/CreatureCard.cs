using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureCard : EntityCard
{
    const string _ussClassName = "creature-card__";

    protected Creature _creature;

    public CreatureCard(Creature creature) : base(creature)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _creature = creature;

        PopulateCard();

        if (_creature.CreatureAbility != null)
            _rightContainer.Add(new CreatureAbilityElement(_creature.CreatureAbility));
    }
}
