using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene Line")]
public class CutsceneLine : BaseScriptableObject
{
    [TextArea(2, 5)]
    [SerializeField] string Text;
    [HideInInspector] public string ParsedText;
    public Sound VO;
    
    // TODO: maybe it makes more sense for having speaker position here, 
    // and then when it changes from line to line I can move the speaker it

    public void Initialize()
    {
        GameManager gm = GameManager.Instance;
        string p = Text;
        if (gm.PlayerHero != null)
            p = Text.Replace("{heroName}", gm.PlayerHero.EntityName);

        ParsedText = p;

    }
}
