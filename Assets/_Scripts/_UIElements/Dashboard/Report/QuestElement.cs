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
    MyButton _actionButton;

    List<CharacterCardMiniSlot> _cardSlots = new();


    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonTextPrimaryBlack = "common__text-primary-black";

    const string _ussClassName = "quest-element";
    const string _ussMain = _ussClassName + "__main";
    const string _ussTopPanelContainer = _ussClassName + "__top-panel-container";
    const string _ussOverlay = _ussClassName + "__overlay";

    const string _ussActionButton = _ussClassName + "__action-button";
    const string _ussActionButtonPending = _ussClassName + "__action-button-pending";
    const string _ussActionButtonFinished = _ussClassName + "__action-button-finished";
    const string _ussActionButtonPlayer = _ussClassName + "__action-button-player";
    const string _ussActionButtonDelegate = _ussClassName + "__action-button-delegate";

    public QuestElement(Report report)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
        _deskManager = DeskManager.Instance;
        _draggableCharacters = _deskManager.GetComponent<DraggableCharacters>();

        _report = report;
        _quest = report.Quest;
        _quest.OnQuestStateChanged += OnQuestStateChanged;

        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.QuestElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        AddTopPanel();
        AddBottomPanel();

        if (_quest.QuestState == QuestState.Expired)
            HandleExpiredQuest();
        if (_quest.QuestState == QuestState.Delegated)
            HandleDelegatedQuest();

        RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.NoTrickleDown);
    }

    // block report pickup
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
        UpdateActionButton();

        if (state == QuestState.Delegated)
            HandleDelegatedQuest();
        if (state == QuestState.Expired)
            HandleExpiredQuest();
    }

    void AddTopPanel()
    {
        _topPanelContainer = new();
        _topPanelContainer.AddToClassList(_ussTopPanelContainer);
        _topPanelContainer.AddToClassList(_ussCommonTextPrimaryBlack);
        Add(_topPanelContainer);

        _topPanelContainer.Add(new TextWithTooltip(_quest.Title, _quest.Description));
        _topPanelContainer.Add(new QuestRankElement(_quest));
    }

    void AddBottomPanel()
    {
        _additionalInfo = new();
        _additionalInfo.AddToClassList(_ussCommonTextPrimaryBlack);
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

        _actionButton = CreateActionButton();
        UpdateActionButton();
        _additionalInfo.Add(_actionButton);
    }

    void HandleDelegatedQuest()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        UpdateDaysUntilFinished();
        _actionButton.SetEnabled(false);
    }

    void HandleExpiredQuest()
    {
        ReturnAssignedCharacters();
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        VisualElement overlay = new VisualElement();
        overlay.AddToClassList(_ussOverlay);
        Add(overlay);
        overlay.BringToFront();

        Label text = new($"Expired! ({_quest.ExpiryDay})");
        text.AddToClassList(_ussCommonTextPrimary);
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
            _actionButton.UpdateButtonText($"Finished in: {_quest.CountDaysLeft()} days.");
        if (_quest.QuestState == QuestState.Expired)
            _actionButton.UpdateButtonText($"Expired.");
    }

    void UpdateSuccessChanceLabel()
    {
        if (_quest.IsPlayerAssigned())
            _successChanceLabel.UpdateText("Success is in your hands.");
        else
            _successChanceLabel.UpdateText($"Success chance: {_quest.GetSuccessChance()}%.");
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
            _deskManager.RegisterCardMiniDrag(card);
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

    MyButton CreateActionButton()
    {
        MyButton button = new("Assign Characters!", _ussActionButton, null);
        button.SetEnabled(false);
        Add(button);

        return button;
    }

    void UpdateActionButton()
    {
        if (_actionButton == null)
            return;

        HandleActionButtonDefault();

        if (_quest.QuestState == QuestState.RewardCollected)
        {
            _actionButton.style.visibility = Visibility.Hidden;
            return;
        }

        if (_quest.QuestState == QuestState.Finished)
        {
            HandleActionButton(SeeResults, "See Results!", _ussActionButtonFinished);
            return;
        }

        if (_quest.IsPlayerAssigned())
        {
            HandleActionButton(StartBattle, "Battle It Out!", _ussActionButtonPlayer);
            return;
        }

        if (_quest.AssignedCharacterCount() > 0)
            HandleActionButton(DelegateBattle, "Delegate It!", _ussActionButtonDelegate);
    }

    void HandleActionButtonDefault()
    {
        _actionButton.ClearCallbacks();
        _actionButton.SetEnabled(false);
        _actionButton.UpdateButtonText("Assign Characters!");
        _actionButton.RemoveFromClassList(_ussActionButtonFinished);
        _actionButton.RemoveFromClassList(_ussActionButtonPlayer);
        _actionButton.RemoveFromClassList(_ussActionButtonDelegate);
        _actionButton.AddToClassList(_ussActionButtonPending);
    }

    void HandleActionButton(Action callback, string text, string className)
    {
        _actionButton.SetEnabled(true);
        _actionButton.RemoveFromClassList(_ussActionButtonPending);

        _actionButton.ChangeCallback(callback);
        _actionButton.UpdateButtonText(text);
        _actionButton.AddToClassList(className);
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
        UpdateActionButton();
    }
    /*
        bool IsPlayerAssigned()
        {
            foreach (CharacterCardMiniSlot slot in _cardSlots)
            {
                if (slot.Card == null)
                    continue;
                Debug.Log($"slot.Card.Character.name: {}");

                if (slot.Card.Character == _gameManager.PlayerTroops[0]) // TODO: incorrect
                    return true;
            }
            return false;
        }
        */

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
