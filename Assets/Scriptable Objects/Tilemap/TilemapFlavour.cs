using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tilemap/Flavour")]

public class TilemapFlavour : BaseScriptableObject
{
    public TileBase[] floorTiles;
    public TileBase[] floorAdditions;
    public TileBase[] outerTiles;

    public TileBase edgeN;
    public TileBase edgeS;
    public TileBase edgeE;
    public TileBase edgeW;
 
    public TileBase cornerNE;
    public TileBase cornerNW;
    public TileBase cornerSE;
    public TileBase cornerSW;
 
    // TODO: temp, dunno how to deal with this...
    public TilemapObject[] objects;

}
