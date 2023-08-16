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

    VisualElement _currentTooltip;
    public GameObject CurrentTooltipDisplayer { get; private set; }

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

    public void ShowInfo(string text, float duration)
    {
        ShowInfo(text);
        Invoke(nameof(HideInfo), duration);
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
            _currentTooltip = new BattleEntityCard(entity);
        if (entity is BattleCreature creature)
            _currentTooltip = new BattleCreatureCard(creature);

        CurrentTooltipDisplayer = entity.gameObject;


        _bottomPanel.Add(_currentTooltip);
    }

    public void DisplayTooltip(VisualElement el, GameObject go)
    {
        HideTooltip();
        _currentTooltip = el;
        _bottomPanel.Add(_currentTooltip);
        CurrentTooltipDisplayer = go;
    }

    public void HideTooltip()
    {
        if (_currentTooltip == null) return;
        CurrentTooltipDisplayer = null;

        //_bottomPanel.Remove(_tooltip);
        _currentTooltip.RemoveFromHierarchy();
        _currentTooltip = null;

        OnTooltipHidden?.Invoke();
    }
}
