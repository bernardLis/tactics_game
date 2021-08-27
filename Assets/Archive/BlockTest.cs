using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class BlockTest : MonoBehaviour
{
	//https://arongranberg.com/astar/docs/turnbased.html
	public void Start () {
		var blocker = GetComponent<SingleNodeBlocker>();

		blocker.BlockAtCurrentPosition();
	}
}
