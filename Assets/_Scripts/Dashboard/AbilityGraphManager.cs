using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGraphManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    [SerializeField] GameObject _abilityGraphHolder;

    [SerializeField] GameObject _abilityNodePrefab;
    [SerializeField] AbilityNodeGraph[] _abilityNodeGraphs;

    List<AbilityNodeScript> _nodeScripts = new();
    List<ShapesPainter> _shapesPainters = new();

    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesClicked += OnAbilitiesClicked;

        CreateGraphs();
    }

    void OnAbilitiesClicked()
    {

    }

    [ContextMenu("Create graphs")]
    void CreateGraphs()
    {
        foreach (Transform child in _abilityGraphHolder.transform)
            GameObject.Destroy(child.gameObject);

        float startX = -7f;
        float startY = 1.5f;

        // HERE: for now, only 1 graph
        for (int i = 0; i < _abilityNodeGraphs[0].AbilityNodes.Length; i++)
        {
            AbilityNode n = _abilityNodeGraphs[0].AbilityNodes[i];
            Vector3 position = new(startX + 4 * i, startY);
            GameObject g = Instantiate(_abilityNodePrefab, position, Quaternion.identity);
            g.transform.parent = _abilityGraphHolder.transform;
            AbilityNodeScript script = g.GetComponent<AbilityNodeScript>();
            script.InitializeScript(n);
            _nodeScripts.Add(script);
        }

        for (int i = 0; i < _nodeScripts.Count; i++)
        {
            if (i + 1 == _nodeScripts.Count)
                break;
            _nodeScripts[i].PaintLine(_nodeScripts[i + 1]);
        }
    }
}


