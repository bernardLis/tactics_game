using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Quest Rank")]
public class QuestRank : BaseScriptableObject
{
    public int Rank;
    public Sprite Icon;
    public string Description;
}
