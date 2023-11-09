using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building")]
public class Building : BaseScriptableObject
{
    public Sprite Icon;
    public bool IsSecured;
    
    public event Action OnSecured;
    public event Action OnCorrupted;

    public virtual void Initialize()
    { }

    public void Secure()
    {
        IsSecured = true;
        OnSecured?.Invoke();
    }

    public void Corrupted()
    {
        IsSecured = false;
        OnCorrupted?.Invoke();
    }


}
