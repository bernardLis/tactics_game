using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Base/Base Upgrade Lives")]
public class BaseUpgradeLives : BaseUpgrade
{
    public IntVariable CurrentLives;

    public List<BaseUpgradeLevel> MaxLivesTree = new();
    public int CurrentMaxLivesLevel;

    public BaseUpgradeLevel RestoreLivesTree;

    public override void Initialize()
    {
        CurrentLives = ScriptableObject.CreateInstance<IntVariable>();
        CurrentLives.SetValue(MaxLivesTree[CurrentMaxLivesLevel].Value);

        base.Initialize();
    }

    public void RestoreLives(int number)
    {
        Mathf.Clamp(number, 0, MaxLivesTree[CurrentMaxLivesLevel].Value - CurrentLives.Value);
        CurrentLives.SetValue(CurrentLives.Value + number);

    }
}
