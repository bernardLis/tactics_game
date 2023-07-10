using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityTooltipManager : Singleton<BattleEntityTooltipManager>
{
    BattleManager _battleManager;

    VisualElement _root;
    VisualElement _bottomPanel;

    VisualElement _entityInfoContainer;

    BattleEntityElement _tooltip;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += OnBattleFinalized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");
        _entityInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
    }

    void OnBattleFinalized()
    {
        HideInfo();
        HideTooltip();
    }

    public void ShowInfo(BattleEntity entity)
    {
        if (entity.IsDead) return;

        _entityInfoContainer.Clear();
        _entityInfoContainer.style.display = DisplayStyle.Flex;

        BattleEntityInfoElement info = new(entity);
        _entityInfoContainer.Add(info);
    }

    public void HideInfo()
    {
        _entityInfoContainer.style.display = DisplayStyle.None;
        _entityInfoContainer.Clear();
    }

    public void DisplayTooltip(BattleEntity entity)
    {
        HideTooltip();

        // HERE: broken due to battle entity = battle creature
        //  _tooltip = new(entity);
        _bottomPanel.Add(_tooltip);
    }

    public void HideTooltip()
    {
        if (_tooltip == null) return;
        //_bottomPanel.Remove(_tooltip);
        _tooltip.RemoveFromHierarchy();
        _tooltip = null;
    }
}
