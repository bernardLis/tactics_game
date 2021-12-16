using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class MapManager : MonoBehaviour
{
    Tilemap tileMap;

    [SerializeField]
    List<MyTileData> myTileDatas;

    Dictionary<TileBase, MyTileData> dataFromTiles;

    [Header("Enemy placement")]
    public Character meeleEnemySO;
    public Character healerEnemySO;

    public GameObject enemyTemplateGO;

    public static MapManager instance;

    void Awake()
    {
        tileMap = TileMapInstance.instance.GetComponent<Tilemap>();

        // singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        dataFromTiles = new Dictionary<TileBase, MyTileData>();
        foreach (var tileData in myTileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    void Start()
    {
        // TODO: this should be done by map generator
        PlaceEnemies();
        CreatePlayerStartingArea();
    }

    // TODO: this should be done by map generator
    void PlaceEnemies()
    {
        // TODO: randomized smart enemy placement
        // for now I am going to place enemies "by hand"
        Vector3[] enemyPositions = new Vector3[3];
        enemyPositions[0] = new Vector3(4.5f, -3.5f, 0f);
        enemyPositions[1] = new Vector3(4.5f, -1.5f, 0f);
        enemyPositions[2] = new Vector3(1.5f, -6.5f, 0f);

        for (int i = 0; i < 3; i++)
        {
            Character pickedChar = meeleEnemySO;
            if (i == 1)
                pickedChar = healerEnemySO;

            GameObject newCharacter = Instantiate(enemyTemplateGO, enemyPositions[i], Quaternion.identity);
            Character instantiatedSO = Instantiate(pickedChar);

            instantiatedSO.Initialize(newCharacter);
            newCharacter.name = instantiatedSO.characterName;

            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

            // face right
            CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();
            characterRendererManager.Face(Vector2.right);
            characterRendererManager.Face(Vector2.zero);
        }
    }


    void CreatePlayerStartingArea()
    {
        // TODO: this is temporary
        Vector2 SWCorner = new Vector2(19.5f, -6.5f);
        Highlighter.instance.HighlightRectanglePlayer(SWCorner, 5, 5, Color.blue);
    }

    public bool IsObstacle(Vector3Int pos)
    {
        TileBase tile = tileMap.GetTile(pos);
        if (tile == null)
            return false;
        if (!dataFromTiles.ContainsKey(tile))
            return false;

        return dataFromTiles[tile].obstacle;
    }

    public int GetTileDamage(Vector3Int pos)
    {
        TileBase tile = tileMap.GetTile(pos);
        if (tile == null)
            return 0;

        return dataFromTiles[tile].tileDamage;
    }
}
