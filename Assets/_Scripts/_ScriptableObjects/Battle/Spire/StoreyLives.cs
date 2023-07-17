using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Lives")]
public class StoreyLives : Storey
{
    public IntVariable CurrentLives;

    public List<StoreyUpgrade> MaxLivesTree = new();
    public int CurrentMaxLivesLevel;

    public StoreyUpgrade RestoreLivesTree;

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
