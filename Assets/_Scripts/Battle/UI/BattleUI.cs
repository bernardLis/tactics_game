using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleUI : Singleton<BattleUI>
{
    GameManager _gameManager;
    BattleManager _battleManager;

    public VisualElement Root { get; private set; }

    VisualElement _battleHelperTextContainer;
    Label _battleHelperText;

    VisualElement _battleLogContainer;

    FullScreenElement _battleEndScreen;
    RewardContainer _battleRewardsContainer;
    bool _wasRewardWarningDisplayed;
    VisualElement _battleEndGoalContainer;

    MyButton _backToDashboardButton;

    public CharacterScreen CharacterScreen { get; private set; }

    public event Action OnBattleEndScreenShown;

    string _levelToLoadAfterFight = Scenes.Dashboard;
    List<VisualElement> _battleLogs = new();
    List<VisualElement> _battleLogsCopy = new();
    BattleLogScreen _battleLogVisual;

    protected override void Awake()
    {
        base.Awake();

        _gameManager = GameManager.Instance;

        _battleManager = BattleManager.Instance;

        // getting ui elements
        Root = GetComponent<UIDocument>().rootVisualElement;

        _battleHelperTextContainer = Root.Q<VisualElement>("battleHelperTextContainer");
        _battleHelperText = Root.Q<Label>("battleHelperText");

        _battleLogContainer = Root.Q<VisualElement>("battleLogContainer");
        _battleLogContainer.RegisterCallback<PointerUpEvent>(OnBattleLogClick);

        _battleEndGoalContainer = new(); // HERE: Root.Q<VisualElement>("battleEndGoalContainer");
        Label goalHeader = new("Goals: ");
        goalHeader.AddToClassList("textPrimary");
        _battleEndGoalContainer.Add(goalHeader);

        _backToDashboardButton = new MyButton("Continue", "menuButton", null);

        // show / hide UI
        ToggleBattleHelperText(PlayerPrefs.GetInt("HideBattleHelperText") != 0);
        ToggleBattleLog(PlayerPrefs.GetInt("HideBattleLog") != 0);

        // subscribing to Actions
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged += OnCharacterStateChanged;
    }

    void Start() { _battleLogContainer.Clear(); }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged -= OnCharacterStateChanged;
    }
    public void HideAllUI() { Root.style.visibility = Visibility.Hidden; }

    public void ShowAllUI() { Root.style.visibility = Visibility.Visible; }

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
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void HandleDeployment()
    {
        _battleHelperTextContainer.style.display = DisplayStyle.Flex;
        DisplayTurnText("DEPLOY TROOPS");
        UpdateBattleHelperText("Deployment phase. Place your characters.");
    }

    void HandlePlayerTurn()
    {
        _battleHelperTextContainer.style.display = DisplayStyle.Flex;
        UpdateBattleHelperText($"Your turn. Select a character");
        DisplayTurnText("TURN " + TurnManager.CurrentTurn.ToString() + " - PLAYER");
    }

    void HandleEnemyTurn() { UpdateBattleHelperText($"Turn {TurnManager.CurrentTurn.ToString()}. Enemy turn."); }

    void UpdateBattleHelperText(string txt) { _battleHelperText.text = txt; }

    public void DisplayBattleLog(BattleLogLine element)
    {
        _battleLogs.Add(element);
        _battleLogContainer.Add(element);
        if (_battleLogContainer.childCount > 5)
            _battleLogContainer.RemoveAt(0);

        GrayOutOldLogs();
    }

    void PopulateBattleLog()
    {
        _battleLogVisual.OnHide -= PopulateBattleLog;
        _battleLogVisual = null;

        List<VisualElement> ch = new(_battleLogContainer.Children());
        ch.Reverse();
        for (int i = 0; i < _battleLogs.Count; i++)
        {
            if (i > 7)
                break;
            _battleLogContainer.Add(_battleLogs[i]);
        }
        GrayOutOldLogs();
    }

    void GrayOutOldLogs()
    {
        List<VisualElement> ch = new(_battleLogContainer.Children());
        ch.Reverse();

        for (int i = 0; i < ch.Count; i++)
        {
            float op = 1 - i * 0.08f;
            ch[i].style.opacity = op;
        }
    }

    void OnBattleLogClick(PointerUpEvent evt)
    {
        if (evt.button != 0) // only left mouse click
            return;
        _battleLogVisual = new BattleLogScreen(new List<VisualElement>(_battleLogs));
        _battleLogVisual.Initialize(Root);
        _battleLogVisual.OnHide += PopulateBattleLog;
    }

    void DisplayTurnText(string text)
    {
        DisplayBattleLog(new BattleLogLine(new Label(text), BattleLogLineType.Info));
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
        CharacterScreen.Hide();
        CharacterScreen = null;
    }

    void BaseBattleEndScreen()
    {
        _battleEndScreen = new();
        _battleEndScreen.Initialize(Root, false);
        _battleEndScreen.style.opacity = 0f;
        _battleEndScreen.style.justifyContent = Justify.Center;
        _battleEndScreen.style.alignItems = Align.Center;

        ShowBattleEndScreen();

        // header
        string txt = "";
        if (TurnManager.BattleState == BattleState.Won)
            txt = $"You won in {TurnManager.CurrentTurn} turns!";
        else
            txt = "You lost.";

        Label battleEndText = new(txt);
        battleEndText.AddToClassList("textPrimary");
        battleEndText.style.fontSize = 48;
        _battleEndScreen.Add(battleEndText);

        // goals
        _battleEndScreen.Add(_battleEndGoalContainer);

        _backToDashboardButton.clickable.clicked += BackToDashboard;
        _battleEndScreen.Add(_backToDashboardButton);

    }

    void ShowBattleWonScreen() { BaseBattleEndScreen(); }

    void ShowBattleLostScreen() { BaseBattleEndScreen(); }

    void ShowBattleEndScreen()
    {
        _battleEndGoalContainer.Clear();

        DOTween.To(() => _battleEndScreen.style.opacity.value, x => _battleEndScreen.style.opacity = x, 1f, 2f)
            .OnComplete(OnShowBattleEndScreenCompleted);
    }

    void OnShowBattleEndScreenCompleted()
    {
        _backToDashboardButton.style.display = DisplayStyle.Flex;
        OnBattleEndScreenShown?.Invoke();
    }

    void BackToDashboard() { _gameManager.LoadLevel(Scenes.Dashboard); }

    public void SetUpContinueButton(string newText, string newLevel)
    {
        _backToDashboardButton.text = newText;
        _levelToLoadAfterFight = newLevel;
    }

    public void AddGoalToBattleEndScreen(VisualElement el) { _battleEndGoalContainer.Add(el); }

    public void ToggleBattleHelperText(bool hide)
    {
        if (hide)
            _battleHelperTextContainer.style.display = DisplayStyle.None;
        else
            _battleHelperTextContainer.style.display = DisplayStyle.Flex;
    }

    public void ToggleBattleLog(bool hide)
    {
        if (hide)
            _battleLogContainer.style.display = DisplayStyle.None;
        else
            _battleLogContainer.style.display = DisplayStyle.Flex;
    }
}
