using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuScreen : FullScreenVisual
{
    public event Action OnClose;

    public MenuScreen(VisualElement root)
    {
        Initialize(root);
        AddToClassList("menuScreen");
        AddButtons();

        var tempColor = style.backgroundColor.value;
        tempColor.a = 0.5f;
        style.backgroundColor = tempColor;

        BattleManager.Instance.PauseGame();
    }

    void AddButtons()
    {
        Button continueButton = new Button();
        Button settingsButton = new Button();
        Button mainMenuButton = new Button();
        Button quitButton = new Button();

        continueButton.text = "Continue";
        settingsButton.text = "Settings";
        mainMenuButton.text = "Main Menu";
        quitButton.text = "Quit";

        continueButton.AddToClassList("menuButton");
        settingsButton.AddToClassList("menuButton");
        mainMenuButton.AddToClassList("menuButton");
        quitButton.AddToClassList("menuButton");

        continueButton.clickable.clicked += Hide;
        settingsButton.clickable.clicked += ShowSettingsScreen;
        mainMenuButton.clickable.clicked += GoToMainMenu;
        quitButton.clickable.clicked += ConfirmQuit;

        Add(continueButton);
        Add(settingsButton);
        Add(mainMenuButton);
        Add(quitButton);
    }

    void ShowSettingsScreen()
    {
        new SettingsScreen(_root, this);
    }

    void GoToMainMenu()
    {
        GameManager.Instance.LoadLevel("Main Menu");
        Hide();
    }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new ConfirmPopUp();
        pop.Initialize(_root, Quit);
    }

    void Quit()
    {
        Application.Quit();
    }

    public override void Hide()
    {
        BattleManager.Instance.ResumeGame();
        base.Hide();
        OnClose?.Invoke();
    }
}
