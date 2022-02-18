using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Chest")]
public class JourneyChest : BaseScriptableObject
{
    public Sprite closedChest;
    public Sprite emptyChest;
    public Sprite fullChest;

    public JourneyNodeReward reward;

    [Range(0, 1)]
    public float chanceToBeEmpty;

    JourneyManager journeyManager;

    public GameObject gameObject;
    SpriteRenderer sr;

    public void Initialize(GameObject _obj)
    {
        gameObject = _obj;
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = closedChest;

        journeyManager = JourneyManager.instance;
    }

    public void Select()
    {
        if (Random.Range(0f, 1f) < chanceToBeEmpty)
            ChestIsEmpty();
        else
            ChestIsFull();
    }

    void ChestIsEmpty()
    {
        sr.sprite = emptyChest;

        reward = Instantiate(reward);
        reward.obols = 0;
        journeyManager.SetNodeReward(reward); // TODO: dunno if necessary
    }

    void ChestIsFull()
    {
        sr.sprite = fullChest;

        reward = Instantiate(reward);
        reward.obols = Random.Range(3, 10);
        journeyManager.SetNodeReward(reward);
    }

}
