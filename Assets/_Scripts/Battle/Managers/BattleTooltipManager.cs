using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleTooltipManager : Singleton<BattleTooltipManager>
{
    BattleManager _battleManager;

    VisualElement _root;
    VisualElement _bottomPanel;

    VisualElement _topContainer;

    VisualElement _tooltip;

    public event Action OnTooltipHidden;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += OnBattleFinalized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");
        _topContainer = _root.Q<VisualElement>("entityInfoContainer");
    }

    void OnBattleFinalized()
    {
        HideInfo();
        HideTooltip();
    }

    public void ShowInfo(BattleEntity entity)
    {
        if (entity.IsDead) return;

        _topContainer.Clear();
        _topContainer.style.display = DisplayStyle.Flex;

        BattleEntityInfoElement info = new(entity);
        _topContainer.Add(info);
    }

    public void ShowInfo(string text)
    {
        _topContainer.Clear();
        _topContainer.style.display = DisplayStyle.Flex;
        Label txt = new(text);
        txt.style.backgroundColor = new(new Color(0f, 0f, 0f, 0.4f));
        txt.style.fontSize = 32;

        _topContainer.Add(txt);
    }

    public void HideInfo()
    {
        _topContainer.style.display = DisplayStyle.None;
        _topContainer.Clear();
    }

    public void DisplayTooltip(BattleEntity entity)
    {
        HideTooltip();
        if (entity is BattleMinion)
            _tooltip = new BattleEntityCard(entity);
        if (entity is BattleCreature creature)
            _tooltip = new BattleCreatureCard(creature);

        _bottomPanel.Add(_tooltip);
    }

    public TurretCard DisplayTooltip(Turret turret)
    {
        HideTooltip();
        _tooltip = new TurretCard(turret);
        _bottomPanel.Add(_tooltip);
        return (TurretCard)_tooltip;
    }

    public void HideTooltip()
    {
        if (_tooltip == null) return;

        //_bottomPanel.Remove(_tooltip);
        _tooltip.RemoveFromHierarchy();
        _tooltip = null;
        
        OnTooltipHidden?.Invoke();
    }
}
