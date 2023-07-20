using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Upgrade Tree")]
public class StoreyUpgradeTree : BaseScriptableObject
{
    public IntVariable CurrentValue;
    [SerializeField] List<StoreyUpgrade> OriginalNodes = new();
    public List<StoreyUpgrade> Nodes = new();
    public int CurrentNodeIndex;

    public void Initialize()
    {
        Nodes = new();
        foreach (StoreyUpgrade u in OriginalNodes)
        {
            StoreyUpgrade instance = Instantiate(u);
            Nodes.Add(instance);
        }
        CurrentValue = ScriptableObject.CreateInstance<IntVariable>();
        CurrentValue.SetValue(Nodes[CurrentNodeIndex].Value);
    }
}
