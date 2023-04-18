using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityElement : VisualElement
{
    const string _ussClassName = "battle-entity__";
    const string _ussMain = _ussClassName + "main";

    BattleEntity _battleEntity;
    ArmyEntity _stats;

    public BattleEntityElement(BattleEntity battleEntity)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleEntityElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _battleEntity = battleEntity;
        _stats = _battleEntity.Stats;

        AddToClassList(_ussMain);

        VisualElement leftContainer = new();
        VisualElement rightContainer = new();
        Add(leftContainer);
        Add(rightContainer);

        leftContainer.Add(new ElementalElement(_stats.Element));
        leftContainer.Add(new Label($"Name: {_stats.Name}"));
        leftContainer.Add(new Label($"Health: {_battleEntity.CurrentHealth} / {_stats.Health}"));
        leftContainer.Add(new Label($"Killed enemies: {_battleEntity.KilledEnemiesCount}"));

        rightContainer.Add(new Label($"Power: {_stats.Power}"));
        rightContainer.Add(new Label($"Armor: {_stats.Armor}"));
        rightContainer.Add(new Label($"Attack Range: {_stats.AttackRange}"));
        rightContainer.Add(new Label($"Attack Cooldown: {_stats.AttackCooldown}"));
        rightContainer.Add(new Label($"Speed: {_stats.Speed}"));
    }


}
