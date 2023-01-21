using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Reward")]
public class Reward : BaseScriptableObject
{
    protected GameManager _gameManager;

    [Header("Main")]
    public int Gold;
    public Item Item;
    public int Spice;

    [Header("Randomized")]
    public bool IsRandomized;

    public Vector2Int GoldRange;
    public bool HasItem;

    public Vector2Int SpiceRange;


    public virtual void Initialize()
    {
        if (IsRandomized)
        {
            // meant to be overwritten
            Gold = Random.Range(GoldRange.x, GoldRange.y);
            Item = GameManager.Instance.GameDatabase.GetRandomItem();
            Spice = Random.Range(SpiceRange.x, SpiceRange.y);
        }
    }

    public virtual void CreateRandom()
    {
        IsRandomized = true;

        GoldRange = new Vector2Int(400, 1000);
        HasItem = true;

        SpiceRange = new Vector2Int(10, 100);

        Initialize();
    }

    public virtual void GetReward()
    {
        _gameManager = GameManager.Instance;

        if (Gold != 0)
            _gameManager.ChangeGoldValue(Gold);

        if (Spice != 0)
            _gameManager.ChangeSpiceValue(Spice);
    }

    public RewardData SerializeSelf()
    {
        RewardData rd = new();
        rd.Gold = Gold;

        rd.ItemId = Item == null ? null : Item.Id;

        return rd;
    }
}

[Serializable]
public struct RewardData
{
    public int Gold;
    public string ItemId;
}
