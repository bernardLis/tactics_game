using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmPopUp : VisualElement
{
    GameManager _gameManager;
    VisualElement _root;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "confirm-popup__";
    const string _ussMain = _ussClassName + "main";
    const string _ussText = _ussClassName + "text";
    const string _ussButtonContainer = _ussClassName + "button-container";

    public void Initialize(VisualElement root, Action callback, string displayText = null)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ConfirmPopupStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _root = root;
        root.Add(this);
        AddToClassList(_ussMain);

        if (displayText == null)
            displayText = "Are you sure?";

        Label confirm = new Label(displayText);
        confirm.AddToClassList(_ussText);
        confirm.AddToClassList(_ussCommonTextPrimary);
        Add(confirm);
        AddButtons(callback);
    }

    void AddButtons(Action callback)
    {
        VisualElement container = new VisualElement();
        container.AddToClassList(_ussButtonContainer);
        Add(container);

        MyButton confirm = new MyButton("Yaasss Queen", _ussCommonMenuButton, callback);
        container.Add(confirm);
        confirm.clickable.clicked += Hide;

        VisualElement spacer = new VisualElement();
        spacer.style.width = 50;
        container.Add(spacer);

        MyButton cancel = new MyButton("Cancel!@!", _ussCommonMenuButton, Hide);
        container.Add(cancel);
    }

    void Hide()
    {
        this.SetEnabled(false);
        _root.Remove(this);
    }
}

