using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureIcon : VisualElement
{
    const string _ussClassName = "creature-icon__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIconContainer = _ussClassName + "icon-container";
    const string _ussFrame = _ussClassName + "frame";

    GameManager _gameManager;

    Creature _creature;

    VisualElement _iconContainer;
    public VisualElement Frame;

    AnimationElement _animationElement;
    public CreatureIcon(Creature creature)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _creature = creature;

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

    public void SwapCreature(Creature newCreature)
    {
        _creature = newCreature;
        _animationElement.SwapAnimationSprites(newCreature.IconAnimation);
    }

    public void PlayAnimationAlways()
    {
        _animationElement.PlayAnimation();
        UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
        UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
    }

    void OnMouseEnter(MouseEnterEvent evt)
    {
        _animationElement.PlayAnimation();
    }

    void OnMouseLeave(MouseLeaveEvent evt)
    {
        _animationElement.PauseAnimation();
    }
}
