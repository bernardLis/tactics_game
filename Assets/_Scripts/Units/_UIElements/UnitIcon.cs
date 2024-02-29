using DG.Tweening;
using Lis.Core;
using Lis.Units.Boss;
using Lis.Units.Creature;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units
{
    public class UnitIcon : ElementWithTooltip
    {
        const string _ussClassName = "unit-icon__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIconContainer = _ussClassName + "icon-container";
        const string _ussFrame = _ussClassName + "frame";

        protected readonly Unit Unit;

        protected readonly VisualElement IconContainer;
        protected readonly VisualElement Frame;

        readonly bool _blockClick;

        readonly AnimationElement _animationElement;
        bool _isAnimationBlocked;

        public UnitIcon(Unit unit, bool blockClick = false)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnitIconStyles);
            if (ss != null) styleSheets.Add(ss);

            Unit = unit;

            AddToClassList(_ussMain);

            IconContainer = new();
            IconContainer.AddToClassList(_ussIconContainer);

            _animationElement = new(unit.IconAnimation, 50, true);
            IconContainer.Add(_animationElement);

            Frame = new();
            Frame.AddToClassList(_ussFrame);

            Add(IconContainer);
            Add(Frame);

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            _blockClick = blockClick;
            if (blockClick) return;
            MoveIcon();
            RegisterCallback<ClickEvent>(OnClick);
        }

        void MoveIcon()
        {
            DOTween.To(x => transform.scale = x * Vector3.one, 1f, 1.1f, 1.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetTarget(transform);

            DOTween.To(x => style.rotate = new Rotate(x), -5f, 5f, 1.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetTarget(transform);
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

            if (!_blockClick)
                DOTween.Kill(transform);
        }

        protected void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (_isAnimationBlocked) return;
            _animationElement.PauseAnimation();

            if (!_blockClick)
                MoveIcon();
        }

        void OnClick(ClickEvent evt)
        {
            evt.StopImmediatePropagation();

            UnitScreen card = null;
            if (Unit is Creature.Creature creature)
                card = new CreatureScreen(creature);
            if (Unit is Minion.Minion minion)
                card = new UnitMovementScreen(minion);
            if (Unit is Boss.Boss boss)
                card = new BossScreen(boss);

            card?.Initialize();
        }
    }
}