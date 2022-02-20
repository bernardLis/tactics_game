using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Journey/KnowledgeEventOption")]
public class KnowledgeEventOption : BaseScriptableObject
{
    public Sprite sprite;
    public Vector2 miniatureSpriteSize; // TODO: probably should be aspect ratio or somehting
    public string text;
    [Tooltip("It's either state (being alive) or transition (dying)")]
    public bool state; 
	
}
