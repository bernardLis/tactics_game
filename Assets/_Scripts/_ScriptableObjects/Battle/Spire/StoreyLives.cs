using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Lives")]
public class StoreyLives : Storey
{
    public IntVariable CurrentLives;

    public StoreyUpgradeTree MaxLivesTree;
    public StoreyUpgrade RestoreLivesUpgrade;

    public override void Initialize()
    {

        MaxLivesTree.Initialize();
        CurrentLives = ScriptableObject.CreateInstance<IntVariable>();
        CurrentLives.SetValue(MaxLivesTree.CurrentValue.Value);

        base.Initialize();
    }

    public void RestoreLives(int number)
    {
        int newLives = CurrentLives.Value + number;
        newLives = Mathf.Clamp(newLives, 0, MaxLivesTree.CurrentValue.Value);
        CurrentLives.SetValue(newLives);
    }
}
