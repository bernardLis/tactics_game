using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Gold")]
public class CollectableGold : Collectable
{
    public override void Create(Vector2 pos)
    {
        base.Create(pos);
        Amount = Random.Range(50, 200);
        Sprite = GameManager.Instance.GameDatabase.GoldSprite;
        name = $"{Amount} Gold";

    }

    public override void Initialize()
    {
        base.Initialize();
    }
    public override void Collect(MapHero hero)
    {
        base.Collect(hero);
        GameManager.Instance.ChangeGoldValue(Amount);
    }

    public override void LoadFromData(CollectableData data)
    {
        base.LoadFromData(data);
        name = $"{data.Amount} Gold";
        Sprite = GameManager.Instance.GameDatabase.GoldSprite;
    }
}
