using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Event")]

public class JourneyEvent : BaseScriptableObject
{
    public Sprite Background;
    public string Description;
    public List<JourneyEventOption> Options = new();
	
}
