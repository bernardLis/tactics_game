using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    VisualElement _root;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += DayPassed;

        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;

        


    }

    void DayPassed(int day)
    {

    }

}
