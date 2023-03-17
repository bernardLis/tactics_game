using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Item")]
public class CollectableItem : Collectable
{
    public Item Item;
    public override void Initialize()
    {
        Item = GameManager.Instance.GameDatabase.GetRandomItem();
        Sprite = Item.Icon;
    }
    public override void Collect(MapHero hero)
    {
        hero.Character.AddItem(Item);
    }

}
