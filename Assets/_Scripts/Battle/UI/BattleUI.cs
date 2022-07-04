using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleUI : Singleton<BattleUI>
{
    GameManager _gameManager;

    public VisualElement Root { get; private set; }
    VisualElement _turnTextContainer;
    Label _turnText;

    VisualElement _battleLogContainer;
    Label _battleLogText;
    Queue<IEnumerator> _coroutineQueue = new();

    VisualElement _battleEndContainer;
    Label _battleEndText;
    VisualElement _battleEndCharacters;
    Button _backToJourney;

    string _turnTextTweenID = "turnTextTweenID";
    string _battleLogTweenID = "battleLogTweenID";

    public CharacterScreen CharacterScreen { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _gameManager = GameManager.Instance;

        // getting ui elements
        Root = GetComponent<UIDocument>().rootVisualElement;

        _turnTextContainer = Root.Q<VisualElement>("turnTextContainer");
        _turnText = Root.Q<Label>("turnText");
        _battleLogContainer = Root.Q<VisualElement>("battleLogContainer");
        _battleLogText = Root.Q<Label>("battleLogText");

        _battleEndContainer = Root.Q<VisualElement>("battleEndContainer");
        _battleEndText = Root.Q<Label>("battleEndText");
        _battleEndCharacters = Root.Q<VisualElement>("battleEndCharacters");
        _backToJourney = Root.Q<Button>("backToJourney");

        _backToJourney.clickable.clicked += BackToJourney;

        // subscribing to Actions
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void Start()
    {
        StartCoroutine(CoroutineCoordinator());
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            DisplayTurnText("DEPLOY TROOPS");
        if (state == BattleState.EnemyTurn)
            DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - ENEMY");
        if (state == BattleState.PlayerTurn)
            DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - PLAYER");
        if (state == BattleState.Won)
            ShowBattleWonScreen();
        if (state == BattleState.Lost)
            ShowBattleLostScreen();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    public void DisplayBattleLog(string text)
    {
        _coroutineQueue.Enqueue(DisplayBattleLogCoroutine(text));
    }

    IEnumerator DisplayBattleLogCoroutine(string text)
    {
        DOTween.Kill(_battleLogTweenID);

        _battleLogText.text = text;
        _battleLogContainer.style.display = DisplayStyle.Flex;
        _battleLogContainer.style.opacity = 1f;

        yield return new WaitForSeconds(2);
        HideBattleLog();
    }

    void HideBattleLog()
    {
        DOTween.To(() => _battleLogContainer.style.opacity.value, x => _battleLogContainer.style.opacity = x, 0f, 0.5f)
            .OnComplete(() => _battleLogContainer.style.display = DisplayStyle.None)
            .SetId(_battleLogTweenID);
    }

    async void DisplayTurnText(string text)
    {
        if (DOTween.TweensById(_turnTextTweenID) != null)
            await DOTween.TweensById(_turnTextTweenID)[0].AsyncWaitForCompletion();

        _turnText.text = text;
        _turnTextContainer.style.display = DisplayStyle.Flex;

        DOTween.To(() => _turnTextContainer.style.opacity.value, x => _turnTextContainer.style.opacity = x, 1f, 2f)
            .OnComplete(HideTurnText)
            .SetId(_turnTextTweenID);
    }

    void HideTurnText()
    {
        DOTween.To(() => _turnTextContainer.style.opacity.value, x => _turnTextContainer.style.opacity = x, 0f, 2f)
            .OnComplete(() => _turnTextContainer.style.display = DisplayStyle.None)
            .SetId(_turnTextTweenID);
    }


    public void ShowCharacterScreen(Character character)
    {
        BattleManager.Instance.PauseGame();
        CharacterScreen = new CharacterScreen(character, Root);
    }

    public void HideCharacterScreen()
    {
        BattleManager.Instance.ResumeGame();
        CharacterScreen.Hide();
        CharacterScreen = null;
    }

    void ShowBattleWonScreen()
    {
        ShowBattleEndScreen();
        foreach (Character character in _gameManager.PlayerTroops)
        {
            CharacterCardVisual visual = new CharacterCardVisual(character);
            _battleEndCharacters.Add(visual);
        }

        _battleEndText.text = "WON!!";
    }

    void ShowBattleLostScreen()
    {
        ShowBattleEndScreen();
        _battleEndText.text = "LOST!!";
    }

    void ShowBattleEndScreen()
    {
        _turnTextContainer.style.display = DisplayStyle.None;

        _battleEndContainer.style.display = DisplayStyle.Flex;
        _battleEndContainer.style.opacity = 0f;
        DOTween.To(() => _battleEndContainer.style.opacity.value, x => _battleEndContainer.style.opacity = x, 1f, 2f)
            .OnComplete(() => _backToJourney.style.display = DisplayStyle.Flex);
    }

    void BackToJourney()
    {
        _gameManager.LoadLevel("Journey");
    }

    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (_coroutineQueue.Count > 0)
                yield return StartCoroutine(_coroutineQueue.Dequeue());
            yield return null;
        }
    }

}
