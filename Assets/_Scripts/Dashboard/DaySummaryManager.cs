using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Threading.Tasks;

public class DaySummaryManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    VisualElement _root;
    VisualElement _mainDaySummary;
    Label _daySummaryDayLabel;
    VisualElement _reportsContainer;
    VisualElement _reportsArchive;

    MyButton _passDayButton;


    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += DayPassed;

        _dashboardManager = GetComponent<DashboardManager>();

        _root = _dashboardManager.Root;

        _mainDaySummary = _root.Q<VisualElement>("mainDaySummary");
        _daySummaryDayLabel = _root.Q<Label>("daySummaryDayLabel");
        _reportsContainer = _root.Q<VisualElement>("reportsContainer");
        _reportsArchive = _root.Q<VisualElement>("reportsArchive");
        _reportsArchive.RegisterCallback<PointerUpEvent>(OnArchiveClick);

        _dashboardManager.OnDaySummaryClicked += Initialize;
        _dashboardManager.OnHideAllPanels += HidePassDay;

        _passDayButton = new("Pass Day", "menuButton", PassDay);
        _passDayButton.style.opacity = 0;
        _passDayButton.style.display = DisplayStyle.None;
        _mainDaySummary.Add(_passDayButton);

        Initialize();
    }

    async void Initialize()
    {
        _daySummaryDayLabel.text = $"Day: {_gameManager.Day}";
        _daySummaryDayLabel.style.opacity = 0;
        _daySummaryDayLabel.style.fontSize = 76;
        DOTween.To(() => _daySummaryDayLabel.style.opacity.value, x => _daySummaryDayLabel.style.opacity = x, 1f, 1f);

        await Task.Delay(200);

        _reportsContainer.Clear();
        float left = 0.25f;
        int top = 0;
        int screenWidth = Screen.width;
        foreach (Report report in _gameManager.Reports)
        {
            ReportVisualElement el = new(_reportsContainer, report);
            el.OnReportDismissed += OnReportDimissed;
            _reportsContainer.Add(el);

            el.style.left = -1000;
            el.style.top = top + Random.Range(-2, 3);

            int leftPx = Mathf.FloorToInt(left * screenWidth) + Random.Range(-10, 11);
            DOTween.To(() => el.style.left.value.value, x => el.style.left = x, leftPx, Random.Range(0.5f, 0.7f)).SetEase(Ease.InOutCubic);

            left -= 0.01f;
            top -= 10;
            await Task.Delay(200);
        }

        ShowPassDayButton();
    }

    void OnReportDimissed()
    {
        Debug.Log("report dismissed");
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
        foreach (Report report in _gameManager.ReportsArchived)
        {
            Label r = new Label($"{report.ReportType}");
            visual.Add(r);
            // https://forum.unity.com/threads/send-additional-parameters-to-callback.777029/
            r.RegisterCallback<PointerUpEvent, Report>(OnArchivedReportClick, report);
        }
        visual.Initialize(_root);
    }

    void OnArchivedReportClick(PointerUpEvent evt, Report report)
    {
        FullScreenVisual visual = new FullScreenVisual();
        visual.Add(new ReportVisualElement(visual, report));
    }
}
