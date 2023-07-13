using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Base/Base Upgrade Lives")]
public class BaseUpgradeLives : BaseUpgrade
{
    public IntVariable CurrentLives;

    public List<BaseUpgradeLevel> MaxLivesTree = new();
    public int CurrentMaxLivesLevel;

    public BaseUpgradeLevel RestoreLives;

    public override void Initialize()
    {
        CurrentLives = ScriptableObject.CreateInstance<IntVariable>();
        CurrentLives.SetValue(MaxLivesTree[CurrentMaxLivesLevel].Value);
        Debug.Log($"intialize lives {CurrentLives.Value}");

        base.Initialize();
    }
}
