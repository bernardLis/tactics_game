using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene Picture")]
public class CutscenePicture : BaseScriptableObject
{
    public Sprite Picture;
    [TextArea(2, 5)]
    public string Text;

    public Vector2 CameraPanDirection;
    public bool ZoomCameraIn;

    public Sound TextToSpeech;

    [Tooltip("In seconds. Duration of text to speech takes precedent")]
    public float Duration;

}
