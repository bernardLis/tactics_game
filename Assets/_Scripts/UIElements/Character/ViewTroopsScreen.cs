using System;
using UnityEngine.UIElements;

public class ViewTroopsScreen : FullScreenVisual
{
    RunManager _runManager;
    public event Action OnClose;

    public ViewTroopsScreen(VisualElement root, bool enableNavigation = true)
    {
        style.flexDirection = FlexDirection.Column;
        _runManager = RunManager.Instance;
        Initialize(root, enableNavigation);

        ScreenWithDraggables screenWithDraggables = new(root);
        Add(screenWithDraggables);
        screenWithDraggables.AddPouches();
        screenWithDraggables.AddCharacters(_runManager.PlayerTroops);

        MyButton backButton = new("Back", "menuButton", Hide);
        backButton.style.width = 200;
        screenWithDraggables.AddElement(backButton);
    }

    public override void Hide()
    {
        base.Hide();
        OnClose?.Invoke();
    }
}
