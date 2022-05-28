using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class TooltipVisual : VisualElement
{
    Label _text;
    int offsetX = 5;
    int offsetY = -10;

    public TooltipVisual(VisualElement element, string text)
    {
        style.backgroundColor = Color.black;
        
        _text = new(text);
        _text.style.color = Color.white;
        _text.style.fontSize = 32;
        Add(_text);

        //https://forum.unity.com/threads/how-can-i-move-a-visualelement-to-the-position-of-the-mouse.1187890/
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 pos = UnityEngine.UIElements.RuntimePanelUtils.ScreenToPanel(element.panel,
                        new Vector2(mousePosition.x, Screen.height - mousePosition.y));

        style.position = Position.Absolute;
        style.left = pos.x + offsetX;
        style.top = pos.y + offsetY - resolvedStyle.height;
        OnPostVisualCreation();
    }

    public void UpdatePosition(VisualElement element)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector2 pos = UnityEngine.UIElements.RuntimePanelUtils.ScreenToPanel(element.panel,
                        new Vector2(mousePosition.x, Screen.height - mousePosition.y));

        style.left = pos.x + offsetX;
        style.top = pos.y + offsetY - resolvedStyle.height;
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
    }

}
