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

    [Tooltip("0: bottom left, 1: bottom right, 2: top left, 3: top right")]
    public int DisplayQuadrant = 0;
}
