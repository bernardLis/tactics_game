using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene Line")]
public class CutsceneLine : BaseScriptableObject
{
    public CutsceneSpeaker Speaker;
    [TextArea(2, 5)]
    [SerializeField] string Text;
    [HideInInspector] public string ParsedText;
    public Sound VO;

    public void Initialize()
    {
        Speaker.Initialize();

        GameManager gm = GameManager.Instance;
        ParsedText = Text.Replace("{heroName}", gm.PlayerHero.HeroName);
        ParsedText = Text.Replace("{overseerName}", gm.OverseerHero.HeroName);
        ParsedText = Text.Replace("{rivalName}", gm.RivalHero.HeroName);
    }
}
