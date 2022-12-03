using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class BattleLogVisual : FullScreenVisual
{
    public BattleLogVisual(List<VisualElement> battleLogs)
    {
        Label header = new Label("Battle Log");
        header.AddToClassList("textPrimary");
        header.style.fontSize = 48;
        Add(header);

        style.backgroundColor = Color.black;
        BattleManager.Instance.PauseGame();
        ScrollView view = new();
        Add(view);

        List<VisualElement> logsCopy = new(battleLogs);
        logsCopy.Reverse();
        foreach (VisualElement log in logsCopy)
            log.style.opacity = 1;
        foreach (VisualElement log in logsCopy)
            view.Add(log);
        
        AddBackButton();
    }
    public override void Hide()
    {
        BattleManager.Instance.ResumeGame();
        base.Hide();
    }

}