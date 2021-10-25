using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    Tilemap tileMap;

    [SerializeField]
    List<MyTileData> myTileDatas;

    Dictionary<TileBase, MyTileData> dataFromTiles;

    [Header("Enemy placement")]
    public Character enemyCharSO;
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
        PlaceEnemies();
    }

    // TODO: this should be done by map generator
    void PlaceEnemies()
    {
        // TODO: randomized smart enemy placement
        // for now I am going to place enemies "by hand"
        Vector3[] enemyPositions = new Vector3[3];
        enemyPositions[0] = new Vector3(6.5f, -3.5f, 0f);
        enemyPositions[1] = new Vector3(3.5f, -3.5f, 0f);
        enemyPositions[2] = new Vector3(3.5f, -5.5f, 0f);

        for (int i = 0; i < 3; i++)
        {
            GameObject newCharacter = Instantiate(enemyTemplateGO, enemyPositions[i], Quaternion.identity);
            Character instantiatedSO = Instantiate(enemyCharSO);
            instantiatedSO.Initialize(newCharacter);
            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

            // face right 
            // TODO: face player => center of designated player spawn location - this spawn location
            CharacterRendererManager cr = newCharacter.GetComponentInChildren<CharacterRendererManager>();
            Debug.Log(cr);
            cr.Face(Vector2.right);
        }
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
