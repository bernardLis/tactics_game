using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward")]
public class JourneyNodeReward : BaseScriptableObject
{
    protected GameManager _gameManager;
    protected RunManager _runManager;
    public int Obols;
    public int Gold;
    public Item Item;

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
}
