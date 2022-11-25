using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityGraphManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    [SerializeField] AbilityNodeGraph[] _abilityNodeGraphs;

    VisualElement _root;
    VisualElement _abilityGraphs;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesOpened += OnAbilitiesClicked;
        _root = _dashboardManager.Root;

        _abilityGraphs = _root.Q<VisualElement>("abilityGraphs");
        _abilityGraphs.Clear();

        CreateGraphs();
    }

    void OnAbilitiesClicked()
    {

    }

    void CreateGraphs()
    {
        VisualElement graphContainer = new();
        graphContainer.AddToClassList("graphContainer");
        graphContainer.AddToClassList("textPrimary");
        Label title = new(_abilityNodeGraphs[0].Title);
        title.style.fontSize = 48;
        graphContainer.Add(title);

        _abilityGraphs.Add(graphContainer);

        // HERE: for now, only 1 graph
        for (int i = 0; i < _abilityNodeGraphs[0].AbilityNodes.Length; i++)
        {
            AbilityNode n = _abilityNodeGraphs[0].AbilityNodes[i];
            AbilityNodeVisualElement el = new(n);
            graphContainer.Add(el);
        }
    }
}


