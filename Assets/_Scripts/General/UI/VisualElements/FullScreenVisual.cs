using UnityEngine;
using UnityEngine.UIElements;

public class FullScreenVisual : VisualElement
{
    protected VisualElement _root;

    public void Initialize(VisualElement root)
    {
        style.backgroundColor = Color.gray;
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
        style.position = Position.Absolute;

        _root = root;
        root.Add(this);

        RegisterCallback<PointerDownEvent>(OnPointerDown);

        var ss = JourneyManager.Instance.GetComponent<AddressableManager>().GetCommonStyles();
        styleSheets.Add(ss);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 1) // only right mouse click
            return;

        Hide();
    }

    public void Hide()
    {
        _root.Remove(this);
    }
}
