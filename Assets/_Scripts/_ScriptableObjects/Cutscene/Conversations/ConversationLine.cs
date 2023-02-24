using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation Line")]
public class ConversationLine : BaseScriptableObject
{
    public bool Player;
    public bool Friend;
    public Character SpeakerCharacter;
    [TextArea(2, 5)]
    [SerializeField] string Text;
    [HideInInspector] public string ParsedText;
    public Sound VO;

    public void Initialize()
    {
        ParsedText = Text.Replace("{name}", GameManager.Instance.PlayerTroops[0].CharacterName);
        ParsedText = ParsedText.Replace("{friendName}", GameManager.Instance.PlayerTroops[1].CharacterName);
    }
}
