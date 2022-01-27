using UnityEngine;

public enum TileMapObjectType { Outer, Obstacle }

[CreateAssetMenu(menuName = "Tilemap/Object")]
public class TilemapObject : BaseScriptableObject
{
    [Tooltip("Number of tiles, like: 1, 2.")]
    public Vector2Int size;
    public TileMapObjectType objectType;
    public Sprite sprite;
    public bool pushable;
    public bool isMoving;

    [Header("Light")]
    public bool emitsLight;
    public Color lightColor;
    [Tooltip("Min, max")]
    public Vector2 lightIntensity;
    [Tooltip("Min, max")]
    public Vector2 innerRadius;
    [Tooltip("Min, max")]
    public Vector2 outerRadius;
    public Vector2 offset;
}
