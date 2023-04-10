using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Item")]
public class CollectableItem : Collectable
{
    public Item Item;

    public override void Create(Vector2 pos)
    {
        base.Create(pos);
        Item = GameManager.Instance.HeroDatabase.GetRandomItem();
        Sprite = Item.Icon;
        name = $"{Item.ItemName}";
    }

    public override void Initialize()
    {
        base.Initialize();
    }
    public override void Collect(MapHero hero)
    {
        base.Collect(hero);
        hero.Hero.AddItem(Item);
    }

    public override CollectableData SerializeSelf()
    {
        CollectableData data = new();
        data = base.SerializeSelf();

        data.ItemId = Item.Id;
        return data;
    }

    public override void LoadFromData(CollectableData data)
    {
        base.LoadFromData(data);
        Item = GameManager.Instance.HeroDatabase.GetItemById(data.ItemId);
        Sprite = Item.Icon;
        name = $"{Item.ItemName}";

    }
}
