using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tilemap/Flavour")]

public class TilemapFlavour : BaseScriptableObject
{
    [Header("Tiles")]
    public TileBase[] floorTiles;
    public TileBase[] floorAdditions;
    public TileBase[] outerTiles;

    [Header("Objects")]
    public TilemapObject[] obstacles;
    public TilemapObject[] outerAdditions;

    [Tooltip("min, max")]
    public Vector2 outerAdditionsPercent;

    [Header("Edge")]
    public TileBase edgeN;
    public TileBase edgeS;
    public TileBase edgeE;
    public TileBase edgeW;

    [Header("Corner")]
    public TileBase cornerNE;
    public TileBase cornerNW;
    public TileBase cornerSW;
    public TileBase cornerSE;

    [Header("Inland Corner")]
    public TileBase inlandCornerNE;
    public TileBase inlandCornerNW;
    public TileBase inlandCornerSE;
    public TileBase inlandCornerSW;

    [Header("Global light")]
    public Color lightColor;
    public float lightIntensity;

}
