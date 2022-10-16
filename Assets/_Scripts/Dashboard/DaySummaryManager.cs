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
    VisualElement _daySummaryContent;

    VisualElement _questsList;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += DayPassed;

        _dashboardManager = GetComponent<DashboardManager>();

        _root = _dashboardManager.Root;

        _mainDaySummary = _root.Q<VisualElement>("mainDaySummary");

        _dashboardManager.OnDaySummaryClicked += Initialize;

        Initialize();
    }

    async void Initialize()
    {
        _mainDaySummary.Clear();


        Label newDay = new($"Day: {_gameManager.Day}");
        newDay.style.opacity = 0;
        newDay.style.fontSize = 76;
        _mainDaySummary.Add(newDay);
        DOTween.To(() => newDay.style.opacity.value, x => newDay.style.opacity = x, 1f, 1f);

        await Task.Delay(200);

        Label maintenance = new($"Maintenance cost: {_gameManager.GetCurrentMaintenanceCost()}");
        maintenance.style.opacity = 0;
        maintenance.style.fontSize = 36;
        _mainDaySummary.Add(maintenance);
        DOTween.To(() => maintenance.style.opacity.value, x => maintenance.style.opacity = x, 1f, 1f);

        await Task.Delay(200);

        Label rewards = new($"Rewards: ");
        rewards.style.opacity = 0;
        maintenance.style.fontSize = 48;
        _mainDaySummary.Add(rewards);
        DOTween.To(() => rewards.style.opacity.value, x => rewards.style.opacity = x, 1f, 1f);

        await Task.Delay(200);

        MyButton passDayButton = new("Pass Day", "menuButton", PassDay);
        passDayButton.style.opacity = 0;
        _mainDaySummary.Add(passDayButton);
        DOTween.To(() => passDayButton.style.opacity.value, x => passDayButton.style.opacity = x, 1f, 1f);
    }

    void PassDay() { _gameManager.PassDay(); }

    void DayPassed(int day) { Initialize(); }
}
