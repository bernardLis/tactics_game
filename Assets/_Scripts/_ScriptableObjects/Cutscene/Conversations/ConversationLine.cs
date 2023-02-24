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
        GameManager gm = GameManager.Instance;
        if (Player)
            SpeakerCharacter = gm.PlayerCharacter;
        if (Friend)
            SpeakerCharacter = gm.FriendCharacter;

        ParsedText = Text.Replace("{name}", gm.PlayerCharacter.CharacterName);
        ParsedText = ParsedText.Replace("{friendName}", gm.FriendCharacter.CharacterName);
    }
}
