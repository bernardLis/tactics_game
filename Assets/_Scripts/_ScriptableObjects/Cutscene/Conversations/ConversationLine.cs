using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Conversation Line")]
public class ConversationLine : BaseScriptableObject
{
    public bool PlayerIsSpeaker;
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

        ParsedText = Text.Replace("{name}", gm.PlayerHero.HeroName);
    }
}
