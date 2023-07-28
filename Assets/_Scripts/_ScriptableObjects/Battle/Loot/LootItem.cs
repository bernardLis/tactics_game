using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Loot/Loot Item")]
public class LootItem : Loot
{
    public float CommonItemChance;
    public float UncommonItemChance;
    public float RareItemChance;
    public float EpicItemChance;
    [HideInInspector] public Item Item;
    protected override void SelectPrize(float v)
    {
        if (v <= EpicItemChance)
        {
            Item = GameManager.Instance.HeroDatabase.GetRandomItem(ItemRarity.Epic);
            return;
        }
        v -= EpicItemChance;

        if (v <= RareItemChance)
        {
            Item = GameManager.Instance.HeroDatabase.GetRandomItem(ItemRarity.Rare);
            return;
        }
        v -= RareItemChance;

        if (v <= UncommonItemChance)
        {
            Item = GameManager.Instance.HeroDatabase.GetRandomItem(ItemRarity.Uncommon);
            return;
        }
        v -= UncommonItemChance;

        if (v <= CommonItemChance)
        {
            Item = GameManager.Instance.HeroDatabase.GetRandomItem(ItemRarity.Common);
            return;
        }
        v -= CommonItemChance;
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
