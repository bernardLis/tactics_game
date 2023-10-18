using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleTooltipManager : Singleton<BattleTooltipManager>
{
    BattleManager _battleManager;

    VisualElement _root;
    VisualElement _entityTooltipContainer;

    VisualElement _textInfoContainer;
    VisualElement _entityInfoContainer;

    VisualElement _currentTooltip;
    public GameObject CurrentTooltipDisplayer { get; private set; }

    bool _isPrioritizedInfoShown;

    public event Action OnTooltipHidden;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += OnBattleFinalized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _entityTooltipContainer = _root.Q<VisualElement>("entityTooltipContainer");
        _textInfoContainer = _root.Q<VisualElement>("textInfoContainer");
        _entityInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
    }

    void OnBattleFinalized()
    {
        HideInfo();
        HideTooltip();
    }

    public void ShowInfo(BattleEntity entity)
    {
        if (_isPrioritizedInfoShown) return;

        if (entity.IsDead) return;

        _entityInfoContainer.Clear();
        _entityInfoContainer.style.display = DisplayStyle.Flex;

        BattleEntityInfoElement info = new(entity);
        _entityInfoContainer.Add(info);
    }

    public void ShowInfo(string text)
    {
        if (_isPrioritizedInfoShown) return;

        _textInfoContainer.Clear();
        _textInfoContainer.style.display = DisplayStyle.Flex;
        Label txt = new(text);
        txt.style.backgroundColor = new(new Color(0f, 0f, 0f, 0.4f));
        txt.style.fontSize = 32;

        _textInfoContainer.Add(txt);
    }

    public void ShowInfo(VisualElement element, bool priority = false)
    {
        if (_isPrioritizedInfoShown) return;
        _isPrioritizedInfoShown = priority;

        _textInfoContainer.Clear();
        _textInfoContainer.style.display = DisplayStyle.Flex;

        _textInfoContainer.Add(element);
    }

    public void RemoveInfoPriority()
    {
        _isPrioritizedInfoShown = false;
    }

    public void ShowInfo(string text, float duration)
    {
        ShowInfo(text);
        Invoke(nameof(HideInfo), duration);
    }

    public void HideEntityInfo()
    {
        _entityInfoContainer.style.display = DisplayStyle.None;
        _entityInfoContainer.Clear();
    }

    public void HideInfo()
    {
        _textInfoContainer.style.display = DisplayStyle.None;
        _textInfoContainer.Clear();
    }

    public void ShowTooltip(BattleEntity entity)
    {
        HideTooltip();
        if (entity is BattleMinion)
            _currentTooltip = new BattleEntityCard(entity);
        if (entity is BattleCreature creature)
            _currentTooltip = new BattleCreatureCard(creature);

        CurrentTooltipDisplayer = entity.gameObject;


        _entityTooltipContainer.Add(_currentTooltip);
    }

    public void ShowTooltip(VisualElement el, GameObject go)
    {
        HideTooltip();
        _currentTooltip = el;
        _entityTooltipContainer.Add(_currentTooltip);
        CurrentTooltipDisplayer = go;
    }

    public void HideTooltip()
    {
        if (_currentTooltip == null) return;
        CurrentTooltipDisplayer = null;

        _currentTooltip.RemoveFromHierarchy();
        _currentTooltip = null;

        OnTooltipHidden?.Invoke();
    }
}
