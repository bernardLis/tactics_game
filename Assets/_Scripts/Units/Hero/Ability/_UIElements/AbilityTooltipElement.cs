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

        Ability _ability;

        Label _power;
        Label _cooldown;
        Label _scale;
        Label _amount;
        Label _duration;

        public AbilityTooltipElement(Ability ability, bool isUpgrade = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.AbilityTooltipElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _ability = ability;
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

            _power = new("Power: " + _ability.GetPower());
            _power.AddToClassList(_ussCommonTextSecondary);

            _cooldown = new("Cooldown: " + _ability.GetCooldown());
            _cooldown.AddToClassList(_ussCommonTextSecondary);

            _scale = new("Scale: " + _ability.GetScale());
            _scale.AddToClassList(_ussCommonTextSecondary);

            _amount = new("Amount: " + _ability.GetAmount());
            _amount.AddToClassList(_ussCommonTextSecondary);

            _duration = new("Duration: " + _ability.GetDuration());
            _duration.AddToClassList(_ussCommonTextSecondary);

            Add(container);
            Add(description);
            Add(level);
            Add(_power);
            Add(_cooldown);
            Add(_scale);
            Add(_amount);
            Add(_duration);

            if (isUpgrade) HandleUpgrade();
            if (!ability.IsArmorPiercing) return;

            Label armorPiercing = new("Armor Piercing");
            armorPiercing.AddToClassList(_ussCommonTextSecondary);
            Add(armorPiercing);
        }

        void HandleUpgrade()
        {
            if (_ability.Level == _ability.Levels.Count - 1) return;
            HandlePower();
            HandleCooldown();
            HandleScale();
            HandleAmount();
            HandleDuration();
        }

        void HandlePower()
        {
            if (_ability.GetPower(_ability.Level) == _ability.GetPower(_ability.Level - 1)) return;
            _power.text = $"Power: {_ability.GetPower(_ability.Level - 1)} -> {_ability.GetPower()}";
        }

        void HandleCooldown()
        {
            if (_ability.GetCooldown(_ability.Level) == _ability.GetCooldown(_ability.Level - 1)) return;
            _cooldown.text = $"Cooldown: {_ability.GetCooldown(_ability.Level - 1)} -> {_ability.GetCooldown()}";
        }

        void HandleScale()
        {
            if (_ability.GetScale(_ability.Level) == _ability.GetScale(_ability.Level - 1)) return;
            _scale.text = $"Scale: {_ability.GetScale(_ability.Level - 1)} -> {_ability.GetScale()}";
        }

        void HandleAmount()
        {
            if (_ability.GetAmount(_ability.Level) == _ability.GetAmount(_ability.Level - 1)) return;
            _amount.text = $"Amount: {_ability.GetAmount(_ability.Level - 1)} -> {_ability.GetAmount()}";
        }

        void HandleDuration()
        {
            if (_ability.GetDuration(_ability.Level) == _ability.GetDuration(_ability.Level - 1)) return;
            _duration.text = $"Duration: {_ability.GetDuration(_ability.Level - 1)} -> {_ability.GetDuration()}";
        }
    }
}