using UnityEngine;
using UnityEngine.UIElements;
using System;

public class FullScreenElement : VisualElement
{
    const string _ussCommonMenuButton = "common__menu-button";
    const string _ussCommonFullScreenMain = "common__full-screen-main";
    const string _ussCommonFullScreenContent = "common__full-screen-content";


    protected GameManager _gameManager;
    BattleManager _battleManager;

    public event Action OnHide;

    bool _resumeGameOnHide;

    protected VisualElement _content;
    protected ContinueButton _continueButton;

    public FullScreenElement(bool isKeyNavigationEnabled = true)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        _battleManager = BattleManager.Instance;
        if (_battleManager != null && _battleManager.IsTimerOn)
        {
            _resumeGameOnHide = true;
            _battleManager.PauseGame();
        }

        _gameManager.Root.Add(this);

        AddToClassList(_ussCommonFullScreenMain);

        _content = new();
        _content.AddToClassList(_ussCommonFullScreenContent);
        Add(_content);

        focusable = true;
        Focus();

        if (isKeyNavigationEnabled)
            EnableNavigation();
    }

    protected void EnableNavigation()
    {
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        // RegisterCallback<KeyDownEvent>(OnKeyDown); // HERE: full screen
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 1) // only right mouse click
            return;

        Hide();
    }

    void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Escape)
            return;

        Hide();
    }

    public void AddContinueButton()
    {
        _continueButton = new("Continue", callback: Hide);
        _content.Add(_continueButton);
    }

    public virtual void Hide()
    {
        Debug.Log($"hide");
        OnHide?.Invoke();

        if (_battleManager != null && _resumeGameOnHide)
            _battleManager.ResumeGame();

        SetEnabled(false);
        RemoveFromHierarchy();
    }
}
