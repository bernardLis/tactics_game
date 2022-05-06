using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewTroopsScreen : FullScreenVisual
{
    public event Action OnClose;
    public ViewTroopsScreen(List<Character> troops, VisualElement root)
    {
        Initialize(root);

        foreach (Character character in troops)
        {
            CharacterCardVisual card = new CharacterCardVisual(character);
            Add(card);
        }

        AddBackButton();
    }

    public override void Hide()
    {
        base.Hide();
        OnClose?.Invoke();
    }
}
