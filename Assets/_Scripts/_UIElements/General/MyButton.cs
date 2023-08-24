using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MyButton : Button
{
    protected GameManager _gameManager;
    AudioManager _audioManager;

    protected Label _text;

    const string _ussCommonButtonBasic = "common__button-basic";

    Action _currentCallback;
    public MyButton(string buttonText = null, string className = null, Action callback = null)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _audioManager = AudioManager.Instance;

        _text = new Label(buttonText);
        _text.style.whiteSpace = WhiteSpace.Normal;
        Add(_text);
        if (buttonText == "")
            _text.style.display = DisplayStyle.None;

        if (className != null)
        {
            AddToClassList(className);
            AddToClassList(_ussCommonButtonBasic);
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
        clickable = new Clickable(() => { });
        clicked += newCallback;
    }

    public void ClearCallbacks() { clickable = new Clickable(() => { }); }

    public void SetText(string newText)
    {
        _text.text = newText;
        _text.style.display = DisplayStyle.Flex;
    }

    void PlayClick(MouseEnterEvent evt)
    {
        if (!enabledSelf)
            return;
        if (_audioManager != null)
            _audioManager.PlayUI("UI Click");
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
