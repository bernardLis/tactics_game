using UnityEngine;
using UnityEngine.UIElements;
using System;
using DG.Tweening;

public class FullScreenElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    protected const string _ussCommonMenuButton = "common__menu-button";
    protected const string _ussCommonHorizontalSpacer = "common__horizontal-spacer";
    const string _ussCommonFullScreenMain = "common__full-screen-main";
    const string _ussCommonFullScreenContent = "common__full-screen-content";


    protected GameManager _gameManager;
    BattleManager _battleManager;

    public event Action OnHide;

    VisualElement _root;

    protected VisualElement _content;
    protected ContinueButton _continueButton;

    bool _isNavigationDisabled;

    public FullScreenElement()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null) styleSheets.Add(commonStyles);

        ResolveRoot();

        _gameManager.OpenFullScreens.Add(this);
        if (_battleManager != null) _battleManager.PauseGame();

        AddToClassList(_ussCommonFullScreenMain);
        AddToClassList(_ussCommonTextPrimary);

        _content = new();
        _content.AddToClassList(_ussCommonFullScreenContent);
        Add(_content);

        focusable = true;
        Focus();

        style.opacity = 0;
        DOTween.To(x => style.opacity = x, style.opacity.value, 1, 0.5f)
            .SetUpdate(true)
            .OnComplete(EnableNavigation);
    }

    void ResolveRoot()
    {
        _root = _gameManager.Root;
        if (_battleManager != null) _root = _battleManager.Root;

        _root.Add(this);
    }

    protected void EnableNavigation()
    {
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<KeyDownEvent>(OnKeyDown); // TODO: full screen management vs menu opening and closing
    }

    public void DisableNavigation()
    {
        _isNavigationDisabled = true;
        UnregisterCallback<PointerDownEvent>(OnPointerDown);
        UnregisterCallback<KeyDownEvent>(OnKeyDown);

    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (_isNavigationDisabled) return;
        if (evt.button != 1) return; // only right mouse click


        Hide();
    }

    void OnKeyDown(KeyDownEvent evt)
    {
        if (_isNavigationDisabled) return;
        if (evt.keyCode != KeyCode.Escape) return;

        Hide();
    }

    public void AddContinueButton()
    {
        _continueButton = new("Continue", callback: Hide);
        Add(_continueButton);
    }

    public virtual void Hide()
    {
        VisualElement tt = _root.Q<VisualElement>("tooltipContainer");
        if (tt != null) tt.style.display = DisplayStyle.None;

        DOTween.To(x => style.opacity = x, style.opacity.value, 0, 0.5f).SetUpdate(true);
        DOTween.To(x => _content.style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                OnHide?.Invoke();

                _gameManager.OpenFullScreens.Remove(this);
                if (_gameManager.OpenFullScreens.Count > 0) _gameManager.OpenFullScreens[^1].Focus();
                else if (_battleManager != null) _battleManager.ResumeGame();

                SetEnabled(false);
                RemoveFromHierarchy();
            });
    }
}
