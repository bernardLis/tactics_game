using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestVisualElement : VisualElement
{
    GameManager _gameManager;
    Quest _quest;

    VisualElement _additionalInfo;
    VisualElement _characterSlotContainer;
    MyButton _startAssignementButton;

    List<CharacterCardMiniSlot> _cardSlots = new();

    Label _successChance;

    bool _isAdditionalInfoShown;

    public QuestVisualElement(Quest quest)
    {
        _gameManager = GameManager.Instance;
        _quest = quest;
        AddToClassList("questElement");
        AddToClassList("textPrimary");

        // basics
        VisualElement basicInfoContainer = new();
        basicInfoContainer.AddToClassList("questBasicInfoContainer");
        Add(basicInfoContainer);

        Label icon = new();
        icon.style.backgroundImage = new StyleBackground(quest.Icon);
        icon.style.width = 50;
        icon.style.height = 50;
        basicInfoContainer.Add(icon);

        Label title = new(quest.Title);
        title.AddToClassList("textPrimary");
        basicInfoContainer.Add(title);

        CreateAdditionalInfo();
        CreateCharacterSlots();

        _successChance = new();
        Add(_successChance);
        _successChance.style.display = DisplayStyle.None;

        _startAssignementButton = new("Battle it out!", "menuButton", StartBattle);
        _characterSlotContainer.Add(_startAssignementButton);
        _startAssignementButton.style.display = DisplayStyle.None;

        RegisterCallback<PointerUpEvent>(OnPointerUp);
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

        // map info
        VisualElement mapInfoContainer = new();
        _additionalInfo.Add(mapInfoContainer);

        Label biome = new($"Biome: {_quest.Biome.name}");
        mapInfoContainer.Add(biome);

        Label variant = new($"Variant: {_quest.MapVariant.name}");
        mapInfoContainer.Add(variant);

        Label mapSize = new($"Map size: {_quest.MapSize.x} x {_quest.MapSize.y}");
        mapInfoContainer.Add(mapSize);

        VisualElement enemyIconContainer = new VisualElement();
        enemyIconContainer.style.flexDirection = FlexDirection.Row;
        mapInfoContainer.Add(enemyIconContainer);

        foreach (var e in _quest.Enemies)
        {
            Label l = new Label();
            l.style.backgroundImage = e.BrainIcon.texture;
            l.style.width = 32;
            l.style.height = 32;
            enemyIconContainer.Add(l);
        }

        // reward
        VisualElement rewardContainer = new();
        rewardContainer.style.flexDirection = FlexDirection.Row;
        rewardContainer.style.alignContent = Align.Center;
        _additionalInfo.Add(rewardContainer);

        if (_quest.Reward.Gold != 0)
            rewardContainer.Add(new GoldElement(_quest.Reward.Gold));
        if (_quest.Reward.Item != null)
            rewardContainer.Add(new ItemSlotVisual(new ItemVisual(_quest.Reward.Item)));

        Add(_additionalInfo);
        _additionalInfo.style.display = DisplayStyle.None;
    }

    void CreateCharacterSlots()
    {
        _characterSlotContainer = new();
        _characterSlotContainer.style.flexDirection = FlexDirection.Row;
        for (int i = 0; i < 3; i++)
        {
            CharacterCardMiniSlot slot = new CharacterCardMiniSlot();
            _cardSlots.Add(slot);
            slot.OnCardAdded += OnCardAdded;
            slot.OnCardRemoved += OnCardRemoved;
            _characterSlotContainer.Add(slot);
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
        _startAssignementButton.ChangeCallback(null);

        if (CountAssignedCharacters() == 0)
        {
            Debug.Log($"0 characters");
            _successChance.style.display = DisplayStyle.None;
            _startAssignementButton.style.display = DisplayStyle.None;
            return;
        }

        _startAssignementButton.style.display = DisplayStyle.Flex;
        _successChance.style.display = DisplayStyle.Flex;

        if (IsPlayerAssigned())
        {
            Debug.Log($"player assigned");
            _successChance.text = "It takes 1 day.";
            _startAssignementButton.ChangeCallback(StartBattle);
            _startAssignementButton.text = "Battle It Out!";
            return;
        }

        Debug.Log($"no player");
        _successChance.text = $"Success chance: {CountAssignedCharacters() * 25}%. Duration: {_quest.Duration} ";
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

    void StartBattle()
    {
        Debug.Log($"lets go to battle!");
        _gameManager.StartBattle(_quest);
    }

    void DelegateBattle()
    {
        _quest.DelegateQuest();
    }

}
