using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObject/Tilemap/Biome")]
public class TilemapBiome : BaseScriptableObject
{
    [Header("Tiles")]
    public TileBase[] FloorTiles;
    public TileBase[] FloorAdditions;
    public TileBase[] OuterTiles;

    [Header("Objects")]
    public TilemapObject[] Obstacles;
    public TilemapObject[] OuterAdditions;

    [Tooltip("min, max")]
    public Vector2 OuterAdditionsPercent;

    [Header("Edge")]
    public TileBase EdgeN;
    public TileBase EdgeS;
    public TileBase EdgeE;
    public TileBase EdgeW;

    [Header("Corner")]
    public TileBase CornerNE;
    public TileBase CornerNW;
    public TileBase CornerSE;
    public TileBase CornerSW;

    [Header("Inland Corner")]
    public TileBase InlandCornerNE;
    public TileBase InlandCornerNW;
    public TileBase InlandCornerSE;
    public TileBase InlandCornerSW;

    [Header("Global light")]
    public Color LightColor;
    public float LightIntensity;

    [Header("Audio")]
    public Sound Music;
    public Sound Ambience;


}
