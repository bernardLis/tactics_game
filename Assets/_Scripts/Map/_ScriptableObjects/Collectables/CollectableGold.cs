using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Gold")]
public class CollectableGold : Collectable
{
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

        Sprite = GameManager.Instance.GameDatabase.GoldSprite;
    }
}
