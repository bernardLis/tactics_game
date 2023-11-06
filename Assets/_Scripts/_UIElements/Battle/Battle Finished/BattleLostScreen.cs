using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleLostScreen : BattleFinishedScreen
{
    const string _ussClassName = "battle-lost__";
    const string _ussMain = _ussClassName + "main";

    public BattleLostScreen()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleLostStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddButtons();

        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlayDialogue(audioManager.GetSound("You Lost"));
    }

    protected override void AddTitle()
    {
        // meant to be overwritten
        Label text = new("Battle lost!");
        text.style.fontSize = 34;

        _mainContainer.Add(text);
    }

    void AddButtons()
    {
        VisualElement container = new();
        container.style.alignItems = Align.Center;
        _mainContainer.Add(container);

        Label text = new("Hey, you lost but you did very well! If you want to give it another go I will give you a bonus 1 000 gold. (it stacks)");
        text.style.fontSize = 24;
        text.style.whiteSpace = WhiteSpace.Normal;
        container.Add(text);

        MyButton takeAdvantage = new("Easy money!", _ussCommonMenuButton, AdvantageButton);
        container.Add(takeAdvantage);

        MyButton noAdvantage = new("I don't need your charity!", _ussCommonMenuButton, QuitButton);
        container.Add(noAdvantage);
    }

    void AdvantageButton()
    {
        QuitButton();
    }

    void QuitButton()
    {
        _gameManager.ClearSaveData();
        _gameManager.LoadScene(Scenes.MainMenu);
    }


}
