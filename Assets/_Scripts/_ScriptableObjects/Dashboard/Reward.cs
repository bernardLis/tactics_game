using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward")]
public class Reward : BaseScriptableObject
{
    protected GameManager _gameManager;
    [Header("Main")]
    public int Gold;
    public Item Item;

    [Header("Randomized")]
    public bool IsRandomized;
    public Vector2Int GoldRange;
    public bool HasItem;

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
        }
    }

    public virtual void GetReward()
    {
        _gameManager = GameManager.Instance;

        if (Gold != 0)
            _gameManager.ChangeGoldValue(Gold);

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
    // TODO: need icon with id
    public int Gold;
    public string ItemReferenceId;
}
