using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

public class QuestElement : VisualElement
{
    GameManager _gameManager;
    DeskManager _deskManager;
    DraggableCharacters _draggableCharacters;
    Report _report;
    Quest _quest;

    VisualElement _topPanelContainer;
    StarRankElement _rankVisualElement;
    VisualElement _additionalInfo;

    TextWithTooltip _expiryDateLabel;
    TextWithTooltip _durationLabel;
    TextWithTooltip _successChanceLabel;
    VisualElement _assignedCharactersContainer;
    MyButton _startAssignmentButton;

    List<CharacterCardMiniSlot> _cardSlots = new();

    public QuestElement(Report report)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        _deskManager = DeskManager.Instance;
        _draggableCharacters = _deskManager.GetComponent<DraggableCharacters>();

        _report = report;
        _quest = report.Quest;
        _quest.OnQuestStateChanged += OnQuestStateChanged;
        AddToClassList("questElement");
        AddToClassList("textPrimary");

        AddTopPanel();
        AddBottomPanel();

        if (_quest.QuestState == QuestState.Expired)
            HandlePendingQuest();
        if (_quest.QuestState == QuestState.Expired)
            HandleExpiredQuest();
        if (_quest.QuestState == QuestState.Delegated)
            HandleDelegatedQuest();
        if (_quest.QuestState == QuestState.Finished)
            HandleFinishedQuest();

        RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.NoTrickleDown);
    }

    void OnPointerDown(PointerDownEvent e) { e.StopImmediatePropagation(); }

    void OnDayPassed(int day)
    {
        UpdateExpiryDateLabel();
        UpdateDaysUntilFinished();
    }

    void OnQuestStateChanged(QuestState state)
    {
        UpdateExpiryDateLabel();
        UpdateDaysUntilFinished();
        UpdateStartAssignmentButton();

        if (state == QuestState.Pending)
            HandlePendingQuest();
        if (state == QuestState.Delegated)
            HandleDelegatedQuest();
        if (state == QuestState.Finished)
            HandleFinishedQuest();
        if (state == QuestState.Expired)
            HandleExpiredQuest();
    }

    void AddTopPanel()
    {
        _topPanelContainer = new();
        _topPanelContainer.AddToClassList("questTopPanelContainer");
        _topPanelContainer.AddToClassList("textPrimaryBlack");
        Add(_topPanelContainer);

        _topPanelContainer.Add(new QuestRankElement(_quest.Rank));
        _topPanelContainer.Add(new TextWithTooltip(_quest.Title, _quest.Description));
    }

    void AddBottomPanel()
    {
        _additionalInfo = new();
        _additionalInfo.AddToClassList("textPrimaryBlack");
        Add(_additionalInfo);

        _expiryDateLabel = new TextWithTooltip($"", "Has to be delegated or taken before it expiries.");
        UpdateExpiryDateLabel();
        _additionalInfo.Add(_expiryDateLabel);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        _durationLabel = new TextWithTooltip($"Duration: {_quest.Duration} day(s).", $"When delegated it will take {_quest.Duration} day(s)");
        container.Add(_durationLabel);

        _successChanceLabel = new TextWithTooltip($"Success chance: {_quest.GetSuccessChance()}%.", "The stronger/more the characters the higher success chance.");
        container.Add(_successChanceLabel);
        _additionalInfo.Add(container);

        _assignedCharactersContainer = CreateCharacterSlots();
        _additionalInfo.Add(_assignedCharactersContainer);

        _startAssignmentButton = CreateStartAssignmentButton();
        UpdateStartAssignmentButton();
        _additionalInfo.Add(_startAssignmentButton);
    }


    void HandlePendingQuest()
    {

    }

    void HandleDelegatedQuest()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        UpdateDaysUntilFinished();
        _startAssignmentButton.SetEnabled(false);
    }

    void HandleFinishedQuest()
    {

    }

    void HandleExpiredQuest()
    {
        ReturnAssignedCharacters();
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        VisualElement overlay = new VisualElement();
        overlay.AddToClassList("questElementOverlay");
        Add(overlay);
        overlay.BringToFront();

        Label text = new($"Expired! ({_quest.ExpiryDay})");
        text.AddToClassList("textPrimary");
        text.style.fontSize = 32;
        text.transform.rotation *= Quaternion.Euler(0f, 0f, 30f);
        overlay.Add(text);
    }

    async void ReturnAssignedCharacters()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
        {
            if (slot.Card == null)
                continue;
            slot.Card.Character.UpdateDeskPosition(new Vector2(slot.worldBound.x, slot.worldBound.y));
            await Task.Delay(500);
            _deskManager.SpitCharacterOntoDesk(slot.Card.Character);
            slot.RemoveCard();
        }
    }

    void UpdateExpiryDateLabel()
    {
        _expiryDateLabel.style.display = DisplayStyle.None;

        if (_quest.QuestState == QuestState.Pending)
        {
            _expiryDateLabel.style.display = DisplayStyle.Flex;
            _expiryDateLabel.UpdateText($"Expires in: {_quest.ExpiryDay - _gameManager.Day} days.");
        }
    }

    void UpdateDaysUntilFinished()
    {
        if (_quest.QuestState == QuestState.Delegated)
            _startAssignmentButton.UpdateButtonText($"Finished in: {_quest.CountDaysLeft()} days.");
        if (_quest.QuestState == QuestState.Expired)
            _startAssignmentButton.UpdateButtonText($"Expired.");
    }

    void UpdateSuccessChanceLabel()
    {
        if (IsPlayerAssigned())
            _successChanceLabel.UpdateText("Success is in your hands.");
        else
            _successChanceLabel.UpdateText($"Success chance: {_quest.GetSuccessChance()}%.");
    }

    VisualElement CreateThreatContainer()
    {
        VisualElement container = new VisualElement();
        container.Add(new Label("Threat: "));
        container.style.flexDirection = FlexDirection.Row;
        _additionalInfo.Add(container);

        foreach (var e in _quest.Enemies)
        {
            Label l = new Label();
            l.style.backgroundImage = e.BrainIcon.texture;
            l.style.width = 32;
            l.style.height = 32;
            container.Add(l);
        }

        return container;
    }

    VisualElement CreateRewardContainer()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignContent = Align.Center;
        _additionalInfo.Add(container);

        if (_quest.Reward.Gold != 0)
            container.Add(new GoldElement(_quest.Reward.Gold));
        if (_quest.Reward.Item != null)
            container.Add(new ItemSlot(new ItemElement(_quest.Reward.Item)));

        return container;
    }

    VisualElement CreateCharacterSlots()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _cardSlots = new();
        bool isLocked = AreCharacterCardsLocked();

        for (int i = 0; i < 3; i++)
        {
            CharacterCardMiniSlot slot = new CharacterCardMiniSlot(null, isLocked);
            _cardSlots.Add(slot);
            container.Add(slot);
        }

        for (int i = 0; i < _quest.AssignedCharacters.Count; i++)
        {
            CharacterCardMini card = new(_quest.AssignedCharacters[i]);
            _cardSlots[i].AddCard(card);
        }

        // after adding cards to not trigger delegates prematurely.
        foreach (CharacterCardMiniSlot slot in _cardSlots)
        {
            _draggableCharacters.AddDraggableSlot(slot); // after cards were added to make them draggable

            slot.OnCardAdded += OnCardAdded;
            slot.OnCardRemoved += OnCardRemoved;
        }

        return container;
    }

    bool AreCharacterCardsLocked()
    {
        if (_quest.QuestState == QuestState.Delegated)
            return true;
        if (_quest.QuestState == QuestState.Finished)
            return true;
        if (_quest.QuestState == QuestState.Expired)
            return true;
        if (_quest.QuestState == QuestState.RewardCollected)
            return true;

        return false;
    }

    MyButton CreateStartAssignmentButton()
    {
        MyButton button = new("Assign Characters!", "questActionButton", null);
        button.SetEnabled(false);
        Add(button);

        return button;
    }

    // TODO: oh nasty nasty function
    void UpdateStartAssignmentButton()
    {
        if (_startAssignmentButton == null)
            return;

        _startAssignmentButton.ClearCallbacks();
        _startAssignmentButton.SetEnabled(false);
        _startAssignmentButton.UpdateButtonText("Assign Characters!");
        _startAssignmentButton.RemoveFromClassList("questActionButtonFinished");
        _startAssignmentButton.RemoveFromClassList("questActionButtonPlayer");
        _startAssignmentButton.RemoveFromClassList("questActionButtonDelegate");
        _startAssignmentButton.AddToClassList("questActionButtonPending");

        if (_quest.QuestState == QuestState.RewardCollected)
        {
            _startAssignmentButton.style.visibility = Visibility.Hidden;
            return;
        }

        if (_quest.QuestState == QuestState.Finished)
        {
            _startAssignmentButton.ChangeCallback(SeeResults);
            _startAssignmentButton.SetEnabled(true);
            _startAssignmentButton.UpdateButtonText("See Results!");
            _startAssignmentButton.RemoveFromClassList("questActionButtonPending");
            _startAssignmentButton.AddToClassList("questActionButtonFinished");
            return;
        }

        if (IsPlayerAssigned())
        {
            _startAssignmentButton.ChangeCallback(StartBattle);
            _startAssignmentButton.SetEnabled(true);
            _startAssignmentButton.UpdateButtonText("Battle It Out!");
            _startAssignmentButton.RemoveFromClassList("questActionButtonPending");
            _startAssignmentButton.AddToClassList("questActionButtonPlayer");
            return;
        }

        if (_quest.AssignedCharacterCount() > 0)
        {
            _startAssignmentButton.ChangeCallback(DelegateBattle);
            _startAssignmentButton.SetEnabled(true);
            _startAssignmentButton.UpdateButtonText("Delegate It!");
            _startAssignmentButton.RemoveFromClassList("questActionButtonPending");
            _startAssignmentButton.AddToClassList("questActionButtonDelegate");
        }
    }

    void OnCardAdded(CharacterCardMini card)
    {
        _quest.AssignCharacter(card.Character);
        OnCardChange();
        _gameManager.SaveJsonData();
    }

    void OnCardRemoved(CharacterCardMini card)
    {
        _quest.RemoveAssignedCharacter(card.Character);
        OnCardChange();
        _gameManager.SaveJsonData();
    }

    void OnCardChange()
    {
        UpdateSuccessChanceLabel();
        UpdateStartAssignmentButton();
    }

    bool IsPlayerAssigned()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
        {
            if (slot.Card == null)
                continue;
            if (slot.Card.Character == _gameManager.PlayerTroops[0]) // TODO: incorrect
                return true;
        }
        return false;
    }

    void StartBattle() { _gameManager.StartBattle(_quest); }

    void DelegateBattle() { _quest.DelegateQuest(); }

    void SeeResults()
    {
        QuestResultsElement el = new QuestResultsElement(_deskManager.Root, _report);
        el.OnHide += OnResultsClosed;
    }

    void OnResultsClosed()
    {
        _quest.UpdateQuestState(QuestState.RewardCollected);
        ReturnAssignedCharacters();
    }

}
