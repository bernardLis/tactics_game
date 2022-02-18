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

    public JourneyNodeReward Won()
    {
        sr.sprite = fullChest;

        reward = Instantiate(reward);
        reward.obols = Random.Range(3, 10);
        return reward;
    }

    public JourneyNodeReward Lost()
    {
        sr.sprite = emptyChest;

        reward = Instantiate(reward);
        reward.obols = 0;
        return reward;
    }

}
