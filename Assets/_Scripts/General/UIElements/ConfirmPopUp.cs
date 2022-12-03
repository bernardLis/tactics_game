using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmPopUp : VisualElement
{
    VisualElement _root;
    public void Initialize(VisualElement root, Action callback, string displayText = null)
    {
        _root = root;
        root.Add(this);

        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        // https://docs.unity3d.com/Packages/com.unity.ui.builder@1.0/manual/uib-styling-ui-positioning.html
        // center
        style.left = 0;
        style.top = 0;
        style.right = 0;
        style.bottom = 0;
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;

        style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        if (displayText == null)
            displayText = "Are you sure?";

        Label confirm = new Label(displayText);
        confirm.AddToClassList("textPrimary");
        Add(confirm);
        AddButtons(callback);
    }

    void AddButtons(Action callback)
    {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.width = Length.Percent(100);
        container.style.justifyContent = Justify.Center;
        Add(container);

        MyButton confirm = new MyButton("Yaasss Queen", "menuButton", callback);
        container.Add(confirm);
        confirm.clickable.clicked += Hide;

        VisualElement spacer = new VisualElement();
        spacer.style.width = 50;
        container.Add(spacer);

        MyButton cancel = new MyButton("Cancel!@!", "menuButton", Hide);
        container.Add(cancel);
    }

    void Hide()
    {
        this.SetEnabled(false);
        _root.Remove(this);
    }
}

