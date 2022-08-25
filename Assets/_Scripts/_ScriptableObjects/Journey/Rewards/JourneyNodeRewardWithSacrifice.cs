using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Reward With Sacrifice")]
public class JourneyNodeRewardWithSacrifice : JourneyNodeReward
{
    public StatType SacrificedStat;
    [Range(0, 1)]
    public float PercentSacrificed;

    public override void GetReward()
    {
        base.GetReward();

        foreach (Character c in _runManager.PlayerTroops)
        {
            int newStatVal = Mathf.CeilToInt(c.GetStatValue(SacrificedStat.ToString()) * (1 - PercentSacrificed));
            c.ChangeStat(SacrificedStat.ToString(), newStatVal);
        }

    }



}
