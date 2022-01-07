using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using DG.Tweening;

// https://learn.unity.com/tutorial/level-generation?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f6
public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int seed = 10;

    public Transform envObjects;

    public int mapSizeX = 10;
    public int mapSizeY = 10;

    public Tilemap backgroundTilemap;
    public Tilemap middlegroundTilemap;
    public Tilemap foregroundTilemap;

    public TilemapFlavour[] tilemapFlavours;

    // TODO: this could be derived from map size
    public Count floorAdditionsCount = new Count(3, 7);
    public Count stoneCount = new Count(2, 5);
    public Count trapCount = new Count(1, 2);

    // TODO: this
    public Count enemyCount = new Count(1, 3);

    public GameObject stone;
    public GameObject trap;

    public Character[] enemyCharacters;
    public GameObject enemyGO;

    List<Vector3> gridPositions = new();

    void ClearTilemaps()
    {
        backgroundTilemap.ClearAllTiles();
        middlegroundTilemap.ClearAllTiles();
        foregroundTilemap.ClearAllTiles();
    }

    void InitaliseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < mapSizeX - 1; x++)
            for (int y = 1; y < mapSizeY - 1; y++) // -1 to leave outer ring of tiles free 
                gridPositions.Add(new Vector3(x, y, 0f));
    }

    void BoardSetup(TilemapFlavour _flav)
    {
        TileBase[] floorTiles = _flav.floorTiles;

        for (int x = -1; x < mapSizeX + 1; x++)
        {
            for (int y = -1; y < mapSizeY + 1; y++) // +-1 to create an edge;
            {
                backgroundTilemap.SetTile(new Vector3Int(x, y), floorTiles[Random.Range(0, floorTiles.Length)]);

                // tiles are overwritten in the process

                // edge
                if (x == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.edgeW);
                if (x == mapSizeX)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.edgeE);
                if (y == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.edgeS);
                if (y == mapSizeY)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.edgeN);

                // corners
                if (x == -1 && y == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.cornerSE);
                if (x == mapSizeX && y == -1)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.cornerSW);
                if (x == -1 && y == mapSizeY)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.cornerNW);
                if (x == mapSizeX && y == mapSizeY)
                    backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.cornerNE);
            }
        }
    }

    void OuterSetup(TilemapFlavour _flav)
    {
        int outerX = mapSizeX * 3;
        int outerY = mapSizeY * 3;
        outerX = Mathf.Clamp(outerX, 30, 10000);
        outerY = Mathf.Clamp(outerY, 30, 10000);

        for (int x = -outerX; x < outerX; x++)
            for (int y = -outerY; y < outerY; y++)
                backgroundTilemap.SetTile(new Vector3Int(x, y), _flav.outerTile);
    }

    Vector3 GetRandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Debug.Log("randomIndex: " + randomIndex);

        Vector3 randomPosition = gridPositions[randomIndex];

        gridPositions.RemoveAt(randomIndex); // only one thing can occupy a position
        Debug.Log("randomPosition: " + randomPosition);

        return randomPosition;
    }
    void ClearObjects()
    {
        foreach (Transform t in envObjects)
            DestroyImmediate(t.gameObject); // TODO: use Destory instead, this is for editor
    }

    void LayoutObjectAtRandom(GameObject obj, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            if (gridPositions.Count <= 0)
                return;

            Vector3 randomPosiiton = GetRandomPosition();
            GameObject ob = Instantiate(obj, new Vector3(randomPosiiton.x + 0.5f, randomPosiiton.y + 0.5f, randomPosiiton.z), Quaternion.identity);
            ob.transform.parent = envObjects;
        }
    }
    void LayoutFloorAdditions(TilemapFlavour _flav, int minimum, int maximum)
    {
        TileBase[] tiles = _flav.floorAdditions;
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            if (gridPositions.Count <= 0)
                return;

            Vector3 randomPosiiton = GetRandomPosition();
            middlegroundTilemap.SetTile(new Vector3Int((int)randomPosiiton.x, (int)randomPosiiton.y),
                             tiles[Random.Range(0, tiles.Length)]);
        }

    }

    void SpawnEnemies(int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Character instantiatedSO = Instantiate(enemyCharacters[Random.Range(0, enemyCharacters.Length)]);
            GameObject newCharacter = Instantiate(enemyGO, GetRandomPosition(), Quaternion.identity); // TODO: get random posiiton is wrong here

            instantiatedSO.Initialize(newCharacter);
            newCharacter.name = instantiatedSO.characterName;

            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

            // face right
            CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();
            characterRendererManager.Face(Vector2.right);
            characterRendererManager.Face(Vector2.zero);
        }
    }

    void PlaceSpecialObject(TilemapFlavour _flav)
    {
        // TODO: this should be way smarter
        TilemapObject ob = _flav.objects[Random.Range(0, _flav.objects.Length)];

        // create a game object with sprite renderer compotenent and set correct layer
        GameObject n = new GameObject(ob.oName);
        n.transform.parent = envObjects;

        float y = mapSizeY + ob.size.y;
        float x = mapSizeX / 2;
        n.transform.position = new Vector3(x, y, 0f);

        n.transform.DOPunchPosition(Vector3.up*0.5f, 2f, 0, 1f, false).SetLoops(-1, LoopType.Yoyo); // TODO: cool! 

        SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 1;
        sr.sprite = ob.sprite;
    }

    [ContextMenu("SetupScene")]
    public void SetupScene()
    {
        Random.InitState(seed);

        // TODO: choose map flavour
        TilemapFlavour flav = tilemapFlavours[Random.Range(0, tilemapFlavours.Length)];

        ClearTilemaps();
        OuterSetup(flav);
        BoardSetup(flav);
        InitaliseList();
        ClearObjects();
        LayoutObjectAtRandom(stone, stoneCount.minimum, stoneCount.maximum);
        LayoutObjectAtRandom(trap, trapCount.minimum, trapCount.maximum);
        LayoutFloorAdditions(flav, floorAdditionsCount.minimum, floorAdditionsCount.maximum);

        // TODO: this should be way smarter
        PlaceSpecialObject(flav);

        // TODO: spawn player chars / set-up player spawn positions;
    }

}
