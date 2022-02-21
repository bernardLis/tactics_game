using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/Event")]

public class JourneyEvent : BaseScriptableObject
{
    public Sprite background;
    public string description;
    public List<JourneyEventOption> options = new();
	
}
