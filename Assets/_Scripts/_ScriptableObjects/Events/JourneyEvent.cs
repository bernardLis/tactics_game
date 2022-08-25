using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Event")]

public class JourneyEvent : BaseScriptableObject
{
    public Sprite Background;
    [TextArea]
    public string Description;
    public Sound VoiceOver;
    public List<JourneyEventOption> Options = new();

}
