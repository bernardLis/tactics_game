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


        string power = "" + Mathf.RoundToInt(_stats.Power);
        string armor = "" + Mathf.RoundToInt(_stats.Armor);
        string attackRange = "" + Mathf.RoundToInt(_stats.AttackRange);
        string attackCooldown = "" + Mathf.RoundToInt(_stats.AttackCooldown);
        string speed = "" + Mathf.RoundToInt(_stats.Speed);

        if (_battleEntity.Stats.Hero != null)
        {
            power += " + " + Mathf.RoundToInt(_battleEntity.Stats.Hero.Power.GetValue());
            armor += " + " + Mathf.RoundToInt(_battleEntity.Stats.Hero.Armor.GetValue());
        }

        leftContainer.Add(new ElementalElement(_stats.Element));
        VisualElement container = new();
        container.style.width = 100;
        container.style.height = 100;

        EntityIcon icon = new(battleEntity.Stats);
        leftContainer.Add(icon);
        leftContainer.Add(new Label($"Name: {_stats.Name}"));
        leftContainer.Add(new Label($"Health: {_battleEntity.CurrentHealth} / {_stats.Health}"));
        leftContainer.Add(new Label($"Killed enemies: {_battleEntity.KilledEnemiesCount}"));

        rightContainer.Add(new Label($"Power: {power}"));
        rightContainer.Add(new Label($"Armor: {armor}"));
        rightContainer.Add(new Label($"Attack Range: {attackRange}"));
        rightContainer.Add(new Label($"Attack Cooldown: {attackCooldown}"));
        rightContainer.Add(new Label($"Speed: {speed}"));
    }


}
