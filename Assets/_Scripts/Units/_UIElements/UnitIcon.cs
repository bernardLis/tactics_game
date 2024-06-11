using Lis.Battle;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitIcon : ElementWithTooltip
    {
        private const string _ussClassName = "unit-icon__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussIconContainer = _ussClassName + "icon-container";
        private const string _ussFrame = _ussClassName + "frame";

        private readonly AnimationElement _animationElement;

        private readonly Unit _unit;
        protected readonly VisualElement Frame;

        protected readonly VisualElement IconContainer;
        private bool _isAnimationBlocked;

        public UnitIcon(Unit unit, bool blockClick = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitIconStyles);
            if (ss != null) styleSheets.Add(ss);

            _unit = unit;

            AddToClassList(_ussMain);

            IconContainer = new();
            IconContainer.AddToClassList(_ussIconContainer);

            if (unit.IconAnimation.Length > 0)
                _animationElement = new(unit.IconAnimation, 50, true);
            else
                _animationElement = new(new[] { unit.Icon }, 50, true);
            IconContainer.Add(_animationElement);

            Frame = new();
            Frame.AddToClassList(_ussFrame);

            Add(IconContainer);
            Add(Frame);

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            if (blockClick) return;
            RegisterCallback<ClickEvent>(OnClick);
        }

        public void PlayAnimationAlways()
        {
            _animationElement.PlayAnimation();
            UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        protected void OnMouseEnter(MouseEnterEvent evt)
        {
            if (_isAnimationBlocked) return;
            _animationElement.PlayAnimation();
        }

        protected void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (_isAnimationBlocked) return;
            _animationElement.PauseAnimation();
        }

        private void OnClick(ClickEvent evt)
        {
            evt.StopImmediatePropagation();
            UnitScreen screen = UnitScreenFactory.Instance.CreateUnitScreen(_unit);
            screen?.Initialize();
        }
    }
}