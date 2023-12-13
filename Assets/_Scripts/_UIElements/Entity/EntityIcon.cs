using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;

public class EntityIcon : ElementWithTooltip
{
    const string _ussClassName = "entity-icon__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIconContainer = _ussClassName + "icon-container";
    const string _ussFrame = _ussClassName + "frame";

    GameManager _gameManager;

    Entity _entity;

    VisualElement _iconContainer;
    public VisualElement Frame;

    bool _blockClick;

    AnimationElement _animationElement;
    bool _isAnimationBlocked;
    public EntityIcon(Entity entity, bool blockClick = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _entity = entity;

        AddToClassList(_ussMain);

        _iconContainer = new();
        _iconContainer.AddToClassList(_ussIconContainer);

        _animationElement = new(entity.IconAnimation, 50, true);
        _iconContainer.Add(_animationElement);

        Frame = new();
        Frame.AddToClassList(_ussFrame);

        Add(_iconContainer);
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

    void OnMouseEnter(MouseEnterEvent evt)
    {
        if (_isAnimationBlocked) return;
        _animationElement.PlayAnimation();

        if (!_blockClick)
            DOTween.Kill(transform);
    }

    void OnMouseLeave(MouseLeaveEvent evt)
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
        if (_entity is Creature creature)
            card = new CreatureScreen(creature);
        if (_entity is Turret turret)
            card = new EntityFightScreen(turret);
        if (_entity is Minion minion)
            card = new EntityMovementScreen(minion);
        if (_entity is Boss boss)
            card = new BossScreen(boss);

        card?.Initialize();
    }
}
