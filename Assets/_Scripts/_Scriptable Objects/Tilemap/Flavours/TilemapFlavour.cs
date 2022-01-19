using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tilemap/Flavour")]

public class TilemapFlavour : BaseScriptableObject
{
    public TileBase[] floorTiles;
    public TileBase[] floorAdditions;
    public TileBase[] outerTiles;

    [Header("Edge")]
    public TileBase edgeN;
    public TileBase edgeS;
    public TileBase edgeE;
    public TileBase edgeW;

    [Header("Corner")]
    public TileBase cornerNE;
    public TileBase cornerNW;
    public TileBase cornerSE;
    public TileBase cornerSW;

    [Header("Inland Corner")]
    public TileBase inlandCornerNE;
    public TileBase inlandCornerNW;
    public TileBase inlandCornerSE;
    public TileBase inlandCornerSW;

    public TilemapObject[] obstacles;
    public TilemapObject[] outerObjects;  
}
