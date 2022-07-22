using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Cutscene/Cutscene")]
public class Cutscene : BaseScriptableObject
{
    public Sprite Picture;
    [TextArea(2, 5)]
    public string Text;

    [Tooltip("Duration in seconds.")]
    public int DisplayDuration;
    public Vector2 CameraPanDirection;
    public bool ZoomCameraIn;

}
