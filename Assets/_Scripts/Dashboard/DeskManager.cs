using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class DeskManager : Singleton<DeskManager>
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    DraggableCharacters _draggableCharacters;
    DraggableAbilities _draggableAbilities;
    DraggableItems _draggableItems;

    public VisualElement Root { get; private set; }
    VisualElement _mainDesk;
    VisualElement _reportsContainer;
    VisualElement _slotsContainer;
    VisualElement _itemSlotsContainer;
    VisualElement _abilitySlotsContainer;

    List<ItemSlot> _deskItemSlots = new();
    List<AbilitySlot> _deskAbilitySlots = new();

    List<CharacterCardMiniSlot> _characterCardSlots = new();

    List<Report> VisibleReports = new();

    protected override void Awake() { base.Awake(); }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnReportAdded += OnReportAdded;

        _dashboardManager = GetComponent<DashboardManager>();
        _draggableCharacters = GetComponent<DraggableCharacters>();
        _draggableItems = GetComponent<DraggableItems>();
        _draggableAbilities = GetComponent<DraggableAbilities>();

        Root = _dashboardManager.Root;

        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _reportsContainer = Root.Q<VisualElement>("reportsContainer");

        _slotsContainer = Root.Q<VisualElement>("slotsContainer");
        _itemSlotsContainer = Root.Q<VisualElement>("itemSlotsContainer");
        _abilitySlotsContainer = Root.Q<VisualElement>("abilitySlotsContainer");

        _dashboardManager.OnDeskOpened += Initialize;
        _dashboardManager.OnHideAllPanels += CleanDraggables;

        Initialize();
    }

    async void OnReportAdded(Report report)
    {
        if (VisibleReports.Contains(report))
            return;
        await CreateReport(report);
    }

    async void Initialize()
    {
        _draggableCharacters.Initialize(Root, _reportsContainer);
        _draggableItems.Initialize(Root, _reportsContainer);
        _draggableAbilities.Initialize(Root, _reportsContainer);

        _reportsContainer.Clear();

        _characterCardSlots = new();
        VisibleReports = new();

        foreach (Report report in _gameManager.Reports)
            await CreateReport(report);

        PopulateItemSlots();
        PopulateAbilitySlots();

        foreach (Character character in _gameManager.PlayerTroops)
        {
            if (character.IsAssigned)
                continue;

            AddMiniCardToDesk(character);
        }
    }

    void CleanDraggables()
    {
        _draggableCharacters.RemoveDragContainer();
        _draggableItems.RemoveDragContainer();
        _draggableAbilities.RemoveDragContainer();
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

    void AddAbilityToPouch(Ability ability) { _gameManager.AddAbilityToPouch(ability); }

    void RemoveAbilityFromPouch(Ability ability) { _gameManager.RemoveAbilityFromPouch(ability); }

    /* CHARACTERS */
    CharacterCardMini AddMiniCardToDesk(Character character)
    {
        CharacterCardMini card = new(character);
        card.style.position = Position.Absolute;
        card.style.left = character.DeskPosition.x;
        card.style.top = character.DeskPosition.y;

        _reportsContainer.Add(card);
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
        _reportsContainer.Add(bigCard);

        _draggableItems.AddCharacterCard(bigCard);
        _draggableAbilities.AddCharacterCard(bigCard);

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

    public async void SpitCharacterOntoDesk(Character character)
    {
        CharacterCardMini card = AddMiniCardToDesk(character);
        float newX = card.Character.DeskPosition.x + Random.Range(-200, 200);
        float newY = card.Character.DeskPosition.y + Random.Range(-300, 300);

        Vector3 endPosition = new(newX, newY, 0);
        await MoveElementOnArc(card, card.Character.DeskPosition, endPosition);
        card.Character.UpdateDeskPosition(endPosition);
    }

    async Task MoveElementOnArc(VisualElement el, Vector3 startPosition, Vector3 endPosition)
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
            await Task.Delay(5);
        }
    }

    /* REPORTS */
    async Task CreateReport(Report report)
    {
        VisibleReports.Add(report);
        ReportElement el = null;
        // depending on type it will look differently
        if (report.ReportType == ReportType.Quest)
            el = (QuestReportElement)new(_reportsContainer, report) as QuestReportElement;
        if (report.ReportType == ReportType.Recruit)
            el = (RecruitReportElement)new(_reportsContainer, report) as RecruitReportElement;
        if (report.ReportType == ReportType.Text)
            el = (TextReportElement)new(_reportsContainer, report) as TextReportElement;
        if (report.ReportType == ReportType.CampBuilding)
            el = (CampReportElement)new(_reportsContainer, report) as CampReportElement;
        if (report.ReportType == ReportType.Shop)
            el = (ShopReportElement)new(_reportsContainer, report) as ShopReportElement;
        if (report.ReportType == ReportType.Pawnshop)
            el = (PawnshopReportElement)new(_reportsContainer, report) as PawnshopReportElement;
        if (report.ReportType == ReportType.Ability)
            el = (AbilityReportElement)new(_reportsContainer, report) as AbilityReportElement;

        el.style.position = Position.Absolute;
        el.OnReportDismissed += OnReportDismissed;
        _reportsContainer.Add(el);

        if (report.Position != Vector2.zero)
        {
            el.style.left = report.Position.x;
            el.style.top = report.Position.y;
            return;
        }
        await AnimateReport(el);
    }

    async Task AnimateReport(VisualElement el)
    {
        int leftPx = Mathf.FloorToInt(0.25f * Screen.width) + Random.Range(-10, 11);

        el.style.left = -1000;
        el.style.top = 0 + Random.Range(-10, 0);

        DOTween.To(() => el.style.left.value.value, x => el.style.left = x, leftPx, Random.Range(0.5f, 0.7f)).SetEase(Ease.InOutCubic);

        await Task.Delay(100);
    }

    async void OnReportDismissed(ReportElement element)
    {
        // clearing transition
        element.style.transitionProperty = new List<StylePropertyName>() { new StylePropertyName("none") };
        DOTween.To(x => element.transform.scale = x * Vector3.one, 1, 0.1f, 1f);
        await MoveReportToArchive(element);

        if (element.parent == _reportsContainer)
            _reportsContainer.Remove(element);
    }

    async Task MoveReportToArchive(VisualElement element)
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
            await Task.Delay(5);
        }
    }
}
