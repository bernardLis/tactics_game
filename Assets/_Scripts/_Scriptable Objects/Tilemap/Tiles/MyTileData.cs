using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://www.youtube.com/watch?v=XIqtZnqutGg
[CreateAssetMenu(menuName = "ScriptableObject/Tilemap/Biome")]
public class MyTileData : BaseScriptableObject
{
	public TileBase[] tiles;

	public bool obstacle;
	
}
