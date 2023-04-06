using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlButton : VisualElement
{
    const string _ussClassName = "control-button__";
    const string _ussBackground = _ussClassName + "background";
    const string _ussIcon = _ussClassName + "icon";
    protected const string _ussSelected = _ussClassName + "selected";

    protected GameManager _gameManager;
    protected CameraSmoothFollow _cameraSmoothFollow;
    protected VisualElement _root;
    protected DraggableArmies _draggableArmies;

    protected VisualElement _icon;

    public ControlButton(VisualElement root, DraggableArmies draggableArmies)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ControlButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _cameraSmoothFollow = Camera.main.GetComponent<CameraSmoothFollow>();
        _root = root;
        _draggableArmies = draggableArmies;

        _gameManager.OnDayPassed += OnDayPassed;

        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

        AddToClassList(_ussBackground);
        _icon = new();
        _icon.AddToClassList(_ussIcon);
        Add(_icon);
    }

    protected virtual void OnDayPassed(int day)
    {
        // meant to be overwritten
    }

    protected virtual void OnPointerDown(PointerDownEvent e)
    {
        // meant to be overwritten
    }

    protected virtual void OnMouseEnter(MouseEnterEvent e)
    {
        // meant to be overwritten
    }

    protected virtual void OnMouseLeave(MouseLeaveEvent e)
    {
        // meant to be overwritten
    }
}
