using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Event/Event")]

public class JourneyEvent : BaseScriptableObject
{
    public Sprite Background;
    [TextArea]
    public string Description;
    public Sound VoiceOver;
    public List<EventOption> Options = new();

}
