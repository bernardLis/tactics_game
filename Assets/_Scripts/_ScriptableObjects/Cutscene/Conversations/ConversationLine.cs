using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation Line")]
public class ConversationLine : BaseScriptableObject
{
    public bool Player;
    public bool Friend;
	public Character SpeakerCharacter;
	[TextArea(2,5)]
	public string Text;
	public Sound VO;

    [Tooltip("0: bottom left, 1: bottom right, 2: top left, 3: top right")]
    public int DisplayQuadrant = 0;
}
