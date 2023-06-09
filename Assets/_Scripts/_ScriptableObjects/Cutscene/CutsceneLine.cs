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
        string p = Text;
        if (gm.PlayerHero != null)
            p = Text.Replace("{heroName}", gm.PlayerHero.HeroName);
        if (gm.OverseerHero != null)
            p = p.Replace("{overseerName}", gm.OverseerHero.HeroName);
        if (gm.RivalHero != null)
            p = p.Replace("{rivalName}", gm.RivalHero.HeroName);

        ParsedText = p;

    }
}
