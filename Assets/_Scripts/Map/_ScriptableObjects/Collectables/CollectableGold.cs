using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Gold")]
public class CollectableGold : Collectable
{
    public override void Initialize(Vector2 mapPosition)
    {
        base.Initialize(mapPosition);
        Amount = Random.Range(100, 500);
    }
    public override void Collect(MapHero hero)
    {
        GameManager.Instance.ChangeGoldValue(Amount);
    }

    public override void LoadFromData(CollectableData data)
    {
        base.LoadFromData(data);

        Sprite = GameManager.Instance.GameDatabase.GoldSprite;
    }
}
