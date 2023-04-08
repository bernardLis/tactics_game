using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation Line")]
public class ConversationLine : BaseScriptableObject
{
    public bool PlayerIsSpeaker;
    public bool FriendIsSpeaker;
    public Hero SpeakerHero;
    [TextArea(2, 5)]
    [SerializeField] string Text;
    [HideInInspector] public string ParsedText;
    public Sound VO;

    public void Initialize()
    {
        GameManager gm = GameManager.Instance;
        if (PlayerIsSpeaker)
            SpeakerHero = gm.PlayerHero;
        if (FriendIsSpeaker)
            SpeakerHero = gm.FriendHero;

        ParsedText = Text.Replace("{name}", gm.PlayerHero.HeroName);
        ParsedText = ParsedText.Replace("{friendName}", gm.FriendHero.HeroName);
    }
}
