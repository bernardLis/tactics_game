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

    public VisualElement Root { get; private set; }
    VisualElement _mainDesk;
    Label _daySummaryDayLabel;
    VisualElement _reportsContainer;
    VisualElement _reportsArchive;

    //  VisualElement _deskTroopsContainer;
    List<CharacterCardMiniSlot> _characterCardSlots = new();

    MyButton _passDayButton;

    List<Report> VisibleReports = new();

    protected override void Awake() { base.Awake(); }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += DayPassed;
        _gameManager.OnCharacterAddedToTroops += AddCharacterToDesk; //AddCharacterToDraggableTroops;
        //_gameManager.OnTroopsLimitChanged += OnTroopsLimitChanged;

        _dashboardManager = GetComponent<DashboardManager>();
        _draggableCharacters = GetComponent<DraggableCharacters>();

        Root = _dashboardManager.Root;

        _mainDesk = Root.Q<VisualElement>("mainDesk");
        _reportsContainer = Root.Q<VisualElement>("reportsContainer");
        _reportsArchive = Root.Q<VisualElement>("reportsArchive");
        _reportsArchive.RegisterCallback<PointerUpEvent>(OnArchiveClick);

        _dashboardManager.OnDeskOpened += Initialize;
        _dashboardManager.OnHideAllPanels += HidePassDay;
        _dashboardManager.OnHideAllPanels += CleanDraggables;

        _passDayButton = new("Pass Day", "menuButton", PassDay);
        _passDayButton.style.opacity = 0;
        _passDayButton.style.display = DisplayStyle.None;

        // _deskTroopsContainer = Root.Q<VisualElement>("deskTroopsContainer");

        Initialize();
    }

    async void DayPassed(int day)
    {
        await Task.Delay(100); // TODO: ugh...I need to make sure other scripts manage their things...
        foreach (Report report in _gameManager.Reports)
        {
            if (VisibleReports.Contains(report))
                continue;
            await CreateReport(report);
        }
    }

    async void Initialize()
    {
        _reportsContainer.Clear();
        _reportsContainer.Add(_passDayButton);
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

        _draggableCharacters.Initialize(Root, _reportsContainer);
        ShowPassDayButton();
    }

    public void AddCharacterToDesk(Character character)
    {
        CharacterCardMini card = new(character);
        card.style.position = Position.Absolute;
        _reportsContainer.Add(card);

        card.style.top = character.DeskPosition.y;
        card.style.left = character.DeskPosition.x;

        _draggableCharacters.AddDraggableCard(card);
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

    void ShowPassDayButton()
    {
        _passDayButton.style.display = DisplayStyle.Flex;
        DOTween.To(() => _passDayButton.style.opacity.value, x => _passDayButton.style.opacity = x, 1f, 1f);
    }

    void HidePassDay()
    {
        _passDayButton.style.display = DisplayStyle.None;
        _passDayButton.style.opacity = 0;
    }

    void PassDay() { _gameManager.PassDay(); }

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
