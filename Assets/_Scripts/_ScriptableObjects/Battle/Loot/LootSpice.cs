using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Loot/Loot Spice")]
public class LootSpice : Loot
{
    public Vector2Int SpiceRange;
    [HideInInspector] public int Spice;

    protected override void SelectPrize()
    {
        Spice = Random.Range(SpiceRange.x, SpiceRange.y);
    }

    public override void Collect()
    {
        base.Collect();
        _gameManager.ChangeSpiceValue(Spice);
    }

    public override string GetDisplayText()
    {
        return "+ " + Spice + " Spice";
    }

    public override Color GetDisplayColor()
    {
        return Color.red;
    }

}
