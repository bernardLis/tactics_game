using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MyButton : Button
{
    AudioManager _audioManager;
    public MyButton(string buttonText = null, string className = null, Action callback = null)
    {
        _audioManager = AudioManager.Instance;

        text = buttonText;

        if (className != null)
            AddToClassList(className);
        if (callback != null)
            clicked += callback;

        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        this.Blur();
    }



    void PlayClick()
    {
        if (!enabledSelf)
            return;
        if (_audioManager != null)
            _audioManager.PlaySFX("uiClick", Vector3.zero);
    }

}
