using UnityEngine;
using UnityEngine.UIElements;
using System;

public class FullScreenElement : VisualElement
{
    GameManager _gameManager;
    BattleManager _battleManager;

    protected VisualElement _root;

    public event Action OnHide;

    const string _ussCommonMenuButton = "common__menu-button";

    public void Initialize(bool enableNavigation = true)
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);

        _root = _gameManager.Root;
        _gameManager.GetComponent<GameUIManager>().DisableMenuButton(); // TODO: ugh...

        _battleManager = BattleManager.Instance;
        if (_battleManager != null)
        {
            _root = _battleManager.Root;
            _battleManager.PauseGame();
        }
        _root.Add(this);

        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        focusable = true;
        Focus();

        if (enableNavigation)
            EnableNavigation();
    }

    protected void EnableNavigation()
    {
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<KeyDownEvent>(OnKeyDown);
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

    public void AddBackButton()
    {
        MyButton backButton = new("Back", _ussCommonMenuButton, Hide);
        backButton.style.width = 200;
        Add(backButton);
    }

    public virtual void Hide()
    {
        OnHide?.Invoke();
        _gameManager.GetComponent<GameUIManager>().EnableMenuButton(); // TODO: ugh...

        if (_battleManager != null) _battleManager.ResumeGame();

        this.SetEnabled(false);
        //  if (this.parent == _root)
        //        _root.Remove(this);
        //    else
        this.RemoveFromHierarchy();
    }
}
