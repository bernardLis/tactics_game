using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestVisualElement : VisualElement
{
    GameManager _gameManager;
    Quest _quest;

    VisualElement _basicInfoContainer;
    VisualElement _additionalInfo;
    VisualElement _characterSlotContainer;
    MyButton _startAssignementButton;

    List<CharacterCardMiniSlot> _cardSlots = new();

    Label _successChance;

    bool _isAdditionalInfoShown;

    public QuestVisualElement(Quest quest, bool isOnClickDisabled = false)
    {
        _gameManager = GameManager.Instance;
        _quest = quest;
        AddToClassList("questElement");
        AddToClassList("textPrimary");

        AddBasicInfo();

        CreateAdditionalInfo();
        CreateCharacterSlots();

        _successChance = new();
        Add(_successChance);
        _successChance.style.display = DisplayStyle.None;

        _startAssignementButton = new("Battle it out!", "menuButton", StartBattle);
        _characterSlotContainer.Add(_startAssignementButton);
        _startAssignementButton.style.display = DisplayStyle.None;

        if (isOnClickDisabled)
            return;

        RegisterCallback<PointerUpEvent>(OnPointerUp);
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

        if (!_quest.IsDelegated)
            return;

        AddDaysLeftLabel();
    }

    void AddDaysLeftLabel()
    {
        int daysLeft = _quest.CountDaysLeft();
        string text = daysLeft == 0 ? $"Success chance: {_quest.GetSuccessChance()}%"
                                    : $"{daysLeft} days left. Success chance: {_quest.GetSuccessChance()}%";
        Label daysLeftLabel = new(text);
        _basicInfoContainer.Add(daysLeftLabel);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (_isAdditionalInfoShown)
        {
            _isAdditionalInfoShown = false;
            _additionalInfo.style.display = DisplayStyle.None;
            _characterSlotContainer.style.display = DisplayStyle.None;
            return;
        }

        _additionalInfo.style.display = DisplayStyle.Flex;
        _characterSlotContainer.style.display = DisplayStyle.Flex;
        _isAdditionalInfoShown = true;
    }

    void CreateAdditionalInfo()
    {
        _additionalInfo = new();
        _additionalInfo.AddToClassList("questBasicInfoContainer");
        Add(_additionalInfo);
        _additionalInfo.style.display = DisplayStyle.None;

        // map info
        VisualElement managementInfoContainer = new();
        _additionalInfo.Add(managementInfoContainer);

        managementInfoContainer.Add(new TextWithTooltip($"Expires in: {_quest.ExpiryDay - _gameManager.Day} days", "Has to be delegated or taken before it expiries."));

        Label duration = new($"If delegated it takes: {_quest.Duration} days");
        managementInfoContainer.Add(duration);

        VisualElement enemyIconContainer = new VisualElement();
        enemyIconContainer.style.flexDirection = FlexDirection.Row;
        managementInfoContainer.Add(enemyIconContainer);

        foreach (var e in _quest.Enemies)
        {
            Label l = new Label();
            l.style.backgroundImage = e.BrainIcon.texture;
            l.style.width = 32;
            l.style.height = 32;
            enemyIconContainer.Add(l);
        }

        if (_quest.Reward == null) // TODO: handle battle lost better cross out the reward or something
            return;

        // reward
        VisualElement rewardContainer = new();
        rewardContainer.style.flexDirection = FlexDirection.Row;
        rewardContainer.style.alignContent = Align.Center;
        _additionalInfo.Add(rewardContainer);

        if (_quest.Reward.Gold != 0)
            rewardContainer.Add(new GoldElement(_quest.Reward.Gold));
        if (_quest.Reward.Item != null)
            rewardContainer.Add(new ItemSlotVisual(new ItemVisual(_quest.Reward.Item)));

    }

    void CreateCharacterSlots()
    {
        _characterSlotContainer = new();
        _characterSlotContainer.style.flexDirection = FlexDirection.Row;
        _cardSlots = new();
        for (int i = 0; i < 3; i++)
        {
            CharacterCardMiniSlot slot = new CharacterCardMiniSlot(null, _quest.IsDelegated);
            _cardSlots.Add(slot);
            slot.OnCardAdded += OnCardAdded;
            slot.OnCardRemoved += OnCardRemoved;
            _characterSlotContainer.Add(slot);
        }

        for (int i = 0; i < _quest.AssignedCharacters.Count; i++)
        {
            CharacterCardMini card = new(_quest.AssignedCharacters[i]);
            _cardSlots[i].Add(card);
        }

        Add(_characterSlotContainer);
        _characterSlotContainer.style.display = DisplayStyle.None;

    }

    void OnCardAdded(CharacterCardMini card)
    {
        _quest.AssignCharacter(card.Character);
        UpdateVisual();
    }

    void OnCardRemoved(CharacterCardMini card)
    {
        _quest.RemoveAssignedCharacter(card.Character);
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (_startAssignementButton == null)
            return;

        _startAssignementButton.ChangeCallback(null);

        if (CountAssignedCharacters() == 0)
        {
            _successChance.style.display = DisplayStyle.None;
            _startAssignementButton.style.display = DisplayStyle.None;
            return;
        }

        _startAssignementButton.style.display = DisplayStyle.Flex;
        _successChance.style.display = DisplayStyle.Flex;

        if (IsPlayerAssigned())
        {
            _successChance.text = "It takes 1 day.";
            _startAssignementButton.ChangeCallback(StartBattle);
            _startAssignementButton.text = "Battle It Out!";
            return;
        }

        _successChance.text = $"Duration: {_quest.Duration} days. Success chance: {_quest.GetSuccessChance()}%. ";
        _startAssignementButton.ChangeCallback(DelegateBattle);
        _startAssignementButton.text = "Delegate It Out!";
    }

    int CountAssignedCharacters()
    {
        int assignedCharacters = 0;
        foreach (CharacterCardMiniSlot slot in _cardSlots)
            if (slot.Card != null)
                assignedCharacters++;

        return assignedCharacters;
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
        _quest.DelegateQuest();

        foreach (CharacterCardMiniSlot slot in _cardSlots)
            slot.Lock();

        _startAssignementButton.text = "Assigned!";
        _startAssignementButton.SetEnabled(false);

        AddDaysLeftLabel();
        Remove(_successChance);
    }
}
