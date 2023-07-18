using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Troops")]
public class StoreyTroops : Storey
{
    public IntVariable CurrentLimit;

    [SerializeField] List<StoreyUpgrade> MaxTroopsTreeOriginals = new();
    public List<StoreyUpgrade> MaxTroopsTree = new();

    public int CurrentMaxTroopsLevel;

    public List<Creature> DeadTroops = new();

    public override void Initialize()
    {
        MaxTroopsTree = new();
        foreach (StoreyUpgrade u in MaxTroopsTreeOriginals)
        {
            StoreyUpgrade instance = Instantiate(u);
            MaxTroopsTree.Add(instance);
        }

        CurrentLimit = ScriptableObject.CreateInstance<IntVariable>();
        CurrentLimit.SetValue(MaxTroopsTree[CurrentMaxTroopsLevel].Value);

        base.Initialize();
    }

}
