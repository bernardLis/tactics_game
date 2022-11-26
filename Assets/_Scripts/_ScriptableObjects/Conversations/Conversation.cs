using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation")]
public class Conversation : BaseScriptableObject
{
	public bool Seen = false;
	public ConversationLine[] Lines;

	public void EndConversation()
	{
		Seen = true;
	}
}
