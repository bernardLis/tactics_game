using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;

public class FullScreenVisual : VisualElement
{
    protected VisualElement _root;

    public event Action OnHide;
    public async void Initialize(VisualElement root)
    {
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        _root = root;
        root.Add(this);

        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetCommonStyles();
        styleSheets.Add(ss);

        GameManager.Instance.GetComponent<GameUIManager>().DisableMenuButton(); // TODO: ugh...

        await Task.Delay(100);
        focusable = true;
        Focus();

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
        Button backButton = new Button();
        backButton.text = "Back";
        backButton.AddToClassList("menuButton");
        backButton.style.width = 200;
        backButton.clickable.clicked += Hide;
        Add(backButton);
    }

    public virtual void Hide()
    {
        OnHide?.Invoke();
        GameManager.Instance.GetComponent<GameUIManager>().EnableMenuButton(); // TODO: ugh...

        this.SetEnabled(false);
        _root.Remove(this);
    }
}
