using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Conversations/Line")]
public class Line : BaseScriptableObject
{
	public Character SpeakerCharacter;
	[TextArea(2,5)]
	public string Text;
	public AudioClip Clip;
}
