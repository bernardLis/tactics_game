using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class WonBattleScreen : FinishedBattleScreen
{
    const string _ussClassName = "finished-battle-screen__";
    const string _ussMain = _ussClassName + "won-main";

    public event Action OnContinuePlaying;
    public event Action OnFinishedPlaying;
    public WonBattleScreen()
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.FinishedBattleScreenStyles);
        if (ss != null) styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddButtons();

        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlayDialogue(audioManager.GetSound("You Won"));
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
        text.style.fontSize = 24;
        container.Add(text);

        MyButton continuePlaying = new("Continue playing", _ussCommonMenuButton,
                 callback: ContinuePlaying);
        container.Add(continuePlaying);

        MyButton advantage = new("Quit (+1k gold next time)",
                _ussCommonMenuButton, callback: AdvantageButton);
        container.Add(advantage);

        MyButton noAdvantage = new("Quit", _ussCommonMenuButton, callback: QuitButton);
        container.Add(noAdvantage);
    }

    void ContinuePlaying()
    {
        OnContinuePlaying?.Invoke();
        Hide();
    }

    void AdvantageButton()
    {
        QuitButton();
    }

    void QuitButton()
    {
        OnFinishedPlaying?.Invoke();
        _gameManager.LoadScene(Scenes.MainMenu);
    }
}
