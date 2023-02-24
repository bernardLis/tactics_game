using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation")]
public class Conversation : BaseScriptableObject
{
    public ConversationLine[] Lines;

    public void Initialize()
    {
        foreach (ConversationLine l in Lines)
            l.Initialize();
    }
}
