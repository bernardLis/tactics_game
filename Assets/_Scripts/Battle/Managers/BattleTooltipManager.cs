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
    VisualElement _hoverInfoContainer; // shows mouse hover info 

    VisualElement _currentTooltip;
    public GameObject CurrentTooltipDisplayer { get; private set; }

    public event Action OnTooltipHidden;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnBattleFinalized += OnBattleFinalized;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _entityTooltipContainer = _root.Q<VisualElement>("entityTooltipContainer");
        _gameInfoContainer = _root.Q<VisualElement>("textInfoContainer");
        _hoverInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
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
        HideHoverInfo();
        HideTooltip();
    }

    public void ShowHoverInfo(VisualElement el)
    {
        _hoverInfoContainer.Clear();
        _hoverInfoContainer.style.display = DisplayStyle.Flex;

        _hoverInfoContainer.Add(el);
    }

    public void ShowEntityInfo(BattleEntity entity)
    {
        if (entity.IsDead) return;

        BattleEntityInfoElement info = new(entity);
        ShowHoverInfo(info);
    }

    public void HideHoverInfo()
    {
        _hoverInfoContainer.style.display = DisplayStyle.None;
        _hoverInfoContainer.Clear();
    }


    public void ShowGameInfo(string text, float duration)
    {
        ShowGameInfo(text);
        Invoke(nameof(HideGameInfo), duration);
    }

    public void ShowGameInfo(string text)
    {
        _gameInfoContainer.Clear();
        _gameInfoContainer.style.display = DisplayStyle.Flex;
        Label txt = new(text);
        txt.style.backgroundColor = new(new Color(0f, 0f, 0f, 0.4f));
        txt.style.fontSize = 32;

        _gameInfoContainer.Add(txt);
    }

    public void HideGameInfo()
    {
        _gameInfoContainer.style.display = DisplayStyle.None;
        _gameInfoContainer.Clear();
    }

    /*TOOLTIP CARD*/
    public void ShowTooltip(BattleEntity entity)
    {
        VisualElement el = new BattleEntityCard(entity);
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
