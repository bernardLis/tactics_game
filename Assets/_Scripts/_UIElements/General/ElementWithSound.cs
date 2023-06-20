using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementWithSound : VisualElement
{
    protected AudioManager _audioManager;

    public ElementWithSound()
    {
        _audioManager = AudioManager.Instance;
        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
    }

    protected void PlayClick() { _audioManager.PlayUI("UI Click"); }

}
