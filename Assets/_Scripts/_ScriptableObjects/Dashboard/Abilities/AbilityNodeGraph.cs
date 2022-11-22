using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Ability Node Graph")]
public class AbilityNodeGraph : BaseScriptableObject
{
    public string Title;
    public AbilityNode[] AbilityNodes;
}
