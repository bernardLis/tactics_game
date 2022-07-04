using UnityEngine;
using UnityEngine.Tilemaps;

//https://www.youtube.com/watch?v=XIqtZnqutGg
[CreateAssetMenu(menuName = "ScriptableObject/Tilemap/Biome")]
public class MyTileData : BaseScriptableObject
{
	public TileBase[] Tiles;

	public bool isObstacle;
	
}
