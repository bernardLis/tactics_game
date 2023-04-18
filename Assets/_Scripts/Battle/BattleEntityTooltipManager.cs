using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityTooltipManager : Singleton<BattleEntityTooltipManager>
{

    VisualElement _root;
    VisualElement _bottomPanel;

    BattleEntityElement _tooltip;

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");
    }

    public void DisplayTooltip(BattleEntity entity)
    {
        HideTooltip();
        
        Debug.Log("Displaying tooltip for " + entity.name);
        _tooltip = new(entity);
        _bottomPanel.Add(_tooltip);
    }

    public void HideTooltip()
    {
        if (_tooltip == null) return;
        Debug.Log("Hiding tooltip");
        _bottomPanel.Remove(_tooltip);
    }
}
