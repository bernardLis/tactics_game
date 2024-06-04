using Lis.Battle;
using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Creature;
using Lis.Units.Pawn;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitIcon : ElementWithTooltip
    {
        const string _ussClassName = "unit-icon__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIconContainer = _ussClassName + "icon-container";
        const string _ussFrame = _ussClassName + "frame";

        readonly Unit _unit;

        protected readonly VisualElement IconContainer;
        protected readonly VisualElement Frame;

        readonly AnimationElement _animationElement;
        bool _isAnimationBlocked;

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

        void OnClick(ClickEvent evt)
        {
            evt.StopImmediatePropagation();
            UnitScreen screen = UnitScreenFactory.Instance.CreateUnitScreen(_unit);
            screen?.Initialize();
        }
    }
}