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

    VisualElement _battleHelperTextContainer;
    Label _battleHelperText;

    VisualElement _turnTextContainer;
    Label _turnText;

    VisualElement _battleLogContainer;
    //    Queue<IEnumerator> _coroutineQueue = new();

    VisualElement _battleGoalContainer;

    VisualElement _battleEndContainer;
    Label _battleEndText;
    VisualElement _battleEndGoalContainer;
    VisualElement _battleEndCharacters;
    VisualElement _battleEndRewardContainer;
    MyButton _backToJourneyButton;

    string _turnTextTweenID = "turnTextTweenID";

    public CharacterScreen CharacterScreen; // HERE: { get; private set; }

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

        _battleHelperTextContainer = Root.Q<VisualElement>("battleHelperTextContainer");
        _battleHelperText = Root.Q<Label>("battleHelperText");
        _battleGoalContainer = Root.Q<VisualElement>("battleGoalContainer");

        _turnTextContainer = Root.Q<VisualElement>("turnTextContainer");
        _turnText = Root.Q<Label>("turnText");
        _battleLogContainer = Root.Q<VisualElement>("battleLogContainer");

        _battleEndContainer = Root.Q<VisualElement>("battleEndContainer");
        _battleEndText = Root.Q<Label>("battleEndText");
        _battleEndCharacters = Root.Q<VisualElement>("battleEndCharacters");
        _battleEndGoalContainer = Root.Q<VisualElement>("battleEndGoalContainer");
        _battleEndRewardContainer = Root.Q<VisualElement>("battleEndRewardContainer");

        _backToJourneyButton = new MyButton("Continue", "menuButton", null);
        _battleEndContainer.Add(_backToJourneyButton);

        // subscribing to Actions
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged += OnCharacterStateChanged;
    }

    void Start()
    {
        _battleLogContainer.Clear();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged -= OnCharacterStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            HandleDeployment();
        if (state == BattleState.EnemyTurn)
            DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - ENEMY");
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.EnemyTurn)
            HandleEnemyTurn();
        if (state == BattleState.Won)
            ShowBattleWonScreen();
        if (state == BattleState.Lost)
            ShowBattleLostScreen();
    }

    void OnCharacterStateChanged(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.None:
                UpdateBattleHelperText($"Turn {TurnManager.CurrentTurn.ToString()}. Your turn. Select a character");
                break;
            case CharacterState.Selected:
                UpdateBattleHelperText("Select a destination.");
                break;
            case CharacterState.Moved:
                UpdateBattleHelperText("Select an ability.");
                break;
            case CharacterState.SelectingInteractionTarget:
                UpdateBattleHelperText("Choose a target.");
                break;
            case CharacterState.SelectingFaceDir:
                UpdateBattleHelperText("Select face direction.");
                break;
            case CharacterState.ConfirmingInteraction:
                UpdateBattleHelperText("Confirm interaction.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void HandleDeployment()
    {
        DisplayTurnText("DEPLOY TROOPS");
        UpdateBattleHelperText("Deployment phase. Place your characters.");
    }

    void HandlePlayerTurn()
    {
        if (TurnManager.CurrentTurn == 1)
            DisplayBattleGoal();

        _battleHelperTextContainer.style.display = DisplayStyle.Flex;
        UpdateBattleHelperText($"Turn {TurnManager.CurrentTurn.ToString()}. Your turn. Select a character");
        DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - PLAYER");
    }

    void HandleEnemyTurn()
    {
        UpdateBattleHelperText($"Turn {TurnManager.CurrentTurn.ToString()}. Enemy turn.");
    }

    void UpdateBattleHelperText(string txt)
    {
        _battleHelperText.text = txt;
    }

    void DisplayBattleGoal()
    {
        _battleGoalContainer.style.display = DisplayStyle.Flex;
    }

    public void DisplayBattleLog(VisualElement element)
    {
        _battleLogContainer.Add(element);
        if (_battleLogContainer.childCount > 5)
        {
            List<VisualElement> ch = new(_battleLogContainer.Children());
            Debug.Log($"ch.count: {ch.Count}");
            _battleLogContainer.RemoveAt(0);
        }
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
        if (CharacterScreen != null)
            return;
        _battleManager.PauseGame();
        CharacterScreen = new CharacterScreen(character, Root);
        CharacterScreen.OnHide += HideCharacterScreen;
    }

    public void HideCharacterScreen()
    {
        _battleManager.ResumeGame();
        CharacterScreen.OnHide -= HideCharacterScreen;
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

        _backToJourneyButton.clickable.clicked += OnContinueButtonClick;

        if (_runManager.JourneyNodeReward == null)
            return;

        VisualElement container = new();
        container.AddToClassList("textPrimary");
        container.style.flexDirection = FlexDirection.Row;
        if (_runManager.JourneyNodeReward.Gold != 0)
            container.Add(new Label($"Gold: {_runManager.JourneyNodeReward.Gold}"));
        if (_runManager.JourneyNodeReward.Item != null)
            container.Add(new ItemVisual(_runManager.JourneyNodeReward.Item));

        _battleEndRewardContainer.Add(new Label("Your reward:"));
        _battleEndRewardContainer.Add(container);
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
        _battleHelperTextContainer.style.display = DisplayStyle.None;

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

    void OnContinueButtonClick()
    {
        if (_levelToLoadAfterFight == null)
            _levelToLoadAfterFight = Scenes.Journey;
        _gameManager.LoadLevel(_levelToLoadAfterFight);
    }

    void BackToMainMenu()
    {
        _gameManager.LoadLevel(Scenes.MainMenu);
    }

    public void SetUpContinueButton(string newText, string newLevel)
    {
        _backToJourneyButton.text = newText;
        _levelToLoadAfterFight = newLevel;
    }

    public void AddGoalToBattleEndScreen(VisualElement el)
    {
        _battleEndGoalContainer.Add(el);
    }

}
