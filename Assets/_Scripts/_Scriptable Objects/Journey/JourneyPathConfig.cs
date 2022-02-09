using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/PathConfig")]
public class JourneyPathConfig : BaseScriptableObject
{
    [Tooltip("inclusive, exclusive")]
    public Vector2 nodeIndexRange;
    public JourneyNode node;
    [Range(0, 1)]
    public float chanceToIgnore;
}
