using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;

public class FullScreenVisual : VisualElement
{
    protected VisualElement _root;

    public event Action OnHide;
    public async void Initialize(VisualElement root, bool enableNavigation = true)
    {
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        _root = root;
        root.Add(this);

        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetCommonStyles();
        if (ss != null)
            styleSheets.Add(ss);

        GameManager.Instance.GetComponent<GameUIManager>().DisableMenuButton(); // TODO: ugh...

        await Task.Delay(100);
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
        MyButton backButton = new("Back", "menuButton", Hide);
        backButton.style.width = 200;
        Add(backButton);
    }

    public virtual void Hide()
    {
        OnHide?.Invoke();
        GameManager.Instance.GetComponent<GameUIManager>().EnableMenuButton(); // TODO: ugh...

        this.SetEnabled(false);
        if (this.parent == _root)
            _root.Remove(this);
    }
}
