using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    DashboardManager _dashboardManager;
    VisualElement _root;

    VisualElement _questsList;

    void Start()
    {
        _dashboardManager = GetComponent<DashboardManager>();
        _root = _dashboardManager.Root;

        _questsList = _root.Q<VisualElement>("questsList");

        Initialize();
    }

    void Initialize()
    {
        
    }

}
