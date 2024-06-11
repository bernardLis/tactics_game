using System.Collections.Generic;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero.Ability
{
    public class AbilityElement : ElementWithTooltip
    {
        private const string _ussCommonTextPrimary = "common__text-primary";
        private const string _ussCommonButtonBasic = "common__button-basic";

        private const string _ussClassName = "ability-element__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussIcon = _ussClassName + "icon";
        private const string _ussBorder = _ussClassName + "border";
        private const string _ussLevelDotEmpty = _ussClassName + "level-dot-empty";
        private const string _ussLevelDotFull = _ussClassName + "level-dot-full";

        private readonly AudioManager _audioManager;

        private readonly VisualElement _icon;

        public readonly Ability Ability;

        private OverlayTimerElement _cooldownTimer;

        private readonly bool _isUpgrade;

        public AbilityElement(Ability ability, bool showLevel = false, int size = 100, bool isUpgrade = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.AbilityElementStyles);
            if (ss != null) styleSheets.Add(ss);

            _audioManager = AudioManager.Instance;

            Ability = ability;
            Ability.OnCooldownStarted += StartCooldown;

            _isUpgrade = isUpgrade;

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussCommonButtonBasic);
            style.width = size;
            style.height = size;

            _icon = new();
            _icon.AddToClassList(_ussIcon);
            _icon.style.backgroundImage = ability.Icon.texture;
            Add(_icon);

            VisualElement border = new();
            border.AddToClassList(_ussBorder);
            Add(border);

            if (showLevel) AddLevelUpDots();
        }

        private void AddLevelUpDots()
        {
            VisualElement dotContainer = new();
            dotContainer.style.flexDirection = FlexDirection.Row;
            dotContainer.style.position = Position.Absolute;
            dotContainer.style.top = Length.Percent(15);
            Add(dotContainer);
            List<VisualElement> dots = new();
            for (int i = 0; i < Ability.Levels.Count; i++)
            {
                VisualElement dot = new();
                dot.AddToClassList(_ussLevelDotEmpty);
                dots.Add(dot);
                dotContainer.Add(dot);
            }

            for (int i = 0; i < Ability.Level + 1; i++)
                dots[i].AddToClassList(_ussLevelDotFull);

            Ability.OnLevelUp += () =>
            {
                for (int i = 0; i < Ability.Level + 1; i++)
                    dots[i].AddToClassList(_ussLevelDotFull);
            };
        }


        private void StartCooldown()
        {
            _icon.style.opacity = 0.5f;
            transform.scale = Vector3.one * 0.9f;

            if (_cooldownTimer != null) RemoveCooldownTimer();

            _cooldownTimer = new(Ability.GetCooldown() - 0.5f, Ability.GetCooldown(), false, "");
            _cooldownTimer.style.width = Length.Percent(90);
            _cooldownTimer.style.height = Length.Percent(90);

            Add(_cooldownTimer);
            _cooldownTimer.OnTimerFinished += OnCooldownFinished;
        }

        private void OnCooldownFinished()
        {
            _audioManager.PlayUI("Ability Available");
            _icon.style.opacity = 1f;
            transform.scale = Vector3.one;

            RemoveCooldownTimer();
        }

        private void RemoveCooldownTimer()
        {
            if (_cooldownTimer != null)
            {
                Remove(_cooldownTimer);
                _cooldownTimer.OnTimerFinished -= OnCooldownFinished;
                _cooldownTimer = null;
            }
        }

        protected override void DisplayTooltip()
        {
            AbilityTooltipElement tt = new(Ability, _isUpgrade);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}