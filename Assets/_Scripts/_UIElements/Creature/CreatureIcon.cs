using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CreatureIcon : ElementWithTooltip
{
    const string _ussClassName = "creature-icon__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIconContainer = _ussClassName + "icon-container";
    const string _ussFrame = _ussClassName + "frame";
    const string _ussLevel = _ussClassName + "level";

    GameManager _gameManager;

    Creature _creature;
    bool _blockTooltip;

    VisualElement _iconContainer;
    public VisualElement Frame;

    Label _level;

    AnimationElement _animationElement;
    bool _isAnimationBlocked;
    public CreatureIcon(Creature creature, bool blockTooltip = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _creature = creature;
        _blockTooltip = blockTooltip;

        AddToClassList(_ussMain);

        _iconContainer = new();
        _iconContainer.AddToClassList(_ussIconContainer);

        _animationElement = new(creature.IconAnimation, 50, true);
        _iconContainer.Add(_animationElement);

        Frame = new();
        Frame.AddToClassList(_ussFrame);

        _level = new($"{_creature.Level}");
        _level.AddToClassList(_ussLevel);
        _creature.OnLevelUp += () => _level.text = $"{_creature.Level}";

        Add(_iconContainer);
        Add(Frame);
        Add(_level);

        RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
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

    public void SetCreature(Creature newCreature)
    {
        _creature = newCreature;
        _animationElement.SwapAnimationSprites(newCreature.IconAnimation);
    }

    public void SwapCreature(Creature newCreature)
    {
        _creature = newCreature;

        DOTween.Shake(() => transform.position, x => transform.position = x,
                2f, 10f);

        Color _initialColor = Frame.style.backgroundColor.value;
        Color _targetColor = Color.white;
        DOTween.To(() => Frame.style.backgroundColor.value,
                x => Frame.style.backgroundColor = x, _targetColor, 1f)
            .OnComplete(() => _animationElement.SwapAnimationSprites(newCreature.IconAnimation));

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

    protected override void DisplayTooltip()
    {
        if (_blockTooltip) return;

        CreatureCard tooltip = new(_creature);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }
}
