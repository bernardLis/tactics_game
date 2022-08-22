using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleUI : Singleton<BattleUI>
{
    GameManager _gameManager;
    RunManager _runManager;
    BattleManager _battleManager;

    public VisualElement Root { get; private set; }
    VisualElement _turnTextContainer;
    Label _turnText;

    VisualElement _battleLogContainer;
    Label _battleLogText;
    Queue<IEnumerator> _coroutineQueue = new();

    VisualElement _battleGoalContainer;

    VisualElement _battleEndContainer;
    Label _battleEndText;
    VisualElement _battleEndGoalContainer;
    VisualElement _battleEndCharacters;
    VisualElement _battleEndRewardContainer;
    MyButton _backToJourneyButton;

    string _turnTextTweenID = "turnTextTweenID";
    string _battleLogTweenID = "battleLogTweenID";

    public CharacterScreen CharacterScreen { get; private set; }

    public event Action OnBattleEndScreenShown;

    string _levelToLoadAfterFight = "Journey";

    protected override void Awake()
    {
        base.Awake();

        _gameManager = GameManager.Instance;
        _runManager = RunManager.Instance;

        _battleManager = BattleManager.Instance;

        // getting ui elements
        Root = GetComponent<UIDocument>().rootVisualElement;

        _turnTextContainer = Root.Q<VisualElement>("turnTextContainer");
        _turnText = Root.Q<Label>("turnText");
        _battleLogContainer = Root.Q<VisualElement>("battleLogContainer");
        _battleLogText = Root.Q<Label>("battleLogText");

        _battleGoalContainer = Root.Q<VisualElement>("battleGoalContainer");

        _battleEndContainer = Root.Q<VisualElement>("battleEndContainer");
        _battleEndText = Root.Q<Label>("battleEndText");
        _battleEndCharacters = Root.Q<VisualElement>("battleEndCharacters");
        _battleEndGoalContainer = Root.Q<VisualElement>("battleEndGoalContainer");
        _battleEndRewardContainer = Root.Q<VisualElement>("battleEndRewardContainer");

        _backToJourneyButton = new MyButton("Continue", "menuButton", null);
        _battleEndContainer.Add(_backToJourneyButton);

        // subscribing to Actions
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void Start()
    {
        StartCoroutine(CoroutineCoordinator());
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            DisplayTurnText("DEPLOY TROOPS");
        if (state == BattleState.EnemyTurn)
            DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - ENEMY");
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.Won)
            ShowBattleWonScreen();
        if (state == BattleState.Lost)
            ShowBattleLostScreen();
    }

    void HandlePlayerTurn()
    {
        if (TurnManager.CurrentTurn == 1)
            DisplayBattleGoal();

        DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - PLAYER");
    }

    void DisplayBattleGoal()
    {
        _battleGoalContainer.style.display = DisplayStyle.Flex;
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
        _battleManager.PauseGame();
        CharacterScreen = new CharacterScreen(character, Root);
    }

    public void HideCharacterScreen()
    {
        _battleManager.ResumeGame();
        CharacterScreen.Hide();
        CharacterScreen = null;
    }

    void ShowBattleWonScreen()
    {
        ShowBattleEndScreen();
        _battleEndCharacters.Clear();
        foreach (Character character in _runManager.PlayerTroops)
        {
            CharacterCardVisual visual = new CharacterCardVisual(character);
            _battleEndCharacters.Add(visual);
        }

        _battleEndText.text = $"You won in {TurnManager.CurrentTurn} turns!";

        VisualElement container = new();

        container.AddToClassList("textPrimary");
        container.style.flexDirection = FlexDirection.Row;
        if (_runManager.JourneyNodeReward.Gold != 0)
            container.Add(new Label($"Gold: {_runManager.JourneyNodeReward.Gold}"));
        if (_runManager.JourneyNodeReward.Item != null)
            container.Add(new ItemVisual(_runManager.JourneyNodeReward.Item));

        _battleEndRewardContainer.Add(new Label("Your reward:"));
        _battleEndRewardContainer.Add(container);

        _backToJourneyButton.clickable.clicked += BackToJourney;
    }

    void ShowBattleLostScreen()
    {
        ShowBattleEndScreen();
        _battleEndText.text = "You lost!";

        _backToJourneyButton.text = "Continue";
        _backToJourneyButton.clickable.clicked += BackToMainMenu;
    }

    void ShowBattleEndScreen()
    {
        _turnTextContainer.style.display = DisplayStyle.None;
        _battleGoalContainer.style.display = DisplayStyle.None;

        _battleEndGoalContainer.Clear();

        _battleEndContainer.style.display = DisplayStyle.Flex;
        _battleEndContainer.style.opacity = 0f;
        DOTween.To(() => _battleEndContainer.style.opacity.value, x => _battleEndContainer.style.opacity = x, 1f, 2f)
            .OnComplete(OnShowBattleEndScreenCompleted);
    }

    void OnShowBattleEndScreenCompleted()
    {
        _backToJourneyButton.style.display = DisplayStyle.Flex;
        OnBattleEndScreenShown?.Invoke();
    }

    void BackToJourney()
    {
        _gameManager.LoadLevel(_levelToLoadAfterFight);
        // reseting level to load to journey as it is the default
        _levelToLoadAfterFight = Scenes.Journey;
    }

    void BackToMainMenu()
    {
        _gameManager.LoadLevel(Scenes.MainMenu);
    }

    public void SetUpContinueButton(string newText, string newLevel)
    {
        _backToJourneyButton.text = newText;
        _levelToLoadAfterFight = newLevel;
        _backToJourneyButton.clickable.clicked += BackToJourney;
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

    public void AddGoalToBattleEndScreen(VisualElement el)
    {
        _battleEndGoalContainer.Add(el);
    }

}
