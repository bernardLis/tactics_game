using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlsButton : VisualElement
{
    protected GameManager _gameManager;
    protected CameraSmoothFollow _cameraSmoothFollow;
    protected VisualElement _root;
    protected DraggableArmies _draggableArmies;

    public ControlsButton(VisualElement root, DraggableArmies draggableArmies)
    {
        _gameManager = GameManager.Instance;
        _cameraSmoothFollow = Camera.main.GetComponent<CameraSmoothFollow>();
        _root = root;
        _draggableArmies = draggableArmies;

        _gameManager.OnDayPassed += OnDayPassed;

        style.width = 100;
        style.height = 100;

        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
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
