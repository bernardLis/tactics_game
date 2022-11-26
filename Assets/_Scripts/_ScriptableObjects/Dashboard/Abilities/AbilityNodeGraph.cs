using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Ability Node Graph")]
public class AbilityNodeGraph : BaseScriptableObject
{
    public string Title;
    public AbilityNode[] AbilityNodes;

    public void ResetNodes()
    {
        foreach (AbilityNode n in AbilityNodes)
            n.IsUnlocked = false;
    }

    public void LoadFromData(AbilityNodeGraphData data)
    {

    }

    public AbilityNodeGraphData SerializeSelf()
    {

        AbilityNodeGraphData data = new();

        data.Id = Id;
        data.AbilityNodeDatas = new();
        foreach (AbilityNode a in AbilityNodes)
        {
            AbilityNodeData d = a.SerializeSelf();
            data.AbilityNodeDatas.Add(d);
        }

        return data;
    }


}

[Serializable]
public struct AbilityNodeGraphData
{
    public string Id;
    public List<AbilityNodeData> AbilityNodeDatas;
}
