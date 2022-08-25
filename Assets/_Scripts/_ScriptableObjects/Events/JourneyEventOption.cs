using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Journey/EventOption")]
public class JourneyEventOption : BaseScriptableObject
{
    public string Text;
    public string Response;
    public JourneyNodeReward Reward;
    public Sound ResponseVoiceOver;

}
