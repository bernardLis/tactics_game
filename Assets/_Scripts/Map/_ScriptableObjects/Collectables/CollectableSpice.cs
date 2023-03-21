using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Spice")]
public class CollectableSpice : Collectable
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Collect(MapHero hero)
    {
        base.Collect(hero);
        GameManager.Instance.ChangeSpiceValue(Amount);
    }

    public override void LoadFromData(CollectableData data)
    {
        base.LoadFromData(data);

        Sprite = GameManager.Instance.GameDatabase.SpiceSprite;
    }


}
