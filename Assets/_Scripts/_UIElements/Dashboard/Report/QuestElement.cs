using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class QuestElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonTextPrimaryBlack = "common__text-primary-black";

    const string _ussClassName = "quest-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTopPanelContainer = _ussClassName + "top-panel-container";
    const string _ussOverlay = _ussClassName + "overlay";

    const string _ussActionButton = _ussClassName + "action-button";
    const string _ussActionButtonPending = _ussClassName + "action-button-pending";
    const string _ussActionButtonFinished = _ussClassName + "action-button-finished";
    const string _ussActionButtonPlayer = _ussClassName + "action-button-player";
    const string _ussActionButtonDelegate = _ussClassName + "action-button-delegate";

    GameManager _gameManager;
    DeskManager _deskManager;
    DraggableCharacters _draggableCharacters;
    Report _report;
    Quest _quest;

    VisualElement _topPanel;
    QuestRankElement _questRankElement;
    VisualElement _bottomPanel;

    TextWithTooltip _durationLabel;
    TextWithTooltip _successChanceLabel;
    VisualElement _assignedCharactersContainer;
    MyButton _actionButton;

    List<CharacterCardMiniSlot> _cardSlots = new();

    CampBuildingQuestInfo _questInfoBuilding;

    public QuestElement(Report report)
    {
        _gameManager = GameManager.Instance;
        _deskManager = DeskManager.Instance;
        _draggableCharacters = _deskManager.GetComponent<DraggableCharacters>();

        BuildingManager bm = _gameManager.GetComponent<BuildingManager>();
        _questInfoBuilding = bm.QuestInfoBuilding;
        _questInfoBuilding.OnUpgraded += OnQuestInfoBuildingUpgraded;

        _report = report;
        _quest = report.Quest;
        _quest.OnQuestStateChanged += OnQuestStateChanged;

        var common = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.QuestElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        AddTopPanel();
        AddBottomPanel();
        HandleQuestState();

        RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.NoTrickleDown);
    }

    void OnPointerDown(PointerDownEvent e) { e.StopImmediatePropagation(); }    // block report pickup

    void OnQuestInfoBuildingUpgraded(int rank)
    {
        UpdateSuccessChanceLabel();
        _questRankElement.UpdateElementalElement(rank);
    }

    void OnQuestStateChanged(QuestState state)
    {
        HandleQuestState();
        UpdateActionButton();
    }

    void HandleQuestState()
    {
        if (_quest.QuestState == QuestState.Expired)
            HandleExpiredQuest();
        if (_quest.QuestState == QuestState.Delegated)
            HandleDelegatedQuest();
        if (_quest.QuestState == QuestState.RewardCollected)
            HandleRewardCollected();
    }

    void AddTopPanel()
    {
        _topPanel = new();
        _topPanel.AddToClassList(_ussTopPanelContainer);
        _topPanel.AddToClassList(_ussCommonTextPrimaryBlack);
        Add(_topPanel);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignContent = Align.Center;

        _questRankElement = new QuestRankElement(_quest);
        _questRankElement.UpdateElementalElement(_questInfoBuilding.UpgradeRank);
        container.Add(_questRankElement);

        _topPanel.Add(container);
    }

    void AddBottomPanel()
    {
        _bottomPanel = new();
        _bottomPanel.style.alignItems = Align.Center;
        _bottomPanel.AddToClassList(_ussCommonTextPrimaryBlack);
        Add(_bottomPanel);

        _assignedCharactersContainer = CreateCharacterSlots();
        _bottomPanel.Add(_assignedCharactersContainer);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _bottomPanel.Add(container);

        _durationLabel = new TextWithTooltip($"Duration: {_quest.DurationSeconds}s.", $"When delegated it will take {_quest.DurationSeconds}s");
        container.Add(_durationLabel);

        _successChanceLabel = new TextWithTooltip($"", "The stronger/more the characters the higher success chance.");
        UpdateSuccessChanceLabel();
        container.Add(_successChanceLabel);

        _actionButton = CreateActionButton();
        UpdateActionButton();
        _bottomPanel.Add(_actionButton);
    }

    void UpdateSuccessChanceLabel()
    {
        if (_questInfoBuilding.UpgradeRank == 0)
            _successChanceLabel.UpdateText($"Success chance: ??.");

        int percent = _quest.GetSuccessChance();
        if (_questInfoBuilding.UpgradeRank == 1 || _questInfoBuilding.UpgradeRank == 2)
        {
            if (percent <= 50)
                _successChanceLabel.UpdateText($"Success chance: unlikely.");
            if (percent > 50)
                _successChanceLabel.UpdateText($"Success chance: likely.");
        }

        if (_questInfoBuilding.UpgradeRank == 3)
            _successChanceLabel.UpdateText($"Success chance: {percent}%.");
    }

    /* CHARACTER SLOT MANAGEMENT */
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

    void ReturnAssignedCharacters()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
        {
            if (slot.Card == null)
                continue;
            slot.Card.Character.UpdateDeskPosition(new Vector2(slot.worldBound.x, slot.worldBound.y));
            _deskManager.SpitCharacterOntoDesk(slot.Card.Character);
            slot.RemoveCard();
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
        UpdateActionButton();
    }

    /* ACTION BUTTON */
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

        if (_quest.QuestState == QuestState.Expired || _quest.QuestState == QuestState.RewardCollected)
        {
            _actionButton.style.visibility = Visibility.Hidden;
            return;
        }

        if (_quest.QuestState == QuestState.Finished)
        {
            HandleActionButton(SeeResults, "See Results!", _ussActionButtonFinished);
            return;
        }

        if (_quest.QuestState == QuestState.Delegated)
        {
            _actionButton.SetEnabled(false);
            _actionButton.UpdateButtonText("In progress...");
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

    void DelegateBattle() { _quest.DelegateQuest(); }


    /* QUEST STATES */
    void HandleDelegatedQuest()
    {
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

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

        Label text = new($"Expired! ({_quest.DateTimeExpired.Day})");
        text.AddToClassList(_ussCommonTextPrimary);
        text.style.fontSize = 32;
        text.transform.rotation *= Quaternion.Euler(0f, 0f, 30f);
        overlay.Add(text);
    }

    void HandleRewardCollected()
    {
        _durationLabel.Clear();
        _successChanceLabel.Clear();
        _assignedCharactersContainer.style.display = DisplayStyle.None;

        if (_quest.Reward.Item == null)
            return;

        if (!_quest.IsWon)
            return;

        ItemElement el = new(_quest.Reward.Item);
        ItemSlot slot = new(el);
        slot.OnItemRemoved += OnRewardItemRemoved;
        slot.OnItemAdded += OnRewardItemAdded;

        _deskManager.GetComponent<DraggableItems>().AddSlot(slot);
        _deskManager.GetComponent<DraggableItems>().AddDraggableItem(el);

        _bottomPanel.Add(slot);
        slot.BringToFront();
    }

    void OnRewardItemRemoved(ItemElement el) { _quest.Reward.Item = null; }

    void OnRewardItemAdded(ItemElement el) { _quest.Reward.Item = el.Item; }

    void SeeResults()
    {
        QuestResultElement el = new QuestResultElement(_deskManager.Root, _report);
        el.OnHide += OnResultsClosed;
    }

    void OnResultsClosed()
    {
        _quest.UpdateQuestState(QuestState.RewardCollected);
        ReturnAssignedCharacters();
    }
}
