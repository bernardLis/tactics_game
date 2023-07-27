using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class EntityIcon : ElementWithTooltip
{
    const string _ussClassName = "entity-icon__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIconContainer = _ussClassName + "icon-container";
    const string _ussFrame = _ussClassName + "frame";

    GameManager _gameManager;

    Entity _entity;
    bool _blockTooltip;

    VisualElement _iconContainer;
    public VisualElement Frame;

    AnimationElement _animationElement;
    bool _isAnimationBlocked;
    public EntityIcon(Entity creature, bool blockTooltip = false, bool blockClick = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _entity = creature;
        _blockTooltip = blockTooltip;

        AddToClassList(_ussMain);

        _iconContainer = new();
        _iconContainer.AddToClassList(_ussIconContainer);

        _animationElement = new(creature.IconAnimation, 50, true);
        _iconContainer.Add(_animationElement);

        Frame = new();
        Frame.AddToClassList(_ussFrame);

        Add(_iconContainer);
        Add(Frame);

        RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        if (!blockClick) RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    public void SmallIcon()
    {
        style.width = 50;
        style.height = 50;

        Frame.style.width = 50;
        Frame.style.height = 50;

        _iconContainer.style.width = 40;
        _iconContainer.style.height = 40;
    }

    public void LargeIcon()
    {
        style.width = 200;
        style.height = 200;

        Frame.style.width = 200;
        Frame.style.height = 200;

        _iconContainer.style.width = 180;
        _iconContainer.style.height = 180;
    }

    public void SetEntity(Entity entity)
    {
        _entity = entity;
        _animationElement.SwapAnimationSprites(entity.IconAnimation);
    }

    public void SwapCreature(Entity newEntity)
    {
        _entity = newEntity;

        DOTween.Shake(() => transform.position, x => transform.position = x,
                2f, 10f);

        Color _initialColor = Frame.style.backgroundColor.value;
        Color _targetColor = Color.white;
        DOTween.To(() => Frame.style.backgroundColor.value,
                x => Frame.style.backgroundColor = x, _targetColor, 1f)
            .OnComplete(() => _animationElement.SwapAnimationSprites(newEntity.IconAnimation));

        DOTween.To(() => Frame.style.backgroundColor.value,
                x => Frame.style.backgroundColor = x, _initialColor, 2f)
            .SetDelay(1f);
    }

    public void BlockAnimation() { _isAnimationBlocked = true; }

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
    }

    void OnMouseLeave(MouseLeaveEvent evt)
    {
        if (_isAnimationBlocked) return;
        _animationElement.PauseAnimation();
    }

    void OnMouseUp(MouseUpEvent evt)
    {
        VisualElement root = _gameManager.Root;

        //  if (_entity is Creature)
        //      tooltip = new CreatureCard((Creature)_entity);
        if (_entity is Minion)
        {
            new EntityCardFull((Minion)_entity);
        }
    }

    protected override void DisplayTooltip()
    {
        if (_blockTooltip) return;

        VisualElement tooltip = new();

        if (_entity is Creature)
            tooltip = new CreatureCard((Creature)_entity);
        if (_entity is Minion)
            tooltip = new EntityCard((Minion)_entity);

        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }
}
