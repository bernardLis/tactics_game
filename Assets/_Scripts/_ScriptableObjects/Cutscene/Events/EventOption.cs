using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Event/EventOption")]
public class EventOption : BaseScriptableObject
{
    public string Text;
    public string Response;
    public Reward Reward;
    public Sound ResponseVoiceOver;

}
