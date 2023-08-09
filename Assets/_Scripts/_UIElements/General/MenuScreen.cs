using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuScreen : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "menu__";
    const string _ussMain = _ussClassName + "main";
    const string _ussButtonContainer = _ussClassName + "button-container";

    public MenuScreen() : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.MenuStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddContinueButton();
        AddMenuButtons();
    }

    void AddMenuButtons()
    {
        VisualElement container = new();
        container.AddToClassList(_ussButtonContainer);
        _content.Add(container);

        MyButton settingsButton = new("Settings", _ussCommonMenuButton, ShowSettingsScreen);
        MyButton mainMenuButton = new("Main Menu", _ussCommonMenuButton, GoToMainMenu);
        MyButton quitButton = new("Quit", _ussCommonMenuButton, ConfirmQuit);

        container.Add(settingsButton);
        container.Add(mainMenuButton);
        container.Add(quitButton);
    }

    void ShowSettingsScreen() { new SettingsScreen(); }

    void GoToMainMenu()
    {
        _gameManager.LoadScene(Scenes.MainMenu);
        Hide();
    }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new();
        pop.Initialize(GameManager.Instance.Root, Quit);
    }

    void Quit() { Application.Quit(); }
}
