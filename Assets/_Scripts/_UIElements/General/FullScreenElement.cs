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
        _gameManager.GetComponent<GameUIManager>().BlockMenuInput(true);

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
            schedule.Execute(EnableNavigation).StartingIn(500);
    }

    protected void EnableNavigation()
    {
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<KeyDownEvent>(OnKeyDown); // TODO: full screen management vs menu opening and closing
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        // only right mouse click
        if (evt.button != 1) return;

        Hide();
    }

    void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Escape) return;

        Hide();
    }

    public void AddContinueButton()
    {
        _continueButton = new("Continue", callback: Hide);
        _content.Add(_continueButton);
    }

    public virtual void Hide()
    {
        OnHide?.Invoke();

        // TODO: if open a second full screen and close one, then esc will open menu, but I don't care.
        _gameManager.GetComponent<GameUIManager>().BlockMenuInput(false);

        if (_battleManager != null && _resumeGameOnHide)
            _battleManager.ResumeGame();

        SetEnabled(false);
        RemoveFromHierarchy();
    }
}
