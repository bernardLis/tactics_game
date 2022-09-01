using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Tilemap/Object")]
public class TilemapObject : BaseScriptableObject
{
    [Tooltip("Number of tiles, like: 1, 2.")]
    public Vector2Int Size;
    public TileMapObjectType ObjectType;
    public Sprite Sprite;
    public bool IsPushable;
    public bool IsMoving;

    [Header("Light")]
    public bool IsEmitingLight;
    public Color LightColor;
    [Tooltip("Min, max")]
    public Vector2 LightIntensity;
    [Tooltip("Min, max")]
    public Vector2 InnerRadius;
    [Tooltip("Min, max")]
    public Vector2 OuterRadius;
    public Vector2 Offset;
}
