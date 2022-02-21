using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile
{
    public Vector3Int LocalPlace { get; set; }
    public Vector3 WorldLocation { get; set; }
    public TileBase TileBase { get; set; }
    public Tilemap TilemapMember { get; set; }
    public string Name { get; set; }

    public int Cost { get; set; }
    
    public bool IsObstacle { get; set; }
    public bool WithinRange { get; set; }

    public void Highlight(Color col)
    {
        TilemapMember.SetTileFlags(LocalPlace, TileFlags.None);
        TilemapMember.SetColor(LocalPlace, col);
    }

    public void ClearHighlightAndTags()
    {
        TilemapMember.SetTileFlags(LocalPlace, TileFlags.None);
        TilemapMember.SetColor(LocalPlace, Color.white);
        WithinRange = false;
    }

    public Vector3 GetMiddleOfTile()
    {
        return new Vector3(LocalPlace.x + 0.5f, LocalPlace.y + 0.5f, LocalPlace.z);
    }

}
