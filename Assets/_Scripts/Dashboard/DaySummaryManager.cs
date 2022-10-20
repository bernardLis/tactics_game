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
        int marginLeftValue = 25; // percent
        int marginTopValue = 0; // pixels  yeah! 

        foreach (Report report in _gameManager.Reports)
        {
            // TODO: here I can move them to look better.
            ReportVisualElement el = new(_reportsContainer, report);

            el.style.marginLeft = Length.Percent(0);
            el.style.marginTop = marginTopValue;

            el.OnReportDismissed += OnReportDimissed;
            _reportsContainer.Add(el);

            // move from the elft quickly
            //            DOTween.To(() => el.style.marginLeft.value.value, x => el.style.marginLeft.value.value = x, 1f, 1f);

            marginLeftValue--;
            marginTopValue -= 10;

        }

        if (_gameManager.Reports.Count == 0)
            ShowPassDayButton();
    }

    void OnReportDimissed()
    {
        if (_reportsContainer.childCount > 0)
            return;

        ShowPassDayButton();
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
}
