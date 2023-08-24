using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuScreen : FullScreenElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "menu__";
    const string _ussContainer = _ussClassName + "container";
    const string _ussButtonContainer = _ussClassName + "button-container";

    VisualElement _container;
    public MenuScreen() : base()
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.MenuStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _container = new();
        _container.AddToClassList(_ussContainer);
        _content.Add(_container);

        AddContinueButton();
        AddTotalGoldCollected();
        AddMenuButtons();
    }

    void AddTotalGoldCollected()
    {
        VisualElement goldContainer = new();
        goldContainer.style.flexDirection = FlexDirection.Row;
        goldContainer.AddToClassList(_ussCommonTextPrimary);
        _container.Add(goldContainer);

        Label goldLabel = new()
        {
            text = $"Total Gold Collected: "
        };
        goldContainer.Add(goldLabel);

        GoldElement el = new(_gameManager.TotalGoldCollected);
        goldContainer.Add(el);
    }

    void AddMenuButtons()
    {
        VisualElement buttonContainer = new();
        buttonContainer.AddToClassList(_ussButtonContainer);
        _container.Add(buttonContainer);

        MyButton settingsButton = new("Settings", _ussCommonMenuButton, ShowSettingsScreen);
        MyButton mainMenuButton = new("Main Menu", _ussCommonMenuButton, GoToMainMenu);
        MyButton quitButton = new("Quit", _ussCommonMenuButton, ConfirmQuit);

        buttonContainer.Add(settingsButton);
        buttonContainer.Add(mainMenuButton);
        buttonContainer.Add(quitButton);
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
