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
    DraggableItems _draggableItems;

    public VisualElement Root { get; private set; }
    VisualElement _mainDesk;
    VisualElement _reportsContainer;
    VisualElement _reportsArchive;

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

        Root = _dashboardManager.Root;

        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _reportsContainer = Root.Q<VisualElement>("reportsContainer");
        _reportsArchive = Root.Q<VisualElement>("reportsArchive");
        _reportsArchive.RegisterCallback<PointerUpEvent>(OnArchiveClick);

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

        _reportsContainer.Clear();
        _characterCardSlots = new();
        VisibleReports = new();

        foreach (Report report in _gameManager.Reports)
            await CreateReport(report);

        foreach (Character character in _gameManager.PlayerTroops)
        {
            if (character.IsAssigned)
                continue;

            AddCharacterToDesk(character);
        }

        foreach (Item item in _gameManager.PlayerItemPouch)
            AddItemToDesk(item);
    }

    ItemElement AddItemToDesk(Item item)
    {
        ItemElement el = new(item);
        el.style.position = Position.Absolute;
        el.style.left = item.DeskPosition.x;
        el.style.top = item.DeskPosition.y;

        _draggableItems.AddDraggableItem(el);

        _reportsContainer.Add(el);

        return el;
    }

    public void RegisterDeskCard(CharacterCardMini card)
    {
        card.RegisterCallback<PointerUpEvent>(OnMiniCardPointerUp);
    }

    CharacterCardMini AddCharacterToDesk(Character character)
    {
        CharacterCardMini card = new(character);
        card.style.position = Position.Absolute;
        _reportsContainer.Add(card);
        card.style.top = character.DeskPosition.y;
        card.style.left = character.DeskPosition.x;
        _draggableCharacters.AddDraggableCard(card);
        RegisterDeskCard(card);
        return card;
    }

    void OnMiniCardPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 1)
            return;

        CharacterCardMini card = (CharacterCardMini)evt.currentTarget;
        if (card.IsLocked)
            return;
        if (card.Character.IsAssigned)
            return;


        foreach (Item item in card.Character.Items)
        {
            item.DeskPosition = evt.position;
            SpitItemsOntoDesk(item);
        }

        card.Character.ClearItems();
    }

    public async void SpitItemsOntoDesk(Item item)
    {
        ItemElement el = AddItemToDesk(item);
        float newX = item.DeskPosition.x + Random.Range(-100, 100);
        float newY = item.DeskPosition.y + Random.Range(-100, 100);

        Vector3 endPosition = new(newX, newY, 0);
        await MoveElementOnArc(el, item.DeskPosition, endPosition);
        item.UpdateDeskPosition(endPosition);
    }

    public async void SpitCharacterOntoDesk(Character character)
    {
        CharacterCardMini card = AddCharacterToDesk(character);
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
        DOTween.To(x => el.transform.scale = x * Vector3.one, 0, 1, 0.5f);

        await Task.Delay(100);
    }

    async void OnReportDismissed(ReportElement element)
    {
        DOTween.To(x => element.transform.scale = x * Vector3.one, 1, 0.1f, 1f);
        await MoveReportToArchive(element, _reportsArchive);

        if (element.parent == _reportsContainer)
            _reportsContainer.Remove(element);
    }

    async Task MoveReportToArchive(VisualElement element, VisualElement destinationElement)
    {
        Vector2 start = new(element.style.left.value.value, element.style.top.value.value);
        // TODO: i'd like it to fly to reports archive but dunno how to do it.
        Vector2 destination = new(Screen.width - element.layout.width, -100);

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
    void OnArchiveClick(PointerUpEvent evt)
    {
        FullScreenElement visual = new FullScreenElement();
        visual.AddToClassList("textPrimary");
        visual.style.backgroundColor = Color.black;
        visual.style.left = Screen.width;

        ScrollView container = new();
        visual.Add(container);

        DOTween.To(x => visual.style.left = x, Screen.width, 0f, 1f);

        foreach (Report report in _gameManager.ReportsArchived)
        {
            Label r = new Label($"{report.ReportType}");
            container.Add(r);
            // https://forum.unity.com/threads/send-additional-parameters-to-callback.777029/
            r.RegisterCallback<PointerUpEvent, Report>(OnArchivedReportClick, report);
        }
        visual.Initialize(Root);
        visual.AddBackButton();
    }

    void OnArchivedReportClick(PointerUpEvent evt, Report report)
    {
        FullScreenElement visual = new FullScreenElement();
        visual.style.backgroundColor = Color.black;
        visual.Add(new ReportElement(visual, report));
        visual.Initialize(Root);
        visual.AddBackButton();
    }

    void CleanDraggables() { _draggableCharacters.RemoveDragContainer(); }
}
