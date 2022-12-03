using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MyButton : Button
{
    AudioManager _audioManager;

    Action _currentCallback;

    public MyButton(string buttonText = null, string className = null, Action callback = null)
    {
        _audioManager = AudioManager.Instance;

        text = buttonText;

        if (className != null)
        {
            AddToClassList(className);
            RemoveFromClassList("unity-button");
        }
        if (callback != null)
        {
            _currentCallback = callback;
            clicked += callback;
        }

        RegisterCallback<MouseEnterEvent>(PlayClick);
        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        this.Blur();
    }

    public void ChangeCallback(Action newCallback)
    {
        clicked -= _currentCallback;
        clicked += newCallback;
    }

    public void ClearCallbacks() { clickable = null; }

    public void UpdateButtonText(string newText) { text = newText; }

    public void UpdateButtonColor(Color color) { style.backgroundColor = color; }

    void PlayClick(MouseEnterEvent evt)
    {
        if (!enabledSelf)
            return;
        if (_audioManager != null)
            _audioManager.PlaySFX("uiClick", Vector3.zero);
    }

    void PreventInteraction(MouseEnterEvent evt)
    {
        evt.PreventDefault();
        evt.StopImmediatePropagation();
    }

    void OnDisable()
    {
        UnregisterCallback<MouseEnterEvent>(PlayClick);
        // https://forum.unity.com/threads/hover-state-control-from-code.914504/
        RegisterCallback<MouseEnterEvent>(PreventInteraction);
    }

    void OnEnable()
    {
        RegisterCallback<MouseEnterEvent>(PlayClick);
        UnregisterCallback<MouseEnterEvent>(PreventInteraction);
    }

}
