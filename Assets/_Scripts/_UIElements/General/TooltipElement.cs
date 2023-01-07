using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class TooltipElement : VisualElement
{
    int offsetX = 20;
    int offsetY = 30;

    VisualElement _parentElement;
    VisualElement _tooltipElement;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "tooltip-element";
    const string _ussMain = _ussClassName + "__main";

    public TooltipElement(VisualElement parent, VisualElement tooltipElement, bool disableTooltipStyle = false)
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TooltipElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _parentElement = parent;
        _tooltipElement = tooltipElement;
        OnPostVisualCreation();

        style.position = Position.Absolute;

        if (!disableTooltipStyle)
        {
            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussMain);
        }

        Add(tooltipElement);
    }

    public void UpdatePosition(VisualElement element)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 pos = UnityEngine.UIElements.RuntimePanelUtils.ScreenToPanel(element.panel,
                        new Vector2(mousePosition.x, Screen.height - mousePosition.y));

        // TODO: inelegant solution, when character card is tooltip element it does not resolve style for some reason. 
        float elWidth = resolvedStyle.width == 0 ? _tooltipElement.resolvedStyle.width : resolvedStyle.width;
        float elHeight = resolvedStyle.height == 0 ? _tooltipElement.resolvedStyle.height : resolvedStyle.height;

        if (pos.x + offsetX + elWidth > Screen.width)
            style.left = pos.x - (elWidth + offsetX);
        else
            style.left = pos.x + offsetX;

        if (pos.y - elHeight + offsetY < 0)
            style.top = pos.y;
        else
            style.top = pos.y - elHeight + offsetY;
    }

    //https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
    void OnPostVisualCreation()
    {
        // Make invisible so you don't see the size re-adjustment
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
