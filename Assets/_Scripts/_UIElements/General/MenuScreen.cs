using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuScreen : FullScreenElement
{
    GameManager _gameManager;
    public event Action OnClose;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "menu__";
    const string _ussMain = _ussClassName + "main";

    public MenuScreen(VisualElement root)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.MenuStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Initialize(root);
        AddToClassList(_ussMain);
        AddButtons();

        var tempColor = style.backgroundColor.value;
        tempColor.a = 0.5f;
        style.backgroundColor = tempColor;
    }

    void AddButtons()
    {
        VisualElement container = new();
        container.style.width = Length.Percent(100);
        container.style.height = Length.Percent(70);
        container.style.alignItems = Align.Center;
        container.style.justifyContent = Justify.Center;

        MyButton continueButton = new("Continue", _ussCommonMenuButton, Hide);
        MyButton settingsButton = new("Settings", _ussCommonMenuButton, ShowSettingsScreen);
        MyButton mainMenuButton = new("Main Menu", _ussCommonMenuButton, GoToMainMenu);
        MyButton quitButton = new("Quit", _ussCommonMenuButton, ConfirmQuit);

        container.Add(continueButton);
        container.Add(settingsButton);
        container.Add(mainMenuButton);
        container.Add(quitButton);

        Add(container);
    }

    void ShowSettingsScreen() { new SettingsScreen(_root, this); }

    void GoToMainMenu()
    {
        GameManager.Instance.LoadScene(Scenes.MainMenu);
        Hide();
    }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new ConfirmPopUp();
        pop.Initialize(_root, Quit);
    }

    void Quit() { Application.Quit(); }

    public override void Hide()
    {
        base.Hide();
        OnClose?.Invoke();
    }
}
