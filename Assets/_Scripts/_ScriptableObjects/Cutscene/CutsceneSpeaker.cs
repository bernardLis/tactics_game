using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene Speaker")]
public class CutsceneSpeaker : BaseScriptableObject
{
    public bool PlayerIsSpeaker;

    public Hero SpeakerHero;

    [Tooltip("0 - left, 1 - middle, 2 right | 0 - up, 1 - middle, 2 - right")]
    public Vector2 SpeakerPosition;

    public void Initialize()
    {
        GameManager gm = GameManager.Instance;
        if (PlayerIsSpeaker)
            SpeakerHero = gm.PlayerHero;
    }
}
