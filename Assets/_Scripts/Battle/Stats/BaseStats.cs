using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class BaseStats : MonoBehaviour
{
    // global
    protected HighlightManager _highlighter;

    // local
    protected ObjectUI _damageUI;

    // statuses
    [HideInInspector] public List<Status> Statuses = new();
    public bool IsElectrified { get; private set; }
    public bool IsStunned { get; private set; }
    public bool IsWet { get; private set; }
    public bool IsFocused { get; private set; }
    public bool IsShielded { get; private set; }

    // actions
    [HideInInspector] public event Action<Status> OnStatusAdded;
    [HideInInspector] public event Action<Status> OnStatusRemoved;

    protected virtual void Awake()
    {
        // global
        _highlighter = HighlightManager.Instance;

        // local
        _damageUI = GetComponent<ObjectUI>();
    }

    public async virtual Task<Status> AddStatus(Status s, GameObject attacker, bool trigger = true)
    {
        Status clone = AddStatusWithoutTrigger(s, attacker);
        // status triggers right away
        if (trigger)
            await clone.FirstTrigger();
        return clone;
    }

    protected Status AddStatusWithoutTrigger(Status s, GameObject attacker)
    {
        // statuses don't stack, they are refreshed
        RemoveStatus(s);

        Status clone = Instantiate(s);
        Statuses.Add(clone);
        clone.Initialize(gameObject, attacker);
        OnStatusAdded?.Invoke(clone);

        return clone;
    }

    public void RemoveStatus(Status s)
    {
        Status toRemove = null;
        foreach (Status status in Statuses)
            if (status.ReferenceID == s.ReferenceID)
                toRemove = status;

        if (toRemove != null)
        {
            toRemove.ResetFlag();
            Statuses.Remove(toRemove);
            OnStatusRemoved?.Invoke(toRemove);
        }
    }

    public void SetIsElectrified(bool isElectrified) { IsElectrified = isElectrified; }
    public void SetIsStunned(bool isStunned) { IsStunned = isStunned; }
    public void SetIsWet(bool isWet) { IsWet = isWet; }
    public void SetIsFocused(bool isFocused) { IsFocused = isFocused; }
    public void SetIsShielded(bool isShielded) { IsShielded = isShielded; }
}
