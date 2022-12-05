using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class TooltipVisual : VisualElement
{
    int offsetX = 20;
    int offsetY = 30;

    VisualElement _parentElement;

    public TooltipVisual(VisualElement element, VisualElement tooltipElement, bool disableTooltipStyle = false)
    {
        _parentElement = element;

        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetCommonStyles();
        styleSheets.Add(ss);

        //https://forum.unity.com/threads/how-can-i-move-a-visualelement-to-the-position-of-the-mouse.1187890/
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (element == null)
            return;
        Vector2 pos = UnityEngine.UIElements.RuntimePanelUtils.ScreenToPanel(element.panel,
                        new Vector2(mousePosition.x, Screen.height - mousePosition.y));

        style.position = Position.Absolute;
        style.left = pos.x + offsetX;
        style.top = pos.y + offsetY - resolvedStyle.height;
        OnPostVisualCreation();

        AddToClassList("textPrimary");
        if (!disableTooltipStyle)
            AddToClassList("tooltipElement");

        Add(tooltipElement);
    }

    public void UpdatePosition(VisualElement element)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 pos = UnityEngine.UIElements.RuntimePanelUtils.ScreenToPanel(element.panel,
                        new Vector2(mousePosition.x, Screen.height - mousePosition.y));

        if (pos.x + offsetX + resolvedStyle.width > Screen.width)
            style.left = pos.x - (resolvedStyle.width + offsetX);
        else
            style.left = pos.x + offsetX;

        if (pos.y - resolvedStyle.height + offsetY < 0)
            style.top = Screen.height - (pos.y + resolvedStyle.height + offsetY); // TODO: wrong but I can't math.
        else
            style.top = pos.y - resolvedStyle.height + offsetY;
    }

    //https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
    void OnPostVisualCreation()
    {
        // Make invisble so you don't see the size re-adjustment
        // (Non-visible objects still go through transforms in the layout engine)
        visible = false;
        schedule.Execute(WaitOneFrame);
    }

    void WaitOneFrame(TimerState obj)
    {
        // Because waiting once wasn't working
        schedule.Execute(AutoSize);
    }

    void AutoSize(TimerState obj)
    {
        // Do any measurements, size adjustments you need (NaNs not an issue now)
        MarkDirtyRepaint();
        visible = true;
        UpdatePosition(_parentElement);
    }

}
