using UnityEngine;

public enum TileMapObjectType { Outer, Obstacle, Scene }

[CreateAssetMenu(menuName = "Tilemap/Object")]
public class TilemapObject : BaseScriptableObject
{
    public string oName;
    [Tooltip("Number of tiles, like: 2, 4.")]
    public Vector2Int size; // when it comes to scene, it's tiles that are a joint\
    public TileMapObjectType objectType;
    public Sprite sprite;
    public bool pushable;
}
