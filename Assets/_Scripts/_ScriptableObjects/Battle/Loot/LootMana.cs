using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Loot/Loot Mana")]
public class LootMana : Loot
{
    public Vector2Int ManaRange;
    [HideInInspector] public int Mana;

    protected override void SelectPrize()
    {
        Mana = Random.Range(ManaRange.x, ManaRange.y);
    }

    public override void Collect()
    {
        base.Collect();
        _gameManager.PlayerHero.RestoreMana(Mana);
    }

    public override string GetDisplayText()
    {
        return "+ " + Mana + " Mana";
    }

    public override Color GetDisplayColor()
    {
        return _gameManager.GameDatabase.GetColorByName("Mana").Color;
    }
}
