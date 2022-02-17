using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Journey/EventOption")]
public class JourneyEventOption : BaseScriptableObject
{
    public string text;
    public JourneyNodeReward reward; // TODO: how do I store rewards? 
	
}
