using UnityEngine;
using UnityEngine.UIElements;
using System;

public class FullScreenVisual : VisualElement
{
    protected VisualElement _root;

    public event Action OnHide;
    public void Initialize(VisualElement root)
    {
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        _root = root;
        root.Add(this);

        RegisterCallback<PointerDownEvent>(OnPointerDown);

        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetCommonStyles();
        styleSheets.Add(ss);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 1) // only right mouse click
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
        this.SetEnabled(false);
        _root.Remove(this);
    }
}
