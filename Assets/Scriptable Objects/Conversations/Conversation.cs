using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conversations/Conversation")]
public class Conversation : ScriptableObject
{
	public string cName = "New Conversation";
	public string cDescription = "New Description";
	public Line[] cLines;
}
