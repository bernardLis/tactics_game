using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collectable : BaseScriptableObject
{
    public Vector2 MapPosition;
    public int Amount;
    public Sprite Sprite;
    public bool IsCollected;

    public virtual void Initialize() { }

    public virtual void Create(Vector2 pos)
    {
        MapPosition = pos;
        Id = Guid.NewGuid().ToString();
        IsCollected = false;
    }

    public virtual void Collect(MapHero hero)
    {
        Debug.Log($"{hero.Hero.HeroName} collects {this.name}");

        IsCollected = true;
        GameManager.Instance.SaveJsonData();
    }

    public virtual CollectableData SerializeSelf()
    {
        CollectableData data = new();
        data.Id = Id;
        data.MapPosition = MapPosition;
        data.Amount = Amount;
        data.IsCollected = IsCollected;
        data.Type = this.GetType().ToString();
        return data;
    }

    public virtual void LoadFromData(CollectableData data)
    {
        Id = data.Id;
        MapPosition = data.MapPosition;
        Amount = data.Amount;
        IsCollected = data.IsCollected;
    }
}

[System.Serializable]
public struct CollectableData
{
    public string Id;
    public Vector2 MapPosition;
    public int Amount;
    public string Type;
    public bool IsCollected;
    public string ItemId;

}
