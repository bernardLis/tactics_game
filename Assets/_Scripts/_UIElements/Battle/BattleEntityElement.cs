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

        Add(new ElementalElement(_stats.Element));
        Add(new Label($"Name: {_stats.Name}"));
        Add(new Label($"Health: {_battleEntity.CurrentHealth} / {_stats.Health}"));
        Add(new Label($"Killed enemies: {_battleEntity.KilledEnemiesCount}"));

        Add(new Label($"Power: {_stats.Power}"));
        Add(new Label($"Armor: {_stats.Armor}"));
        Add(new Label($"Attack Range: {_stats.AttackRange}"));
        Add(new Label($"Attack Cooldown: {_stats.AttackCooldown}"));
        Add(new Label($"Speed: {_stats.Speed}"));
    }


}
