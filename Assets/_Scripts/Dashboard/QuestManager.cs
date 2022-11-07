using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    VisualElement _root;

    ScrollView _questsList;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;
        _dashboardManager.OnQuestsClicked += Initialize;

        _questsList = _root.Q<ScrollView>("questList");
    }

    void Initialize()
    {
        _questsList.Clear();

        foreach (Quest q in _gameManager.AvailableQuests)
            _questsList.Add(new QuestVisualElement(q));


    }


}
