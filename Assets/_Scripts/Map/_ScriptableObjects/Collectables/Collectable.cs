using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : BaseScriptableObject
{
    public Vector2 MapPosition;
    public int Amount;
    public Sprite Sprite;
    public bool IsCollected;

    public virtual void Initialize(Vector2 mapPosition)
    {
        MapPosition = mapPosition;
    }

    public virtual void Collect(MapHero hero)
    {
        Debug.Log($"{hero.Character.CharacterName} collects {this.name}");
        IsCollected = true;
    }
    public virtual CollectableData SerializeSelf()
    {
        CollectableData data = new();
        data.MapPosition = MapPosition;
        data.Amount = Amount;
        data.IsCollected = IsCollected;
        data.Type = this.GetType().ToString();
        return data;
    }

    public virtual void LoadFromData(CollectableData data)
    {
        MapPosition = data.MapPosition;
        Amount = data.Amount;
        IsCollected = data.IsCollected;

    }
}

[System.Serializable]
public struct CollectableData
{
    public Vector2 MapPosition;
    public int Amount;
    public string Type;
    public bool IsCollected;
    public string ItemId;

}
