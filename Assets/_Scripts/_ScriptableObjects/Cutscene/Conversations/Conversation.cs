using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation")]
public class Conversation : BaseScriptableObject
{
	public ConversationLine[] Lines;
}
