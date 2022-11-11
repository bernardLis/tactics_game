using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestVisualElement : VisualElement
{
    GameManager _gameManager;
    DeskManager _deskManager;
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

    bool _isDelegated; // TODO: dunno why I get multiple callbacks when assigning and removing characters.

    public QuestVisualElement(Quest quest)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        _deskManager = DeskManager.Instance;

        _quest = quest;
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
        if (_quest.QuestState == QuestState.Won || _quest.QuestState == QuestState.Lost)
            HandleFinishedQuest();
    }

    void OnDayPassed(int day)
    {
        //ReturnCharacters();
        UpdateExpiryDateLabel();
        UpdateDaysUntilFinishedLabel();
    }
    /*
        void ReturnCharacters()
        {
            if (_quest.QuestState != QuestState.Pending)
                return;

            foreach (CharacterCardMiniSlot slot in _cardSlots)
            {
                if (slot.Card == null)
                    continue;

                _deskManager.AddCharacterToDraggableTroops(slot.Card.Character);
                slot.RemoveCard();
            }
        }
        */

    void OnQuestStateChanged(QuestState state)
    {
        if (state == QuestState.Pending)
            Debug.Log($"pending");
        if (state == QuestState.Delegated)
            Debug.Log($"Delegated");
        if (state == QuestState.Won || state == QuestState.Lost)
            Debug.Log($"Finished");
        if (state == QuestState.Expired)
            Debug.Log($"Expired");
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
        _additionalInfo.Add(_startAssignementButton);
    }


    void HandlePendingQuest()
    {

    }

    void HandleDelegatedQuest()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        _startAssignementButton.text = "Assigned!";
        _startAssignementButton.SetEnabled(false);
    }

    void HandleFinishedQuest()
    {

    }

    void HandleExpiredQuest()
    {
        ExpiryCheck();
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
        if (_quest.QuestState == QuestState.Won || _quest.QuestState == QuestState.Lost)
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
        if (_quest.QuestState == QuestState.Won || _quest.QuestState == QuestState.Lost)
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
        for (int i = 0; i < 3; i++)
        {
            bool isLocked = _quest.QuestState == QuestState.Delegated;
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
            slot.OnCardAdded += OnCardAdded;
            slot.OnCardRemoved += OnCardRemoved;
        }

        return container;
    }

    MyButton CreateStartAssignmentButton()
    {
        MyButton button = new("Assign Characters!", "questActionButton", StartBattle);
        button.SetEnabled(false);
        Add(button);

        return button;
    }

    void UpdateStartAssignmentButton()
    {
        if (_startAssignementButton == null)
            return;
        _startAssignementButton.ChangeCallback(null);
        _startAssignementButton.SetEnabled(false);
        _startAssignementButton.text = "Assign Characters!";

        if (IsPlayerAssigned())
        {
            _startAssignementButton.ChangeCallback(StartBattle);
            _startAssignementButton.text = "Battle It Out!";
            _startAssignementButton.SetEnabled(true);
            return;
        }

        if (_quest.AssignedCharacterCount() > 0)
        {
            _startAssignementButton.ChangeCallback(DelegateBattle);
            _startAssignementButton.text = "Delegate It Out!";
            _startAssignementButton.SetEnabled(true);
        }
    }

    void OnCardAdded(CharacterCardMini card)
    {
        Debug.Log($"on card added {card.Character.CharacterName}");
        _quest.AssignCharacter(card.Character);
        OnCardChange();
    }

    void OnCardRemoved(CharacterCardMini card)
    {
        _quest.RemoveAssignedCharacter(card.Character);
        OnCardChange();
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

    void DelegateBattle()
    {
        Debug.Log($"Delegate!");
        if (_isDelegated)
            return;
        _isDelegated = true;

        _quest.DelegateQuest();
    }

    void ExpiryCheck()
    {
        if (!_quest.IsExpired())
            return;

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
}
