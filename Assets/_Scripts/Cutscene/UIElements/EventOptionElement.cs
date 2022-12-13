using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using DG.Tweening;

public class EventOptionElement : VisualElement
{
    ScreenWithDraggables _screenWithDraggables;

    public EventOption EventOption;

    public event Action<EventOptionElement> OnMouseEnter;
    public event Action OnMouseLeave;
    public event Action<EventOptionElement> OnPointerUp;


    ItemSlot _itemSlotVisual;
    GoldElement _goldElement;


    public EventOptionElement(EventOption option, ScreenWithDraggables screenWithDraggables)
    {
        EventOption = option;

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

        if (option.Reward.Gold != 0)
        {
            _goldElement = new(option.Reward.Gold);
            rewardContainer.Add(_goldElement);
        }
        if (option.Reward.Item != null)
        {
            _itemSlotVisual = _screenWithDraggables.CreateDraggableItem(option.Reward.Item, false);
            rewardContainer.Add(_itemSlotVisual);
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

    public void UnlockRewards()
    {
        AddToClassList("eventOptionElementClicked");

        if (_itemSlotVisual != null)
            _screenWithDraggables.UnlockItem(_itemSlotVisual.ItemVisual);
        if (_goldElement != null)
            _goldElement.MakeClickable();
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
        if (EventOption.Reward.Item != null && _itemSlotVisual != null)
        {
            Helpers.DisplayTextOnElement(_screenWithDraggables, _itemSlotVisual, "Take me with you", Color.red);
            return false;
        }

        return true;
    }
}
