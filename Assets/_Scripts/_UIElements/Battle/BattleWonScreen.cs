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

        MyButton continuePlaying = new("Continue playing", _ussCommonMenuButton,
                 callback: () => Hide());
        container.Add(continuePlaying);

        MyButton advantage = new("Quit (+1k gold next time)",
                _ussCommonMenuButton, callback: AdvantageButton);
        container.Add(advantage);

        MyButton noAdvantage = new("Quit", _ussCommonMenuButton, callback: QuiteButton);
        container.Add(noAdvantage);
    }

    void AdvantageButton()
    {
        _gameManager.GoldAdvantage++;
        QuiteButton();
    }

    void QuiteButton()
    {
        _gameManager.ClearSaveData();
        _gameManager.LoadScene(Scenes.MainMenu);
    }
}
