using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityNodeSlot : VisualElement
{
    public AbilityNodeElement Node;
    public bool IsLocked { get; private set; }

    const string _ussClassName = "ability-crafting__";
    const string _ussNodeSlot = _ussClassName + "node-slot";

    public event Action<AbilityNodeElement> OnNodeAdded;
    public event Action OnNodeRemoved;
    public event Action<AbilityNodeSlot> OnLocked;
    public AbilityNodeSlot(AbilityNodeElement node = null, bool isLocked = false) : base()
    {
        AddToClassList(_ussNodeSlot);

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
        if (Node == null)
            return;

        Remove(Node);
        Node = null;
        OnNodeRemoved?.Invoke();
    }

    void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
    }
}
