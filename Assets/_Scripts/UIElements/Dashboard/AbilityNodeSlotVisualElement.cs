using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityNodeSlotVisualElement : VisualElement
{
    public AbilityNodeVisualElement Node;
    public bool IsLocked { get; private set; }

    public event Action<AbilityNodeVisualElement> OnNodeAdded;
    public event Action<AbilityNodeVisualElement> OnNodeRemoved;
    public event Action<AbilityNodeSlotVisualElement> OnLocked;


    public AbilityNodeSlotVisualElement(AbilityNodeVisualElement node = null, bool isLocked = false) : base()
    {
        AddToClassList("abilityNodeSlot");

        if (node != null)
            AddNode(node);

        if (isLocked)
            Lock();
    }

    public void AddNode(AbilityNodeVisualElement node)
    {
        Node = node;
        Add(node);

        OnNodeAdded?.Invoke(node);
    }

    void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
        //if (Node != null)
        //     Node.Lock();

        //RemoveFromClassList("characterCardMiniSlot");
        // AddToClassList("characterCardMiniSlotLocked");

    }


}
