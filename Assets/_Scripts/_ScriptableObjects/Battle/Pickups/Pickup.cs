using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Pickup")]
public class Pickup : BaseScriptableObject
{
    public float PickupChance;
    public Color PickupColor;

    public float GoldChance;
    public Vector2Int GoldRange;
    [HideInInspector] public int Gold;

    public float SpiceChance;
    public Vector2Int SpiceRange;
    [HideInInspector] public int Spice;

    public float CommonItemChance;
    public float UncommonItemChance;
    public float RareItemChance;
    public float EpicItemChance;
    [HideInInspector] public Item Item;

    public GameObject Effect;
    public GameObject ClickEffect;

    public Sound DropSound;

    public void Initialize()
    {
        float roll = Random.value;

        SelectPrize(roll);

        Debug.Log($"Initializing pickup {name}, roll: {roll}, gold: {Gold}, spice: {Spice}, item: {Item}");
    }

    void SelectPrize(float v)
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

        if (v <= SpiceChance)
        {
            Spice = Random.Range(SpiceRange.x, SpiceRange.y);
            return;
        }
        v -= SpiceChance;

        if (v <= GoldChance)
        {
            Gold = Random.Range(GoldRange.x, GoldRange.y);
            return;
        }
        v -= GoldChance;
    }

    public void Collect()
    {
        GameManager gameManager = GameManager.Instance;
        if (Item != null)
            gameManager.PlayerHero.AddItem(Item);
        if (Spice > 0)
            gameManager.ChangeSpiceValue(Spice);
        if (Gold > 0)
            gameManager.ChangeGoldValue(Gold);
    }

    public string GetDisplayText()
    {
        if (Item != null)
            return "+ " + Item.ItemName;
        if (Spice > 0)
            return "+ " + Spice + " Spice";
        if (Gold > 0)
            return "+ " + Gold + " Gold";

        return "";
    }

    public Color GetDisplayColor()
    {
        if (Item != null)
            return Helpers.GetColor(Item.Rarity.ToString());
        if (Spice > 0)
            return Color.red;
        if (Gold > 0)
            return Color.yellow;

        return Color.white;
    }

    public Sound GetPickupSound()
    {

        if (Item != null)
            return AudioManager.Instance.GetSound("Item Pickup");
        if (Spice > 0)
            return AudioManager.Instance.GetSound("Spice Pickup");
        if (Gold > 0)
            return AudioManager.Instance.GetSound("Gold Pickup");

        return null;
    }

}
