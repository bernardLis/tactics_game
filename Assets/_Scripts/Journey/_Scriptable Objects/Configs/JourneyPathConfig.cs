using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/PathConfig")]
public class JourneyPathConfig : BaseScriptableObject
{
    [Tooltip("inclusive, exclusive")]
    public Vector2 NodeIndexRange;
    public JourneyNode[] Nodes;
    [Range(0, 1)]
    public float ChanceToIgnore;
}
