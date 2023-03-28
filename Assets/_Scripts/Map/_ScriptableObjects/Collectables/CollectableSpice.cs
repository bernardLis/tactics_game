using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Collectable Spice")]
public class CollectableSpice : Collectable
{

    public override void Create(Vector2 pos)
    {
        base.Create(pos);
        Amount = Random.Range(50, 200);
        Sprite = GameManager.Instance.GameDatabase.SpiceSprite;
        name = $"{Amount} Spice";
    }

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
        name = $"{data.Amount} Spice";
        Sprite = GameManager.Instance.GameDatabase.SpiceSprite;
    }


}
