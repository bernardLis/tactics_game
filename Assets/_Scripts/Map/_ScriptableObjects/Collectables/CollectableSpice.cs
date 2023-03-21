using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Spice")]
public class CollectableSpice : Collectable
{
    public override void Initialize(Vector2 mapPosition)
    {
        base.Initialize(mapPosition);
        Amount = Random.Range(10, 50);
    }

    public override void Collect(MapHero hero) { GameManager.Instance.ChangeSpiceValue(Amount); }

    public override void LoadFromData(CollectableData data)
    {
        base.LoadFromData(data);

        Sprite = GameManager.Instance.GameDatabase.SpiceSprite;
    }


}
