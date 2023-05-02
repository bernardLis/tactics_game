using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityIcon : VisualElement
{
    const string _ussClassName = "entity-icon__";
    const string _ussMain = _ussClassName + "main";
    const string _ussIconContainer = _ussClassName + "icon-container";
    const string _ussFrame = _ussClassName + "frame";

    GameManager _gameManager;

    ArmyEntity _entity;

    AnimationElement _animationElement;
    public EntityIcon(ArmyEntity entity)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _entity = entity;

        AddToClassList(_ussMain);

        VisualElement iconContainer = new();
        iconContainer.AddToClassList(_ussIconContainer);

        _animationElement = new(entity.IconAnimation, 50, true);
        iconContainer.Add(_animationElement);

        VisualElement frame = new();
        frame.AddToClassList(_ussFrame);

        Add(iconContainer);
        Add(frame);

        RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
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
