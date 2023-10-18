using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using DG.Tweening;

public class BattleTooltipManager : Singleton<BattleTooltipManager>
{
    GameManager _gameManager;
    PlayerInput _playerInput;
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

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Battle");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void OnDestroy()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["RightMouseClick"].performed += RightMouseClick;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["RightMouseClick"].performed -= RightMouseClick;
    }

    void RightMouseClick(InputAction.CallbackContext ctx)
    {
        if (CurrentTooltipDisplayer == null) return;
        HideTooltip();
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
        VisualElement el = null;
        if (entity is BattleMinion)
            el = new BattleEntityCard(entity);
        if (entity is BattleCreature creature)
            el = new BattleCreatureCard(creature);

        ShowTooltip(el, entity.gameObject);
    }

    public void ShowTooltip(VisualElement el, GameObject go)
    {
        bool tooltipAnimation = _currentTooltip == null;

        _currentTooltip = el;
        _entityTooltipContainer.Add(_currentTooltip);
        CurrentTooltipDisplayer = go;

        StartCoroutine(ShowTooltipCoroutine(tooltipAnimation));
    }

    IEnumerator ShowTooltipCoroutine(bool isAnimated)
    {
        _entityTooltipContainer.Clear();
        _entityTooltipContainer.Add(_currentTooltip);

        if (!isAnimated) yield break;
        yield return new WaitForSeconds(0.1f);

        _entityTooltipContainer.style.left = -_currentTooltip.resolvedStyle.width;
        _entityTooltipContainer.style.visibility = Visibility.Visible;
        DOTween.To(x => _entityTooltipContainer.style.left = x, -_currentTooltip.resolvedStyle.width, 0, 0.5f)
                .SetEase(Ease.InOutSine);
    }

    public void HideTooltip()
    {
        if (_currentTooltip == null) return;
        CurrentTooltipDisplayer = null;

        DOTween.To(x => _entityTooltipContainer.style.left = x, 0, -_currentTooltip.worldBound.width, 0.5f)
                        .SetEase(Ease.InOutSine)
                        .OnComplete(() =>
                        {
                            _currentTooltip.RemoveFromHierarchy();
                            _currentTooltip = null;

                            OnTooltipHidden?.Invoke();
                        });
    }
}
