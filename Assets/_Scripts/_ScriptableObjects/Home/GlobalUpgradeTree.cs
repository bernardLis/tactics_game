using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Global Upgrade Tree")]
public class GlobalUpgradeTree : BaseScriptableObject
{
    [HideInInspector] public IntVariable CurrentValue;
    [SerializeField] List<GlobalUpgradeLevel> OriginalNodes = new();
    [HideInInspector] public List<GlobalUpgradeLevel> Nodes = new();
    public int CurrentNodeIndex;

    public void Initialize()
    {
        CurrentNodeIndex = 0;

        Nodes = new();
        foreach (GlobalUpgradeLevel u in OriginalNodes)
        {
            GlobalUpgradeLevel instance = Instantiate(u);
            Nodes.Add(instance);
        }
        CurrentValue = ScriptableObject.CreateInstance<IntVariable>();
        CurrentValue.SetValue(Nodes[CurrentNodeIndex].Value);
    }

    public GlobalUpgradeLevel GetCurrentNode()
    {
        return Nodes[CurrentNodeIndex];
    }
}
