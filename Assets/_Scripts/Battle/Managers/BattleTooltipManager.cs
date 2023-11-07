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

    VisualElement _gameInfoContainer;
    VisualElement _entityInfoContainer; // shows mouse hover info 
    VisualElement _keyTooltipContainer;

    string _gameInfoTweenID = "_gameInfoContainer";

    VisualElement _currentTooltip;
    public GameObject CurrentTooltipDisplayer { get; private set; }

    public BattleEntity CurrentEntityInfo { get; private set; }

    public event Action OnTooltipHidden;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += OnBattleFinalized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _entityTooltipContainer = _root.Q<VisualElement>("entityTooltipContainer");
        _entityInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
        _gameInfoContainer = _root.Q<VisualElement>("gameInfoContainer");
        _keyTooltipContainer = _root.Q<VisualElement>("keyTooltipContainer");
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
        HideEntityInfo();
        HideKeyTooltipInfo();
        HideTooltip();
    }

    public void ShowKeyTooltipInfo(VisualElement el)
    {
        _keyTooltipContainer.Clear();
        _keyTooltipContainer.Add(el);
        _keyTooltipContainer.style.display = DisplayStyle.Flex;

        DOTween.To(x => _keyTooltipContainer.style.opacity = x, 0, 1, 0.5f)
            .SetEase(Ease.InOutSine);

    }

    public void HideKeyTooltipInfo()
    {
        DOTween.To(x => _keyTooltipContainer.style.opacity = x, 1, 0, 0.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _keyTooltipContainer.style.display = DisplayStyle.None;
                _keyTooltipContainer.Clear();
            });
    }

    public void ShowEntityInfo(BattleEntity entity)
    {
        if (entity.IsDead) return;
        _entityInfoContainer.Clear();

        CurrentEntityInfo = entity;
        BattleEntityInfoElement info = new(entity);
        _entityInfoContainer.Add(info);
        _entityInfoContainer.style.display = DisplayStyle.Flex;
    }

    public void HideEntityInfo()
    {
        _entityInfoContainer.style.display = DisplayStyle.None;
        _entityInfoContainer.Clear();
    }

    public void ShowGameInfo(string text, float duration)
    {
        ShowGameInfo(text);
        Invoke(nameof(HideGameInfo), duration);
    }

    public void ShowGameInfo(string text)
    {
        _gameInfoContainer.Clear();
        Label txt = new(text);
        _gameInfoContainer.Add(txt);

        _gameInfoContainer.style.display = DisplayStyle.Flex;
        _gameInfoContainer.style.opacity = 0;
        DOTween.Kill(_gameInfoTweenID);
        DOTween.To(x => _gameInfoContainer.style.opacity = x, 0, 1, 0.5f)
                .SetEase(Ease.InOutSine);
    }

    public void HideGameInfo()
    {
        DOTween.To(x => _gameInfoContainer.style.opacity = x, 1, 0, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetId(_gameInfoTweenID)
            .OnComplete(() =>
            {
                _gameInfoContainer.style.display = DisplayStyle.None;
                _gameInfoContainer.Clear();
            });

    }

    /* TOOLTIP CARD */
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
                            if (_currentTooltip == null) return;
                            _currentTooltip.RemoveFromHierarchy();
                            _currentTooltip = null;

                            OnTooltipHidden?.Invoke();
                        });
    }
}
