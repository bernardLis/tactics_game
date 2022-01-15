using UnityEngine;

public enum TileMapObjectType { Outer, Obstacle, Walkable }

[CreateAssetMenu(menuName = "Tilemap/Object")]
public class TilemapObject : BaseScriptableObject
{
    public string oName;
    [Tooltip("Number of tiles, like: 2, 4.")]
    public Vector2Int size;
    public TileMapObjectType objectType;
    public Sprite sprite;
    public bool pushable;
}
