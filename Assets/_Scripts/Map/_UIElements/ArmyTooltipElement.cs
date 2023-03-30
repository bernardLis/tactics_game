using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ArmyTooltipElement : VisualElement
{
    ArmyGroup _armyGroup;

    public ArmyTooltipElement(ArmyGroup armyGroup)
    {
        _armyGroup = armyGroup;
        Label sprite = new();
        sprite.style.width = 100;
        sprite.style.height = 100;
        sprite.style.backgroundImage = new StyleBackground(_armyGroup.ArmyEntity.Icon);
        Add(sprite);

        Label name = new(_armyGroup.ArmyEntity.Name);
        Add(name);

        Label health = new("Health: " + _armyGroup.ArmyEntity.Health);
        Add(health);

        Label power = new("Power: " + _armyGroup.ArmyEntity.Power);
        Add(power);

        Label attackRange = new("Attack Range: " + _armyGroup.ArmyEntity.AttackRange);
        Add(attackRange);

        Label attackCooldown = new("Attack Cooldown: " + _armyGroup.ArmyEntity.AttackCooldown);
        Add(attackCooldown);

        Label speed = new("Speed: " + _armyGroup.ArmyEntity.Speed);
        Add(speed);

        Label isRanged = new("Is Ranged: " + (_armyGroup.ArmyEntity.Projectile != null));
        Add(isRanged);
    }
}
