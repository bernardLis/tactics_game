using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Troops")]
public class StoreyTroops : Storey
{
    public IntVariable CurrentLimit;

    public List<StoreyUpgrade> MaxTroopsTree = new();
    public int CurrentMaxTroopsLevel;

    public List<Creature> DeadTroops = new();

    public override void Initialize()
    {
        CurrentLimit = ScriptableObject.CreateInstance<IntVariable>();
        CurrentLimit.SetValue(MaxTroopsTree[CurrentMaxTroopsLevel].Value);

        base.Initialize();
    }

}
