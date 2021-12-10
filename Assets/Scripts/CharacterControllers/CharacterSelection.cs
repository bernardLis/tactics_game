using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterSelection : MonoBehaviour
{
    protected Highlighter highlighter;

    // https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    protected Tilemap tilemap;
    protected WorldTile _tile;
    protected Dictionary<Vector3, WorldTile> tiles;

    // local
    protected CharacterStats myStats;

    [Header("after rewritting enemies you can get rid of range")]
    public int range;

    public virtual void Awake()
    {
        highlighter = GameManager.instance.GetComponent<Highlighter>();

        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

        myStats = GetComponent<CharacterStats>();
    }

    public virtual void FinishCharacterTurn()
    {
        myStats.SetAttacker(false);
    }
}
