using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleWonScreen : BattleFinishedScreen
{
    const string _ussClassName = "battle-won__";
    const string _ussMain = _ussClassName + "main";

    public BattleWonScreen()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleWonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        AddButtons();
    }

    protected override void AddTitle()
    {
        // meant to be overwritten
        Label text = new("Battle Won!");
        text.style.fontSize = 34;

        _mainContainer.Add(text);
    }

    void AddButtons()
    {
        VisualElement container = new();
        container.style.alignItems = Align.Center;
        _mainContainer.Add(container);

        Label text = new("You won, congratz! Here, I am giving you a virtual handshake <handshake>… If you want you can continue playing, the game can go on forever, but I have not balanced it. Let me know what you think about this experience. Cheers!");
        text.style.whiteSpace = WhiteSpace.Normal;
        container.Add(text);

        Button takeAdvantage = new() { text = "Continue playing!" };
        takeAdvantage.AddToClassList(_ussCommonMenuButton);
        takeAdvantage.clicked += () =>
        {
            Hide();
        };
        container.Add(takeAdvantage);

        Button advantage = new() { text = "Quit and get advantage for your next run. (1k)" };
        advantage.AddToClassList(_ussCommonMenuButton);
        advantage.clicked += () =>
        {
            _gameManager.GoldAdvantage++;
            _gameManager.ClearSaveData();
            _gameManager.LoadScene(Scenes.MainMenu);
        };
        container.Add(advantage);

        Button noAdvantage = new() { text = "Quit" };
        noAdvantage.AddToClassList(_ussCommonMenuButton);
        noAdvantage.clicked += () =>
        {
            _gameManager.GoldAdvantage++;
            _gameManager.ClearSaveData();
            _gameManager.LoadScene(Scenes.MainMenu);
        };
        container.Add(noAdvantage);
    }
}
