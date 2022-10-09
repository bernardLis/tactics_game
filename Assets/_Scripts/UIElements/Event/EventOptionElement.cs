using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;


public class EventOptionElement : VisualElement
{
    ScreenWithDraggables _screenWithDraggables;

    public event Action<EventOptionElement> OnMouseEnter;
    public event Action OnMouseLeave;

    public EventOptionElement(EventOption option, ScreenWithDraggables screenWithDraggables)
    {
        AddToClassList("textPrimary");
        AddToClassList("eventOptionElement");
        _screenWithDraggables = screenWithDraggables;

        Label text = new(option.Text);
        Add(text);

        VisualElement rewardContainer = new();
        rewardContainer.style.flexDirection = FlexDirection.Row;
        rewardContainer.style.alignItems = Align.Center;
        Add(rewardContainer);

        if (option.Reward.Obols != 0)
        {
            Label obols = new($"Obols: {option.Reward.Obols}");
            rewardContainer.Add(obols);
        }
        if (option.Reward.Gold != 0)
        {
            Label gold = new($"Gold: {option.Reward.Gold}");
            rewardContainer.Add(gold);
        }
        if (option.Reward.Item != null)
        {
            rewardContainer.Add(_screenWithDraggables.CreateDraggableItem(option.Reward.Item));
        }

        RegisterCallback<MouseEnterEvent>((evt) => MouseEnter());
        RegisterCallback<MouseLeaveEvent>((evt) => MouseLeave());
    }

    void MouseEnter()
    {
        AddToClassList("eventOptionElementHover");
        OnMouseEnter?.Invoke(this);
    }

    void MouseLeave()
    {
        RemoveFromClassList("eventOptionElementHover");
        OnMouseLeave?.Invoke();
    }
}
