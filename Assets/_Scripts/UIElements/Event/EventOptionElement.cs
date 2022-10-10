using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using DG.Tweening;

public class EventOptionElement : VisualElement
{
    ScreenWithDraggables _screenWithDraggables;

    EventOption _eventOption;

    public event Action<EventOptionElement> OnMouseEnter;
    public event Action OnMouseLeave;
    public event Action<EventOptionElement> OnPointerUp;


    public ItemSlotVisual ItemSlotVisual;


    public EventOptionElement(EventOption option, ScreenWithDraggables screenWithDraggables)
    {
        _eventOption = option;

        AddToClassList("textPrimary");
        AddToClassList("eventOptionElement");
        _screenWithDraggables = screenWithDraggables;

        Label text = new(option.Text);
        text.AddToClassList("textPrimary");
        text.style.whiteSpace = WhiteSpace.Normal;
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
            ItemSlotVisual = _screenWithDraggables.CreateDraggableItem(option.Reward.Item, false);
            rewardContainer.Add(ItemSlotVisual);
        }

        RegisterCallback<MouseEnterEvent>(MouseEnter);
        RegisterCallback<MouseLeaveEvent>(MouseLeave);
        RegisterCallback<PointerUpEvent>(PointerUp);
    }

    void MouseEnter(MouseEnterEvent e)
    {
        AddToClassList("eventOptionElementHover");
        OnMouseEnter?.Invoke(this);
    }

    void MouseLeave(MouseLeaveEvent e)
    {
        RemoveFromClassList("eventOptionElementHover");
        OnMouseLeave?.Invoke();
    }

    void PointerUp(PointerUpEvent e)
    {
        OnPointerUp?.Invoke(this);
    }

    public void UnregisterCallbacks()
    {
        UnregisterCallback<MouseEnterEvent>(MouseEnter);
        UnregisterCallback<MouseLeaveEvent>(MouseLeave);
        UnregisterCallback<PointerUpEvent>(PointerUp);
    }

    public void LockRewards()
    {
        style.opacity = 0.5f;
        RegisterCallback<PointerDownEvent>(LockedPointerDown);
    }

    void LockedPointerDown(PointerDownEvent e)
    {
        DOTween.Shake(() => transform.position, x => transform.position = x, 1, 5, 10, 45, true);
    }

    public bool WasRewardTaken()
    {
        if (_eventOption.Reward.Item != null && ItemSlotVisual != null)
        {
            Helpers.DisplayTextOnElement(_screenWithDraggables, ItemSlotVisual, "Take me with you", Color.red);
            return false;
        }

        return true;
    }
}
