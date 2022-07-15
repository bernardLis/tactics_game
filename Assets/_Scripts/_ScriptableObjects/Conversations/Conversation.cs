using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Conversations/Conversation")]
public class Conversation : BaseScriptableObject
{
	public bool Seen = false;
	public Line[] Lines;

	public void EndConversation()
	{
		Seen = true;
	}
}
