using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units.Creature.Ability
{
    public class Element : ElementWithTooltip
    {
        const string _ussClassName = "creature-ability__";
        const string _ussMain = _ussClassName + "main";
        const string _ussTimerLabelWrapper = _ussClassName + "timer-label-wrapper";

        readonly Ability _ability;

        readonly bool _isLocked;

        LockOverlayElement _lockOverlay;
        OverlayTimerElement _timer;

        public Element(Ability ability, float currentCooldown = 0, bool isLocked = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CreatureAbilityStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _isLocked = isLocked;
            _ability = ability;

            AddToClassList(_ussMain);

            style.backgroundImage = _ability.Icon.texture;

            _ability.OnAbilityUsed += OnAbilityUsed;
            RegisterCallback<DetachFromPanelEvent>(_ => _ability.OnAbilityUsed -= OnAbilityUsed);

            AddTimer();
            AddLockedOverlay();

            if (currentCooldown > 0)
            {
                _timer.UpdateTimerValues(currentCooldown, _ability.Cooldown);
                _timer.style.display = DisplayStyle.Flex;
            }
        }

        void AddTimer()
        {
            _timer = new(0, 0, false, "");
            _timer.style.display = DisplayStyle.None;
            _timer.OnTimerFinished += () => _timer.style.display = DisplayStyle.None;
            _timer.SetStyles(null, null, _ussTimerLabelWrapper);
            Add(_timer);
        }

        void AddLockedOverlay()
        {
            if (!_isLocked) return;
            Label label = new($"Unlocked at level {_ability.UnlockLevel}.");
            label.style.whiteSpace = WhiteSpace.Normal;

            _lockOverlay = new(label);
            Add(_lockOverlay);

            _ability.OnAbilityUnlocked += Unlock;
        }

        void Unlock()
        {
            // TODO: this is called twice, idk why...
            //   Debug.Log($"is unlock in ability element called twice?");
            // if (!_isLocked) return;
            // _isLocked = false;
            _ability.OnAbilityUnlocked -= Unlock;

            _lockOverlay?.Unlock();
        }

        void OnAbilityUsed()
        {
            _timer.UpdateTimerValues(_ability.Cooldown, _ability.Cooldown);
            _timer.style.display = DisplayStyle.Flex;
            _timer.Resume();
        }

        protected override void DisplayTooltip()
        {
            TooltipElement tt = new(_ability);
            _tooltip = new(this, tt);
            base.DisplayTooltip();
        }
    }
}