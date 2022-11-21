using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Reward")]
public class Reward : BaseScriptableObject
{
    protected GameManager _gameManager;
    [Header("Main")]
    public int Gold;
    public Item Item;
    public int YellowSpice;
    public int BlueSpice;
    public int RedSpice;

    [Header("Randomized")]
    public bool IsRandomized;

    public Vector2Int GoldRange;
    public bool HasItem;

    public Vector2Int YellowSpiceRange;
    public Vector2Int BlueSpiceRange;
    public Vector2Int RedSpiceRange;


    [Header("Sacrifice")]
    public StatType SacrificedStat;
    [Range(0, 1)]
    public float PercentSacrificed;

    [Header("Recruit")]
    public Character Recruit;
    public Brain PursuingEnemy;

    public virtual void Initialize()
    {
        if (IsRandomized)
        {
            // meant to be overwritten
            Gold = Random.Range(GoldRange.x, GoldRange.y);
            Item = GameManager.Instance.GameDatabase.GetRandomItem();

            YellowSpice = Random.Range(YellowSpiceRange.x, YellowSpiceRange.y);
            BlueSpice = Random.Range(BlueSpiceRange.x, BlueSpiceRange.y);
            RedSpice = Random.Range(RedSpiceRange.x, RedSpiceRange.y);
        }
    }

    public virtual void CreateRandom()
    {
        IsRandomized = true;

        GoldRange = new Vector2Int(400, 1000);
        HasItem = true;

        YellowSpiceRange = new Vector2Int(10, 100);
        BlueSpiceRange = new Vector2Int(10, 100);
        RedSpiceRange = new Vector2Int(10, 100);

        Initialize();
    }

    public virtual void GetReward()
    {
        _gameManager = GameManager.Instance;

        if (Gold != 0)
            _gameManager.ChangeGoldValue(Gold);

        if (Item != null)
            _gameManager.AddItemToPouch(Item);
            
        if (YellowSpice != 0)
            _gameManager.ChangeYellowSpiceValue(YellowSpice);
        if (BlueSpice != 0)
            _gameManager.ChangeBlueSpiceValue(BlueSpice);
        if (RedSpice != 0)
            _gameManager.ChangeRedSpiceValue(RedSpice);

        if (PercentSacrificed != 0)
            HandleSacrifice();

        if (Recruit != null)
            HandleRecruit();
    }

    void HandleSacrifice()
    {
        foreach (Character c in _gameManager.PlayerTroops)
        {
            int val = Mathf.FloorToInt(c.GetStatValue(SacrificedStat.ToString()) * PercentSacrificed);
            c.ChangeStat(SacrificedStat.ToString(), -val);
        }

        _gameManager.SaveJsonData();
    }

    void HandleRecruit() { _gameManager.AddCharacterToTroops(Recruit); }


    public RewardData SerializeSelf()
    {
        RewardData rd = new();
        rd.Gold = Gold;
        rd.ItemReferenceId = Item.ReferenceID;

        return rd;
    }
}

[Serializable]
public struct RewardData
{
    public int Gold;
    public string ItemReferenceId;
}
