using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class DeskManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    DraggableCharacters _draggableCharacters;

    VisualElement _root;
    VisualElement _mainDesk;
    Label _daySummaryDayLabel;
    VisualElement _reportsContainer;
    VisualElement _reportsArchive;

    VisualElement _deskTroopsContainer;

    MyButton _passDayButton;


    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += DayPassed;

        _dashboardManager = GetComponent<DashboardManager>();
        _draggableCharacters = GetComponent<DraggableCharacters>();

        _root = _dashboardManager.Root;

        _mainDesk = _root.Q<VisualElement>("mainDesk");
        _reportsContainer = _root.Q<VisualElement>("reportsContainer");
        _reportsArchive = _root.Q<VisualElement>("reportsArchive");
        _reportsArchive.RegisterCallback<PointerUpEvent>(OnArchiveClick);

        _dashboardManager.OnDeskClicked += Initialize;
        _dashboardManager.OnHideAllPanels += HidePassDay;
        _dashboardManager.OnHideAllPanels += CleanDraggables;

        _passDayButton = new("Pass Day", "menuButton", PassDay);
        _passDayButton.style.opacity = 0;
        _passDayButton.style.display = DisplayStyle.None;

        _deskTroopsContainer = _root.Q<VisualElement>("deskTroopsContainer");

        Initialize();
    }

    async void Initialize()
    {
        _reportsContainer.Clear();
        _reportsContainer.Add(_passDayButton);
        _deskTroopsContainer.Clear();

        float left = 0.25f;
        int top = 0;

        foreach (Report report in _gameManager.Reports)
        {
            ReportVisualElement el = new(_reportsContainer, report);
            el.OnReportDismissed += OnReportDimissed;
            _reportsContainer.Add(el);

            await AnimateReport(el, left, top);

            left -= 0.01f;
            top -= 10;
        }

        foreach (Character character in _gameManager.PlayerTroops)
            if (!character.IsUnavailable)
                _deskTroopsContainer.Add(new CharacterCardMiniSlot(new CharacterCardMini(character)));

        _deskTroopsContainer.Add(new CharacterCardMiniSlot());
        _draggableCharacters.Initialize(_root);

        ShowPassDayButton();
    }

    async Task AnimateReport(VisualElement el, float left, int top)
    {
        int screenWidth = Screen.width;
        int leftPx = Mathf.FloorToInt(left * screenWidth) + Random.Range(-10, 11);

        el.style.left = -1000;
        el.style.top = top + Random.Range(-2, 3);

        DOTween.To(() => el.style.left.value.value, x => el.style.left = x, leftPx, Random.Range(0.5f, 0.7f)).SetEase(Ease.InOutCubic);
        DOTween.To(x => el.transform.scale = x * Vector3.one, 0, 1, 0.5f);

        await Task.Delay(100);
    }

    async void OnReportDimissed(ReportVisualElement element)
    {
        DOTween.To(x => element.transform.scale = x * Vector3.one, 1, 0.1f, 1f);
        // DOTween.To(x => element.transform.rotation = x, 0, 360, 1f);
        await MoveReportToArchive(element, _reportsArchive);

        _reportsContainer.Remove(element);
    }

    async Task MoveReportToArchive(VisualElement element, VisualElement destinationElement)
    {
        Vector2 start = new(element.style.left.value.value, element.style.top.value.value);
        // TODO: i'd like it to fly to reports archive but dunno how to do it.
        Vector2 destination = new(element.style.left.value.value + 1000, element.style.top.value.value + 200);

        float percent = 0;
        while (percent < 1)
        {
            Vector3 result = Vector3.Lerp(start, destination, percent); // lerp between the 2 for the result
            element.style.left = result.x;
            element.style.top = result.y;

            percent += 0.01f;
            await Task.Delay(10);
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

    void DayPassed(int day) { Initialize(); }

    void OnArchiveClick(PointerUpEvent evt)
    {
        FullScreenVisual visual = new FullScreenVisual();
        visual.AddToClassList("textPrimary");
        visual.style.backgroundColor = Color.black;
        visual.style.left = Screen.width;

        DOTween.To(x => visual.style.left = x, Screen.width, 0f, 1f);

        foreach (Report report in _gameManager.ReportsArchived)
        {
            Label r = new Label($"{report.ReportType}");
            visual.Add(r);
            // https://forum.unity.com/threads/send-additional-parameters-to-callback.777029/
            r.RegisterCallback<PointerUpEvent, Report>(OnArchivedReportClick, report);
        }
        visual.Initialize(_root);
        visual.AddBackButton();
    }

    void OnArchivedReportClick(PointerUpEvent evt, Report report)
    {
        FullScreenVisual visual = new FullScreenVisual();
        visual.style.backgroundColor = Color.black;
        visual.Add(new ReportVisualElement(visual, report));
        visual.Initialize(_root);
        visual.AddBackButton();
    }

    void CleanDraggables() { _draggableCharacters.RemoveDragContainer(); }

}
