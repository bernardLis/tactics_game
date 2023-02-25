using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Random = UnityEngine.Random;

public class DeskManager : Singleton<DeskManager>
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    DraggableCharacters _draggableCharacters;
    DraggableAbilities _draggableAbilities;
    DraggableItems _draggableItems;

    public VisualElement Root { get; private set; }
    VisualElement _mainDesk;
    VisualElement _reportContainer;
    VisualElement _summonCharactersButton;

    VisualElement _slotsContainer;
    VisualElement _itemSlotsContainer;
    VisualElement _abilitySlotsContainer;

    List<ItemSlot> _deskItemSlots = new();
    List<AbilitySlot> _deskAbilitySlots = new();

    List<CharacterCardMini> _characterCardsMini = new();
    List<CharacterCard> _characterCards = new();

    List<CharacterCardMiniSlot> _characterCardSlots = new();

    List<ReportElement> _reportElements = new();
    List<Report> _visibleReports = new();

    const string _ussCardMini = "character-card-mini__main";

    public event Action OnDeskInitialized;
    protected override void Awake() { base.Awake(); }
    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnReportAdded += OnReportAdded;
        _gameManager.OnCharacterRemovedFromTroops += OnCharacterRemovedFromTroops;

        _dashboardManager = GetComponent<DashboardManager>();
        _draggableCharacters = GetComponent<DraggableCharacters>();
        _draggableItems = GetComponent<DraggableItems>();
        _draggableAbilities = GetComponent<DraggableAbilities>();

        Root = _dashboardManager.Root;

        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _reportContainer = Root.Q<VisualElement>("reportContainer");

        _summonCharactersButton = Root.Q<VisualElement>("summonCharacters");
        _summonCharactersButton.RegisterCallback<PointerUpEvent>(SummonCharacters);

        _slotsContainer = Root.Q<VisualElement>("slotsContainer");
        _itemSlotsContainer = Root.Q<VisualElement>("itemSlotsContainer");
        _abilitySlotsContainer = Root.Q<VisualElement>("abilitySlotsContainer");

        Initialize();
    }

    void SummonCharacters(PointerUpEvent evt)
    {
        List<VisualElement> cards = Root.Query(className: _ussCardMini).ToList();
        foreach (VisualElement item in cards)
        {
            CharacterCardMini card = (CharacterCardMini)item;
            if (card.Character.IsAssigned)
                continue;

            item.style.left = _summonCharactersButton.style.left;
            item.style.top = _summonCharactersButton.style.top;
        }
    }

    public List<CharacterCardMini> GetAllCharacterCardsMini() { return _characterCardsMini; }

    void OnReportAdded(Report report)
    {
        if (_visibleReports.Contains(report))
            return;
        CreateReport(report);
    }

    public void HideAllReports()
    {
        foreach (ReportElement r in _reportElements)
            r.style.visibility = Visibility.Hidden;
    }

    public void ShowAllReports()
    {
        foreach (ReportElement r in _reportElements)
            r.style.visibility = Visibility.Visible;
    }


    void OnCharacterRemovedFromTroops(Character character)
    {
        for (int i = _characterCardsMini.Count - 1; i >= 0; i--)
        {
            if (_characterCardsMini[i].Character == character)
            {
                _reportContainer.Remove(_characterCardsMini[i]);
                _characterCardsMini.Remove(_characterCardsMini[i]);
                return;
            }
        }
    }

    void Initialize()
    {
        _draggableCharacters.Initialize(Root, _reportContainer);
        _draggableItems.Initialize(Root, _reportContainer);
        _draggableAbilities.Initialize(Root, _reportContainer);

        _characterCardSlots = new();
        _visibleReports = new();

        foreach (Report report in _gameManager.Reports)
            CreateReport(report);

        PopulateItemSlots();
        PopulateAbilitySlots();

        foreach (Character character in _gameManager.GetAllCharacters())
        {
            if (character.IsAssigned)
                continue;

            AddMiniCardToDesk(character);
        }

        Debug.Log($"Desk initialized...");
        OnDeskInitialized?.Invoke();
    }

    void PopulateItemSlots()
    {
        _itemSlotsContainer.Clear();
        foreach (ItemSlot slot in _deskItemSlots)
            _draggableItems.RemoveSlot(slot);
        _deskItemSlots = new();

        for (int i = 0; i < 5; i++) // TODO: magic 5
        {
            ItemSlot slot = new ItemSlot();
            _itemSlotsContainer.Add(slot);
            _deskItemSlots.Add(slot);
            _draggableItems.AddSlot(slot);
            slot.OnItemAdded += AddItemToPouch;
            slot.OnItemRemoved += RemoveItemFromPouch;
        }

        for (int i = 0; i < _gameManager.PlayerItemPouch.Count; i++)
        {
            if (i == 5) // TODO: magic 5
                return;
            ItemElement itemElement = new(_gameManager.PlayerItemPouch[i]);
            _deskItemSlots[i].AddItemNoDelegates(itemElement);
            _draggableItems.AddDraggableItem(itemElement);
        }
    }

    void AddItemToPouch(ItemElement element) { _gameManager.AddItemToPouch(element.Item); }
    void RemoveItemFromPouch(ItemElement element) { _gameManager.RemoveItemFromPouch(element.Item); }

    void PopulateAbilitySlots()
    {
        _abilitySlotsContainer.Clear();
        foreach (AbilitySlot slot in _deskAbilitySlots)
            _draggableAbilities.RemoveSlot(slot);
        _deskAbilitySlots = new();

        for (int i = 0; i < 5; i++) // TODO: magic 5
        {
            AbilitySlot slot = new AbilitySlot();
            _abilitySlotsContainer.Add(slot);
            _deskAbilitySlots.Add(slot);
            _draggableAbilities.AddSlot(slot);
            slot.OnAbilityAdded += AddAbilityToPouch;
            slot.OnAbilityRemoved += RemoveAbilityFromPouch;
        }

        for (int i = 0; i < _gameManager.PlayerAbilityPouch.Count; i++)
        {
            if (i == 5) // TODO: magic 5
                return;
            _deskAbilitySlots[i].AddDraggableButtonNoDelegates(_gameManager.PlayerAbilityPouch[i], _draggableAbilities);
        }
    }

    public void AddAbilityToEmptySlot(AbilityButton b)
    {
        foreach (AbilitySlot slot in _deskAbilitySlots)
        {
            if (slot.AbilityButton == null)
            {
                slot.AddDraggableButton(b.Ability, _draggableAbilities);
                return;
            }
        }
    }

    public void AddItemToEmptySlot(ItemElement e)
    {
        foreach (ItemSlot slot in _deskItemSlots)
        {
            if (slot.ItemElement == null)
            {
                slot.AddItemNoDelegates(e);
                _draggableItems.AddDraggableItem(e);
                return;
            }
        }
    }

    void AddAbilityToPouch(Ability ability) { _gameManager.AddAbilityToPouch(ability); }

    void RemoveAbilityFromPouch(Ability ability) { _gameManager.RemoveAbilityFromPouch(ability); }

    /* CHARACTERS */
    CharacterCardMini AddMiniCardToDesk(Character character)
    {
        CharacterCardMini card = new(character);
        card.style.position = Position.Absolute;
        card.style.left = character.DeskPosition.x;
        card.style.top = character.DeskPosition.y;

        _reportContainer.Add(card);
        _characterCardsMini.Add(card);
        RegisterCardMiniDrag(card);
        return card;
    }

    public void RegisterCardMiniDrag(CharacterCardMini card)
    {
        card.RegisterCallback<PointerUpEvent>(OnMiniCardPointerUp);
        _draggableCharacters.AddDraggableCard(card);
    }

    void OnMiniCardPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 1)
            return;
        if (_draggableCharacters.IsDragging)
            return;

        CharacterCardMini card = (CharacterCardMini)evt.currentTarget;
        if (card.IsLocked)
            return;
        if (card.Character.IsAssigned)
            return;

        CharacterCard bigCard = new(card.Character);
        bigCard.RegisterCallback<PointerUpEvent>(OnBigCardPointerUp);
        bigCard.PortraitVisualElement.RegisterCallback<PointerDownEvent>(OnPortraitPointerDown);
        bigCard.style.left = evt.position.x - 100;
        bigCard.style.top = evt.position.y - 100;
        _reportContainer.Add(bigCard);
        _characterCards.Add(bigCard);

        _draggableItems.AddCharacterCard(bigCard);
        _draggableAbilities.AddCharacterCard(bigCard);

        _characterCardsMini.Remove(card);
        card.parent.Remove(card);
    }

    void OnPortraitPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        CharacterPortraitElement portraitElement = (CharacterPortraitElement)evt.currentTarget;
        _draggableItems.RemoveCharacterCard(portraitElement.Card);
        _draggableAbilities.RemoveCharacterCard(portraitElement.Card);

        CharacterCardMini card = AddMiniCardToDesk(portraitElement.Character);
        card.BlockTooltip();
        _draggableCharacters.StartCardDrag(evt.position, null, card);

        portraitElement.Card.parent.Remove(portraitElement.Card);
    }

    void OnBigCardPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 1)
            return;

        CharacterCard card = (CharacterCard)evt.currentTarget;
        if (card.Character.IsAssigned)
            return;

        _draggableItems.RemoveCharacterCard(card);
        _draggableAbilities.RemoveCharacterCard(card);
        AddMiniCardToDesk(card.Character);
        card.parent.Remove(card);
    }

    public void SpitCharacterOntoDesk(Character character)
    {
        CharacterCardMini card = AddMiniCardToDesk(character);
        float newX = card.Character.DeskPosition.x + Random.Range(-200, 200);
        float newY = card.Character.DeskPosition.y + Random.Range(-300, 300);

        Vector3 endPosition = new(newX, newY, 0);
        StartCoroutine(MoveElementOnArc(card, card.Character.DeskPosition, endPosition));
        card.Character.UpdateDeskPosition(endPosition);

        card.Character.RaiseCheck();
    }

    IEnumerator MoveElementOnArc(VisualElement el, Vector3 startPosition, Vector3 endPosition)
    {
        el.style.visibility = Visibility.Visible;

        Vector3 p0 = startPosition;
        Vector3 p2 = endPosition;

        float newX = startPosition.x + (endPosition.x - startPosition.x) * 0.5f;
        float newY = startPosition.y - 400f;
        Vector3 p1 = new Vector3(newX, newY);

        float percent = 0;
        while (percent < 1)
        {
            Vector3 r = DOCurve.CubicBezier.GetPointOnSegment(p0, p1, p2, p1, percent);
            el.style.left = r.x;
            el.style.top = r.y;

            percent += 0.01f;
            yield return null;
        }
    }

    /* REPORTS */
    void CreateReport(Report report)
    {
        _visibleReports.Add(report);
        ReportElement el = null;
        // depending on type it will look differently
        if (report.ReportType == ReportType.Quest)
            el = (QuestReportElement)new(_reportContainer, report) as QuestReportElement;
        if (report.ReportType == ReportType.Recruit)
            el = (RecruitReportElement)new(_reportContainer, report) as RecruitReportElement;
        if (report.ReportType == ReportType.Text)
            el = (TextReportElement)new(_reportContainer, report) as TextReportElement;
        if (report.ReportType == ReportType.CampBuilding)
            el = (CampReportElement)new(_reportContainer, report) as CampReportElement;
        if (report.ReportType == ReportType.Shop)
            el = (ShopReportElement)new(_reportContainer, report) as ShopReportElement;
        if (report.ReportType == ReportType.Pawnshop)
            el = (PawnshopReportElement)new(_reportContainer, report) as PawnshopReportElement;
        if (report.ReportType == ReportType.Ability)
            el = (AbilityReportElement)new(_reportContainer, report) as AbilityReportElement;
        if (report.ReportType == ReportType.Item)
            el = (ItemReportElement)new(_reportContainer, report) as ItemReportElement;
        if (report.ReportType == ReportType.SpiceRecycle)
            el = (SpiceRecycleReportElement)new(_reportContainer, report) as SpiceRecycleReportElement;
        if (report.ReportType == ReportType.Wages)
            el = (WagesReportElement)new(_reportContainer, report) as WagesReportElement;
        if (report.ReportType == ReportType.RaiseRequest)
            el = (RaiseRequestReportElement)new(_reportContainer, report) as RaiseRequestReportElement;

        _reportElements.Add(el);
        el.style.position = Position.Absolute;
        el.OnReportDismissed += OnReportDismissed;
        _reportContainer.Add(el);

        if (report.Position != Vector2.zero)
        {
            el.style.left = report.Position.x;
            el.style.top = report.Position.y;
            return;
        }
        AnimateReport(el);
    }

    void AnimateReport(VisualElement el)
    {
        int leftPx = Mathf.FloorToInt(0.25f * Screen.width) + Random.Range(-30, 31);

        el.style.left = -1000;
        el.style.top = 0 + Random.Range(-20, 20);

        DOTween.To(() => el.style.left.value.value, x => el.style.left = x, leftPx, Random.Range(0.5f, 0.7f))
                .SetEase(Ease.InOutCubic);
    }

    void OnReportDismissed(ReportElement element)
    {
        // clearing transition
        element.style.transitionProperty = new List<StylePropertyName>() { new StylePropertyName("none") };
        DOTween.To(x => element.transform.scale = x * Vector3.one, 1, 0.1f, 1f);
        StartCoroutine(MoveReportToArchive(element));
    }

    IEnumerator MoveReportToArchive(VisualElement element)
    {
        Vector2 start = new(element.style.left.value.value, element.style.top.value.value);
        Vector2 destination = new(Screen.width + 100, -100);
        float percent = 0;
        while (percent < 1)
        {
            Vector3 result = Vector3.Lerp(start, destination, percent); // lerp between the 2 for the result
            element.style.left = result.x;
            element.style.top = result.y;

            percent += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        if (element.parent == _reportContainer)
            _reportContainer.Remove(element);

    }
}
