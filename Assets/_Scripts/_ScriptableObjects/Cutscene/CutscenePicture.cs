using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/CutscenePicture")]
public class CutscenePicture : BaseScriptableObject
{
    public Sprite Picture;
    [TextArea(2, 5)]
    public string Text;

    public Vector2 CameraPanDirection;
    public bool ZoomCameraIn;

    public Sound TextToSpeech;

}
