using Lis.Units.Hero.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class AbilityStatsElement : VisualElement
    {
        public AbilityStatsElement(Ability ability)
        {
            style.flexDirection = FlexDirection.Row;

            AbilityElement abilityElement = new(ability, true);
            Add(abilityElement);

            VisualElement container = new();
            Add(container);

            Label totalDamage = new($"Total damage: {ability.DamageDealt}");
            container.Add(totalDamage);

            Label dpsSinceActive = new($"DPS: {Mathf.RoundToInt(ability.GetDpsSinceActive())}");
            container.Add(dpsSinceActive);
        }
    }
}