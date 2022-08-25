using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward")]
public class JourneyNodeReward : BaseScriptableObject
{
    protected GameManager _gameManager;
    protected RunManager _runManager;
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
        _runManager = RunManager.Instance;

        if (Obols != 0)
            _gameManager.ChangeObolValue(Obols);
        if (Gold != 0)
            _runManager.ChangeGoldValue(Gold);
        if (Item != null)
            AddItem();

        if (PercentSacrificed != 0)
            HandleSacrifice();

        if (Recruit != null)
            HandleRecruit();
    }

    void AddItem()
    {
        if (_runManager.PlayerItemPouch.Count < 3) // TODO: hardcoded value
        {
            _runManager.AddItemToPouch(Item);
            return;
        }

        // TODO: this is very crude
        foreach (Character character in _runManager.PlayerTroops)
        {
            if (character.Items.Count < 3) // TODO: hardcoded value
            {
                character.AddItem(Item);
                return;
            }
        }
    }

    void HandleSacrifice()
    {
        foreach (Character c in _runManager.PlayerTroops)
        {
            int newStatVal = Mathf.CeilToInt(c.GetStatValue(SacrificedStat.ToString()) * (1 - PercentSacrificed));
            c.ChangeStat(SacrificedStat.ToString(), newStatVal);
        }

    }

    void HandleRecruit()
    {
        _runManager.AddCharacterToTroops(Recruit);
        _runManager.AddEnemyToAllBattleNodes(PursuingEnemy);
    }
}
