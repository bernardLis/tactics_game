using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementWithSound : VisualElement
{
    AudioManager _audioManager;

    public ElementWithSound()
    {
        _audioManager = AudioManager.Instance;
        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
    }

    protected void PlayClick() { _audioManager.PlaySFX("uiClick", Vector3.zero); }

}
