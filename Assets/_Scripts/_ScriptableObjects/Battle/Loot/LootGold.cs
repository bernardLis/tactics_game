using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Loot/Loot Gold")]
public class LootGold : Loot
{
    public Vector2Int GoldRange;
    [HideInInspector] public int Gold;

    protected override void SelectPrize(float v)
    {
        Gold = Random.Range(GoldRange.x, GoldRange.y);
    }

    public override void Collect()
    {
        base.Collect();
        _gameManager.ChangeGoldValue(Gold);
    }

    public override string GetDisplayText()
    {
        return "+ " + Gold + " Gold";
    }

    public override Color GetDisplayColor()
    {
        return Color.yellow;
    }
}
