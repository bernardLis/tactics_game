using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis.Units.Attack
{
    public class AttackTooltipElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";
        const string _ussCommonTextSecondary = "common__text-secondary";

        const string _ussClassName = "attack-element__";
        const string _ussMain = _ussClassName + "tooltip-main";

        public AttackTooltipElement(Attack attack)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.AttackElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);

            Label nameLabel = new(Helpers.ParseScriptableObjectName(attack.name));
            nameLabel.AddToClassList(_ussCommonTextPrimary);
            nameLabel.style.alignSelf = Align.Center;

            Label description = new(attack.Description);
            description.AddToClassList(_ussCommonTextSecondary);
            description.style.whiteSpace = WhiteSpace.Normal;

            Label damage = new("Damage: " + attack.GetDamage());
            damage.AddToClassList(_ussCommonTextSecondary);

            Label range = new("Range: " + attack.Range);
            range.AddToClassList(_ussCommonTextSecondary);

            Label globalCooldown = new("Global Cooldown: " + attack.GlobalCooldown);
            globalCooldown.AddToClassList(_ussCommonTextSecondary);

            Label cooldown = new("Cooldown: " + attack.Cooldown);
            cooldown.AddToClassList(_ussCommonTextSecondary);

            Label damageDealt = new("Damage Dealt: " + attack.DamageDealt);
            damageDealt.AddToClassList(_ussCommonTextSecondary);

            Add(nameLabel);
            Add(new NatureElement(attack.Nature));
            Add(description);
            Add(damage);
            Add(range);
            Add(globalCooldown);
            Add(cooldown);
            Add(damageDealt);

            if (!attack.IsArmorPiercing) return;

            Label armorPiercing = new("Armor Piercing");
            armorPiercing.AddToClassList(_ussCommonTextSecondary);
            Add(armorPiercing);
        }
    }
}