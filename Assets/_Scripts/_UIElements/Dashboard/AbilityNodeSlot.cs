using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityNodeSlot : VisualElement
{
    public AbilityNodeElement Node;
    public bool IsLocked { get; private set; }

    public event Action<AbilityNodeElement> OnNodeAdded;
    public event Action<AbilityNodeSlot> OnLocked;

    public AbilityNodeSlot(AbilityNodeElement node = null, bool isLocked = false) : base()
    {
        AddToClassList("abilityNodeSlot");

        if (node != null)
            AddNode(node);

        if (isLocked)
            Lock();
    }

    public void AddNode(AbilityNodeElement node)
    {
        if (Node != null)
            Remove(Node);

        Node = node;
        Add(node);

        OnNodeAdded?.Invoke(node);
    }

    public void RemoveNode()
    {
        if (Node != null)
            Remove(Node);
        Node = null;
    }

    void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
    }


}
