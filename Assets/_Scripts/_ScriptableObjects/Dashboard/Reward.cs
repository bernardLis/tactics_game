using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Reward")]
public class Reward : BaseScriptableObject
{
    protected GameManager _gameManager;

    [Header("Static")]
    public int Rank;
    public Sprite[] ChestIdleSprites;
    public Sprite[] ChestOpenSprites;
    public Vector2Int GoldRange;
    public Vector2Int SpiceRange;
    public float UncommonItemChance;
    public float RareItemChance;
    public float EpicItemChance;

    [HideInInspector] public int Gold;
    [HideInInspector] public Item Item;
    [HideInInspector] public int Spice;

    public virtual void Initialize()
    {
        _gameManager = GameManager.Instance;

        Gold = Random.Range(GoldRange.x, GoldRange.y);
        Spice = Random.Range(SpiceRange.x, SpiceRange.y);
        Item = ChooseItem();
    }

    Item ChooseItem()
    {
        float v = Random.value;
        if (v < EpicItemChance)
            return _gameManager.GameDatabase.GetRandomEpicItem();
        if (v < RareItemChance)
            return _gameManager.GameDatabase.GetRandomRareItem();
        if (v < UncommonItemChance)
            return _gameManager.GameDatabase.GetRandomUncommonItem();

        return _gameManager.GameDatabase.GetRandomCommonItem();
    }

    public virtual void GetReward()
    {
        if (Gold != 0)
            _gameManager.ChangeGoldValue(Gold);

        if (Spice != 0)
            _gameManager.ChangeSpiceValue(Spice);
    }

    public void LoadFromData(RewardData rd)
    {
        _gameManager = GameManager.Instance;

        Gold = rd.Gold;
        Spice = rd.Spice;
        Item = rd.ItemId == null ? null : _gameManager.GameDatabase.GetItemById(rd.ItemId);
    }

    public RewardData SerializeSelf()
    {
        RewardData rd = new();
        rd.Gold = Gold;
        rd.Spice = Spice;
        rd.ItemId = Item == null ? null : Item.Id;

        return rd;
    }

}

[Serializable]
public struct RewardData
{
    public int Gold;
    public int Spice;
    public string ItemId;
}
