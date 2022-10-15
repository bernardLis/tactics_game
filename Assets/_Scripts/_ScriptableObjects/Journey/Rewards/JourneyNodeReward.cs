using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward")]
public class JourneyNodeReward : BaseScriptableObject
{
    protected GameManager _gameManager;
    [Header("Main")]
    public int Obols;
    public int Gold;
    public Item Item;

    [Header("Sacrifice")]
    public StatType SacrificedStat;
    [Range(0, 1)]
    public float PercentSacrificed;

    [Header("Recruit")]
    public Character Recruit;
    public Brain PursuingEnemy;

    public virtual void Initialize()
    {
        // meant to be overwritten
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
}
