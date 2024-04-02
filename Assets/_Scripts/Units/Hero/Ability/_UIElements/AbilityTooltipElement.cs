using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Ability
{
    public class AbilityTooltipElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";
        const string _ussCommonTextSecondary = "common__text-secondary";

        const string _ussClassName = "ability-tooltip-element__";
        const string _ussMain = _ussClassName + "main";

        public AbilityTooltipElement(Ability ability)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.AbilityTooltipElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            AddToClassList(_ussMain);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;

            Label n = new(Helpers.ParseScriptableObjectName(ability.name));
            n.AddToClassList(_ussCommonTextPrimary);
            n.style.alignSelf = Align.Center;
            container.Add(n);
            container.Add(new NatureElement(ability.Nature));

            Label description = new(ability.Description);
            description.AddToClassList(_ussCommonTextSecondary);
            description.style.whiteSpace = WhiteSpace.Normal;

            Label level = new($"Level: {ability.Level + 1}");
            level.AddToClassList(_ussCommonTextSecondary);

            Label power = new("Power: " + ability.GetPower());
            power.AddToClassList(_ussCommonTextSecondary);

            Label cooldown = new("Cooldown: " + ability.GetCooldown());
            cooldown.AddToClassList(_ussCommonTextSecondary);

            Label scale = new("Scale: " + ability.GetScale());
            scale.AddToClassList(_ussCommonTextSecondary);

            Add(container);
            Add(description);
            Add(level);
            Add(power);
            Add(cooldown);
            Add(scale);

            if (!ability.IsArmorPiercing) return;

            Label armorPiercing = new("Armor Piercing");
            armorPiercing.AddToClassList(_ussCommonTextSecondary);
            Add(armorPiercing);
        }
    }
}