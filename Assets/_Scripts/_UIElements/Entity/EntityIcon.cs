using DG.Tweening;
using Lis.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class EntityIcon : ElementWithTooltip
    {
        const string _ussClassName = "entity-icon__";
        const string _ussMain = _ussClassName + "main";
        const string _ussIconContainer = _ussClassName + "icon-container";
        const string _ussFrame = _ussClassName + "frame";

        readonly GameManager _gameManager;

        protected readonly Entity Entity;

        protected readonly VisualElement IconContainer;
        protected readonly VisualElement Frame;

        readonly bool _blockClick;

        readonly AnimationElement _animationElement;
        bool _isAnimationBlocked;

        public EntityIcon(Entity entity, bool blockClick = false)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.EntityIconStyles);
            if (ss != null) styleSheets.Add(ss);

            Entity = entity;

            AddToClassList(_ussMain);

            IconContainer = new();
            IconContainer.AddToClassList(_ussIconContainer);

            _animationElement = new(entity.IconAnimation, 50, true);
            IconContainer.Add(_animationElement);

            Frame = new();
            Frame.AddToClassList(_ussFrame);

            Add(IconContainer);
            Add(Frame);

            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            _blockClick = blockClick;
            if (!blockClick)
            {
                MoveIcon();
                RegisterCallback<ClickEvent>(OnClick, TrickleDown.NoTrickleDown);
            }
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

            EntityScreen card = null;
            if (Entity is Creature creature)
                card = new CreatureScreen(creature);
            if (Entity is Turret turret)
                card = new EntityFightScreen(turret);
            if (Entity is Minion minion)
                card = new EntityMovementScreen(minion);
            if (Entity is Boss boss)
                card = new BossScreen(boss);

            card?.Initialize();
        }
    }
}