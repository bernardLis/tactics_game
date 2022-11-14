using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestVisualElement : VisualElement
{
    GameManager _gameManager;
    DeskManager _deskManager;
    DraggableCharacters _draggableCharacters;
    Report _report;
    Quest _quest;

    VisualElement _basicInfoContainer;
    VisualElement _additionalInfo;

    TextWithTooltip _expiryDateLabel;
    TextWithTooltip _durationLabel;
    TextWithTooltip _daysUntilFinishedLabel;
    TextWithTooltip _successChanceLabel;
    VisualElement _threatContainer;
    VisualElement _rewardContainer;
    VisualElement _assignedCharactersContainer;
    MyButton _startAssignementButton;

    List<CharacterCardMiniSlot> _cardSlots = new();

    public QuestVisualElement(Report report)
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

        AddBasicInfo();

        _additionalInfo = new();
        _additionalInfo.AddToClassList("textPrimary");
        Add(_additionalInfo);

        PopulateAdditionalInfo();

        if (_quest.QuestState == QuestState.Expired)
            HandlePendingQuest();
        if (_quest.QuestState == QuestState.Expired)
            HandleExpiredQuest();
        if (_quest.QuestState == QuestState.Delegated)
            HandleDelegatedQuest();
        if (_quest.QuestState == QuestState.Finished)
            HandleFinishedQuest();
    }

    void OnDayPassed(int day)
    {
        UpdateExpiryDateLabel();
        UpdateDaysUntilFinishedLabel();
    }

    void OnQuestStateChanged(QuestState state)
    {
        UpdateExpiryDateLabel();
        UpdateDaysUntilFinishedLabel();
        UpdateStartAssignmentButton();

        if (state == QuestState.Pending)
            Debug.Log($"pending");
        if (state == QuestState.Delegated)
            HandleDelegatedQuest();
        if (state == QuestState.Finished)
            HandleFinishedQuest();
        if (state == QuestState.Expired)
            HandleExpiredQuest();
    }

    void PopulateAdditionalInfo()
    {
        _expiryDateLabel = new TextWithTooltip($"", "Has to be delegated or taken before it expiries.");
        UpdateExpiryDateLabel();
        _additionalInfo.Add(_expiryDateLabel);

        _durationLabel = new TextWithTooltip($"If delegated it takes: {_quest.Duration} days", "");
        _additionalInfo.Add(_durationLabel);

        _daysUntilFinishedLabel = new TextWithTooltip($"{_quest.CountDaysLeft()} days left.", "");
        UpdateDaysUntilFinishedLabel();
        _additionalInfo.Add(_daysUntilFinishedLabel);

        _successChanceLabel = new TextWithTooltip($"Success chance: {_quest.GetSuccessChance()}%", "");
        _additionalInfo.Add(_successChanceLabel);

        _threatContainer = CreateThreatContainer();
        _additionalInfo.Add(_threatContainer);

        _rewardContainer = CreateRewardContainer();
        _additionalInfo.Add(_rewardContainer);

        _assignedCharactersContainer = CreateCharacterSlots();
        _additionalInfo.Add(_assignedCharactersContainer);

        _startAssignementButton = CreateStartAssignmentButton();
        UpdateStartAssignmentButton();
        _additionalInfo.Add(_startAssignementButton);
    }


    void HandlePendingQuest()
    {

    }

    void HandleDelegatedQuest()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        _startAssignementButton.UpdateButtonText("Assigned!");
        _startAssignementButton.SetEnabled(false);
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
        Add(overlay);
        overlay.BringToFront();
        overlay.style.position = Position.Absolute;
        overlay.style.width = Length.Percent(105);
        overlay.style.height = Length.Percent(105);
        overlay.style.alignSelf = Align.Center;
        overlay.style.alignItems = Align.Center;
        overlay.style.justifyContent = Justify.Center;
        overlay.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));

        Label text = new($"Expired! ({_quest.ExpiryDay})");
        text.AddToClassList("textPrimary");
        text.style.fontSize = 32;
        text.transform.rotation *= Quaternion.Euler(0f, 0f, 45f);
        overlay.Add(text);
    }

    void ReturnAssignedCharacters()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
        {
            if (slot.Card == null)
                continue;
            _deskManager.AddCharacterToDraggableTroops(slot.Card.Character);
            slot.RemoveCard();
        }
    }

    void AddBasicInfo()
    {
        _basicInfoContainer = new();
        _basicInfoContainer.AddToClassList("questBasicInfoContainer");
        _basicInfoContainer.AddToClassList("textPrimary");
        Add(_basicInfoContainer);

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(_quest.Icon.Icon);
        icon.style.width = 50;
        icon.style.height = 50;
        _basicInfoContainer.Add(icon);

        Label title = new(_quest.Title);
        _basicInfoContainer.Add(title);
    }

    void UpdateExpiryDateLabel()
    {
        if (_quest.QuestState == QuestState.Pending)
            _expiryDateLabel.UpdateText($"Expires in: {_quest.ExpiryDay - _gameManager.Day} days.");
        if (_quest.QuestState == QuestState.Delegated)
            _expiryDateLabel.UpdateText($"Does not expire.");
        if (_quest.QuestState == QuestState.Finished)
            _expiryDateLabel.UpdateText($"Does not expire.");
        if (_quest.QuestState == QuestState.Expired)
            _expiryDateLabel.UpdateText($"Expired.");
    }

    void UpdateDaysUntilFinishedLabel()
    {
        if (_quest.QuestState == QuestState.Pending)
            _daysUntilFinishedLabel.UpdateText($"Waiting for assignement.");
        if (_quest.QuestState == QuestState.Delegated)
            _daysUntilFinishedLabel.UpdateText($"Finished in: {_quest.CountDaysLeft()} days.");
        if (_quest.QuestState == QuestState.Finished)
            _daysUntilFinishedLabel.UpdateText($"Finished.");
        if (_quest.QuestState == QuestState.Expired)
            _expiryDateLabel.UpdateText($"Expired.");
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
            container.Add(new ItemSlotVisual(new ItemVisual(_quest.Reward.Item)));

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
        if (_startAssignementButton == null)
            return;
        _startAssignementButton.ClearCallbacks();
        _startAssignementButton.SetEnabled(false);
        _startAssignementButton.UpdateButtonText("Assign Characters!");
        _startAssignementButton.UpdateButtonColor(Helpers.GetColor(QuestState.Pending.ToString()));

        if (_quest.QuestState == QuestState.RewardCollected)
        {
            _startAssignementButton.UpdateButtonText("-");
            return;
        }

        if (_quest.QuestState == QuestState.Finished)
        {
            _startAssignementButton.ChangeCallback(SeeResults);
            _startAssignementButton.SetEnabled(true);
            _startAssignementButton.UpdateButtonText("See Results!");
            _startAssignementButton.UpdateButtonColor(Helpers.GetColor(QuestState.Finished.ToString()));
            return;
        }

        if (IsPlayerAssigned())
        {
            _startAssignementButton.ChangeCallback(StartBattle);
            _startAssignementButton.SetEnabled(true);
            _startAssignementButton.UpdateButtonText("Battle It Out!");
            _startAssignementButton.UpdateButtonColor(Helpers.GetColor(QuestState.Delegated.ToString()));
            return;
        }

        if (_quest.AssignedCharacterCount() > 0)
        {
            _startAssignementButton.ChangeCallback(DelegateBattle);
            _startAssignementButton.SetEnabled(true);
            _startAssignementButton.UpdateButtonText("Delegate It Out!");
            _startAssignementButton.UpdateButtonColor(Helpers.GetColor(QuestState.Delegated.ToString()));
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
        Debug.Log($"see results clicked");
        QuestResultsVisualElement el = new QuestResultsVisualElement(_deskManager.Root, _report);
        el.OnHide += OnResultsClosed;
    }

    void OnResultsClosed()
    {
        _quest.UpdateQuestState(QuestState.RewardCollected);
        // character are returned to player troops after reward is collected and window closed
        ReturnAssignedCharacters();
    }

}
