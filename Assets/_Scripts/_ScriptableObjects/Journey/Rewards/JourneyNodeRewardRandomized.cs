using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward Randomized")]
public class JourneyNodeRewardRandomized : JourneyNodeReward
{
    public Vector2Int GoldRange;
    public bool hasItem;

    public override void Initialize()
    {
        // meant to be overwritten
        Gold = Random.Range(GoldRange.x, GoldRange.y);
        Debug.Log($"initialize reward gold: {Gold}");

        Item = GameManager.Instance.GameDatabase.GetRandomItem();
    }

}
