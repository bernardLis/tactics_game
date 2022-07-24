using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene")]
public class Cutscene : BaseScriptableObject
{
    public CutscenePicture[] Pictures;
    public string NextLevelName;
    public Sound Music;
}
