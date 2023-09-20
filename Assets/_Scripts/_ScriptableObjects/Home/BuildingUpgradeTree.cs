using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building Upgrade Tree")]
public class BuildingUpgradeTree : BaseScriptableObject
{
    [HideInInspector] public IntVariable CurrentValue;
    [SerializeField] List<BuildingUpgrade> OriginalNodes = new();
    [HideInInspector] public List<BuildingUpgrade> Nodes = new();
    public int CurrentNodeIndex;

    public void Initialize()
    {
        CurrentNodeIndex = 0;

        Nodes = new();
        foreach (BuildingUpgrade u in OriginalNodes)
        {
            BuildingUpgrade instance = Instantiate(u);
            Nodes.Add(instance);
        }
        CurrentValue = ScriptableObject.CreateInstance<IntVariable>();
        CurrentValue.SetValue(Nodes[CurrentNodeIndex].Value);
    }

    public BuildingUpgrade GetCurrentNode()
    {
        return Nodes[CurrentNodeIndex];
    }
}
