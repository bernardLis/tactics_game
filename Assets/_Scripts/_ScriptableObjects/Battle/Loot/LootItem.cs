using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Loot/Loot Item")]
public class LootItem : Loot
{
    public ItemRarity Rarity;

    [HideInInspector] public Item Item;
    protected override void SelectPrize()
    {
        Item = GameManager.Instance.HeroDatabase.GetRandomItem(Rarity);
        return;
    }

    public override void Collect()
    {
        base.Collect();
        _gameManager.PlayerHero.AddItem(Item);
    }

    public override string GetDisplayText()
    {
        return Helpers.ParseScriptableObjectName(Item.name);
    }

    public override Color GetDisplayColor()
    {
        return _gameManager.GameDatabase.GetColorByName(Item.Rarity.ToString()).Color;
    }
}
