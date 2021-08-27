using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://www.youtube.com/watch?v=XIqtZnqutGg
[CreateAssetMenu]
public class MyTileData : ScriptableObject
{
	public TileBase[] tiles;

	public bool obstacle;

	public int tileDamage;
	
}
