using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuScreen : FullScreenVisual
{
    GameManager _gameManager;
    public event Action OnClose;

    public MenuScreen(VisualElement root)
    {
        _gameManager = GameManager.Instance;

        Initialize(root);
        AddToClassList("menuScreen");
        AddButtons();
        AddGlobals();

        var tempColor = style.backgroundColor.value;
        tempColor.a = 0.5f;
        style.backgroundColor = tempColor;

        if (BattleManager.Instance != null)
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

    void AddGlobals()
    {
        Label obols = new Label($"Obols: {_gameManager.Obols}");
        obols.AddToClassList("textPrimary");

        VisualElement container = new VisualElement();
        foreach (GlobalUpgrade u in _gameManager.PurchasedGlobalUpgrades)
        {
            GlobalUpgradeVisual visual = new(u, true, false);
            container.Add(visual);
        }

        Add(obols);
        Add(container);
    }

    void ShowSettingsScreen()
    {
        new SettingsScreen(_root, this);
    }

    void GoToMainMenu()
    {
        GameManager.Instance.LoadLevel(Scenes.MainMenu);
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
        if (BattleManager.Instance != null)
            BattleManager.Instance.ResumeGame();

        base.Hide();
        OnClose?.Invoke();
    }
}
