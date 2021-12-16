using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conversations/Line")]
public class Line : BaseScriptableObject
{
	public Character character;
	[TextArea(2,5)]
	public string text;
	public AudioClip clip;
}
