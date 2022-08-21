using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward")]
public class JourneyNodeReward : BaseScriptableObject
{
    RunManager _runManager;

    public int Gold;
    public Item Item;

    public virtual void Initialize()
    {
        // meant to be overwritten
    }

    public void GetReward()
    {
        _runManager = RunManager.Instance;
        if (Gold != 0)
            _runManager.ChangeGoldValue(Gold);

        if (Item != null)
            AddItem();
    }

    void AddItem()
    {
        // TODO: this is very crude
        foreach (Character character in _runManager.PlayerTroops)
        {
            if (character.Items.Count < 3) // TODO: hardcoded value
            {
                character.AddItem(Item);
                return;
            }
        }

        if (_runManager.PlayerItemPouch.Count < 3) // TODO: hardcoded value
            _runManager.AddItemToPouch(Item);

    }
}
