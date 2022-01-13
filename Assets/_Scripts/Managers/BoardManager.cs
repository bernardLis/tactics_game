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
    [Header("Map Setup")]
    public string mapVariantChosen; // TODO: just for dev
    public int seed;
    public Vector2Int mapSize = new(20, 20);

    [Header("Enemies")]
    public Character[] enemyCharacters;
    public GameObject enemyGO;

    [Header("Unity objects")]
    public TilemapFlavour[] tilemapFlavours;
    public Tilemap backgroundTilemap;
    public Tilemap middlegroundTilemap;
    public Tilemap foregroundTilemap;
    public GameObject envObjectsHolder;
    public GameObject obstaclePrefab;
    public GameObject pushableObstaclePrefab;
    public GameObject trap;     // TODO: this

    // other map vars
    Vector3 emptyCorner;
    float obstaclePercent;
    List<Vector3> openGridPositions = new();
    int mapSizeSqm;
    TilemapFlavour flav;

    public void SetupScene()
    {
        InitialSetup();
        BoardSetup();
        InitialiseOpenPositions();

        // pick a map variant
        int mapVariant = Random.Range(1, 5);
        if (mapVariant == 1) // river in the middle
        {
            PlaceRiver();
            obstaclePercent = Random.Range(0f, 0.1f);
            mapVariantChosen = "River";
        }
        if (mapVariant == 2) // big space with some obstacles
        {
            obstaclePercent = Random.Range(0f, 0.1f);
            mapVariantChosen = "Space";
        }
        if (mapVariant == 3) // "labirynth"
        {
            obstaclePercent = 1f;
            mapVariantChosen = "Labirynth";
        }
        if (mapVariant == 4)
        {
            PlaceLakeInTheMiddle();
            obstaclePercent = Random.Range(0f, 0.1f);
            mapVariantChosen = "Lake in the middle";
        }

        // TODO: this should be way smarter
        PlaceSpecialObject();

        LayoutObstacles();
        //LayoutObjectAtRandom(trap, trapCount.minimum, trapCount.maximum);
        LayoutFloorAdditions(Mathf.RoundToInt(mapSizeSqm * 0.1f), Mathf.RoundToInt(mapSizeSqm * 0.2f));

        DrawOuter();

        // TODO: spawn player chars / set-up player spawn positions;
    }

    void InitialSetup()
    {
        mapSizeSqm = mapSize.x * mapSize.y;
        Random.InitState(seed);
        flav = tilemapFlavours[Random.Range(0, tilemapFlavours.Length)];

        backgroundTilemap.ClearAllTiles();
        middlegroundTilemap.ClearAllTiles();
        foregroundTilemap.ClearAllTiles();

        foreach (Transform child in envObjectsHolder.transform)
            DestroyImmediate(child.gameObject);

    }
    void DrawOuter()
    {
        // outer
        int outerX = mapSize.x * 3;
        int outerY = mapSize.y * 3;
        outerX = Mathf.Clamp(outerX, 30, 10000);
        outerY = Mathf.Clamp(outerY, 30, 10000);

        TileBase[] tiles = flav.outerTiles;

        for (int x = -outerX; x < outerX; x++)
            for (int y = -outerY; y < outerY; y++)
                if (!backgroundTilemap.GetTile(new Vector3Int(x, y)))
                    backgroundTilemap.SetTile(new Vector3Int(x, y), tiles[Random.Range(0, tiles.Length)]);
    }
    void BoardSetup()
    {
        // inner
        for (int x = -1; x < mapSize.x + 1; x++)
        {
            for (int y = -1; y < mapSize.y + 1; y++) // +-1 to create an edge;
            {
                TileBase selectedTile = flav.floorTiles[Random.Range(0, flav.floorTiles.Length)];

                // edge
                if (x == -1)
                    selectedTile = flav.edgeW;
                if (x == mapSize.x)
                    selectedTile = flav.edgeE;
                if (y == -1)
                    selectedTile = flav.edgeS;
                if (y == mapSize.y)
                    selectedTile = flav.edgeN;

                // corners
                if (x == -1 && y == -1)
                    selectedTile = flav.cornerSE;
                if (x == mapSize.x && y == -1)
                    selectedTile = flav.cornerSW;
                if (x == -1 && y == mapSize.y)
                    selectedTile = flav.cornerNW;
                if (x == mapSize.x && y == mapSize.y)
                    selectedTile = flav.cornerNE;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }
    }

    void InitialiseOpenPositions()
    {
        openGridPositions.Clear();

        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                openGridPositions.Add(new Vector3(x, y, 0f));

        // one of the corners is always empty TODO: better way of doing it? 
        int cornerChoice = Random.Range(1, 5);
        if (cornerChoice == 1)
            emptyCorner = Vector3.zero;
        if (cornerChoice == 2)
            emptyCorner = new Vector3(0f, mapSize.y - 1f, 0f);
        if (cornerChoice == 3)
            emptyCorner = new Vector3(mapSize.x - 1f, 0f, 0f);
        if (cornerChoice == 4)
            emptyCorner = new Vector3(mapSize.x - 1f, mapSize.y - 1f, 0f);

        openGridPositions.Remove(emptyCorner);

        openGridPositions = Utility.ShuffleList<Vector3>(openGridPositions, seed);
    }

    List<Vector3> GetRandomOpenPosition(Vector2 _size)
    {
        List<Vector3> candidatePositions = new();
        foreach (Vector3 pos in openGridPositions)
        {
            candidatePositions.Add(pos);
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    Vector3 posToCheck = new Vector3(pos.x - x, pos.y - y, 0f);
                    if (openGridPositions.Contains(posToCheck) && !candidatePositions.Contains(posToCheck))
                        candidatePositions.Add(posToCheck);
                }
            }

            if (candidatePositions.Count >= _size.x * _size.y)
            {
                foreach (Vector3 posToRemove in candidatePositions)
                    openGridPositions.Remove(posToRemove);

                return candidatePositions;
            }
            candidatePositions.Clear();
        }
        return null;
    }

    void LayoutObstacles()
    {
        bool[,] obstacleMap = new bool[mapSize.x, mapSize.y];

        int objectCount = (int)(mapSizeSqm * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            TilemapObject selectedObject = flav.obstacles[Random.Range(0, flav.obstacles.Length)];

            List<Vector3> randomPosition = GetRandomOpenPosition(selectedObject.size);
            if (randomPosition == null)
                return;

            foreach (Vector3 pos in randomPosition)
                obstacleMap[(int)pos.x, (int)pos.y] = true;

            currentObstacleCount += (int)(selectedObject.size.x * selectedObject.size.y); // TODO: improve this

            if (!MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                foreach (Vector3 pos in randomPosition)
                    obstacleMap[(int)pos.x, (int)pos.y] = false;
                currentObstacleCount -= (int)(selectedObject.size.x * selectedObject.size.y);// TODO: improve this
                continue;
            }

            GameObject selectedPrefab = obstaclePrefab;
            if (selectedObject.pushable)
                selectedPrefab = pushableObstaclePrefab;

            // TODO: I am not certain why is that so:
            // size 1 object in the middle of the tile, while object with a different size in the bottom left corner.
            float posX = randomPosition[0].x;
            float posY = randomPosition[0].y;
            if (selectedObject.size.x % 2 != 0)
                posX += 0.5f;
            if (selectedObject.size.y % 2 != 0)
                posY += 0.5f;

            Vector3 placingPos = new Vector3(posX, posY, randomPosition[0].z);

            GameObject ob = Instantiate(selectedPrefab,
                            placingPos,
                            Quaternion.identity);
            ob.GetComponent<Obstacle>().Initialise(selectedObject);
            ob.name = selectedObject.oName;
            ob.transform.parent = envObjectsHolder.transform;
        }
    }

    // https://www.youtube.com/watch?v=2ycN6ZkWgOo&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=11
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        //return true;
        // flood fill algo from bottom left corner that stays free;
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Vector3> queue = new(); // TODO: do I need "Coord" like in the video?
        queue.Enqueue(emptyCorner); // always free
        mapFlags[(int)emptyCorner.x, (int)emptyCorner.y] = true;// always free

        int accessibleTileCount = 1;
        while (queue.Count > 0)
        {
            Vector3 tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = (int)tile.x + x;
                    int neighbourY = (int)tile.y + y;

                    // TODO: Improve this? I have 'reversed' if statements to get rid of indentation.
                    if (!(x == 0 ^ y == 0)) // (using the XOR operator instead) that way, we'll skip the current tile (: from comment
                        continue;

                    if (!(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) &&
                        neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)))
                        continue; // make sure it is in bounds

                    if (mapFlags[neighbourX, neighbourY] || obstacleMap[neighbourX, neighbourY])
                        continue;

                    mapFlags[neighbourX, neighbourY] = true;
                    queue.Enqueue(new Vector3(neighbourX, neighbourY, 0f));
                    accessibleTileCount++;
                }
            }
        }

        int targetAccessibleTileCount = mapSizeSqm - currentObstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;
    }
    void LayoutFloorAdditions(int minimum, int maximum)
    {
        TileBase[] tiles = flav.floorAdditions;
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            List<Vector3> randomPosiiton = GetRandomOpenPosition(Vector2.one);
            middlegroundTilemap.SetTile(new Vector3Int((int)randomPosiiton[0].x, (int)randomPosiiton[0].y),
                             tiles[Random.Range(0, tiles.Length)]);
        }

    }

    void PlaceSpecialObject()
    {
        // TODO: this should be way smarter
        TilemapObject ob = flav.outerObjects[Random.Range(0, flav.outerObjects.Length)];

        GameObject n = new GameObject(ob.oName);
        n.transform.parent = envObjectsHolder.transform;

        float y = mapSize.y + ob.size.y;
        float x = mapSize.x * 0.5f;
        n.transform.position = new Vector3(x, y, 0f);

        n.transform.DOPunchPosition(Vector3.up * 0.5f, 2f, 0, 1f, false).SetLoops(-1, LoopType.Yoyo); // TODO: cool! 

        SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 1;
        sr.sprite = ob.sprite;
    }

    void PlaceRiver()
    {
        // TODO: is there a way to write one function for both rivers?
        if (Random.Range(0, 2) == 0)
            PlaceVerticalRiver();
        else
            PlaceHorizontalRiver();
    }

    void PlaceVerticalRiver()
    {
        // river
        int middleOfMap = Mathf.RoundToInt(mapSize.x * 0.5f);
        int riverWidth = Random.Range(1, mapSize.x / 5);
        int xMin = middleOfMap - riverWidth;
        int xMax = middleOfMap + riverWidth;

        // make sure edge of the river is walkable;
        for (int x = xMin - 1; x <= xMax + 1; x++)
            for (int y = -1; y < mapSize.y + 1; y++) // -+1 to cover the edges
                ClearTile(new Vector2Int(x, y));

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = -1; y < mapSize.y + 1; y++) // -+1 to cover the edges
            {
                TileBase selectedTile = flav.outerTiles[Random.Range(0, flav.outerTiles.Length)];
                // edges
                if (x == xMin)
                    selectedTile = flav.edgeE;
                if (x == xMax)
                    selectedTile = flav.edgeW;
                // corners
                if (x == xMin && y == -1)
                    selectedTile = flav.cornerSW;
                if (x == xMin && y == mapSize.y)
                    selectedTile = flav.cornerNE;
                if (x == xMax && y == -1)
                    selectedTile = flav.cornerSE;
                if (x == xMax && y == mapSize.y)
                    selectedTile = flav.cornerNW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }

        // bridge
        int bridgeWidth = Random.Range(1, 4);
        int bridgeY = Random.Range(1, mapSize.y - bridgeWidth - 1); // so the bridge can fit
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = bridgeY - 1; y <= bridgeY + bridgeWidth; y++) // +-1 for edges
            {
                TileBase selectedTile = flav.floorTiles[Random.Range(0, flav.floorTiles.Length)];
                // edges
                if (y == bridgeY - 1)
                    selectedTile = flav.edgeS;
                if (y == bridgeY + bridgeWidth)
                    selectedTile = flav.edgeN;
                // corners
                if (y == bridgeY - 1 && x == xMin)
                    selectedTile = flav.inlandCornerNE;
                if (y == bridgeY - 1 && x == xMax)
                    selectedTile = flav.inlandCornerNW;
                if (y == bridgeY + bridgeWidth && x == xMin)
                    selectedTile = flav.inlandCornerSE;
                if (y == bridgeY + bridgeWidth && x == xMax)
                    selectedTile = flav.inlandCornerSW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }
    }

    void PlaceHorizontalRiver()
    {
        // river
        int middleOfMap = Mathf.RoundToInt(mapSize.y * 0.5f);
        int riverWidth = Mathf.RoundToInt(Random.Range(1, mapSize.y * 0.2f));
        int yMin = middleOfMap - riverWidth;
        int yMax = middleOfMap + riverWidth;

        // make sure edge of the river is walkable;
        for (int y = yMin - 1; y <= yMax + 1; y++)
            for (int x = -1; x < mapSize.x + 1; x++)
                ClearTile(new Vector2Int(x, y));

        for (int y = yMin; y <= yMax; y++)
        {
            for (int x = -1; x < mapSize.x + 1; x++) // -+1 to cover the edges
            {
                TileBase selectedTile = flav.outerTiles[Random.Range(0, flav.outerTiles.Length)];
                // edges
                if (y == yMin)
                    selectedTile = flav.edgeN;
                if (y == yMax)
                    selectedTile = flav.edgeS;
                // corners
                if (y == yMin && x == -1)
                    selectedTile = flav.cornerNW;
                if (y == yMin && x == mapSize.x)
                    selectedTile = flav.cornerNE;
                if (y == yMax && x == -1)
                    selectedTile = flav.cornerSE;
                if (y == yMax && x == mapSize.x)
                    selectedTile = flav.cornerSW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }

        // bridge
        int bridgeWidth = Random.Range(1, 4);
        int bridgeX = Random.Range(1, mapSize.x - bridgeWidth - 1); // so the bridge can fit
        for (int y = yMin; y <= yMax; y++)
        {
            for (int x = bridgeX - 1; x <= bridgeX + bridgeWidth; x++) // +-1 for edges
            {
                TileBase selectedTile = flav.floorTiles[Random.Range(0, flav.floorTiles.Length)];
                // edges
                if (x == bridgeX - 1)
                    selectedTile = flav.edgeW;
                if (x == bridgeX + bridgeWidth)
                    selectedTile = flav.edgeE;
                // corners
                if (x == bridgeX - 1 && y == yMin)
                    selectedTile = flav.inlandCornerSW;
                if (x == bridgeX - 1 && y == yMax)
                    selectedTile = flav.inlandCornerNW;
                if (x == bridgeX + bridgeWidth && y == yMin)
                    selectedTile = flav.inlandCornerSE;
                if (x == bridgeX + bridgeWidth && y == yMax)
                    selectedTile = flav.inlandCornerNE;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }
    }

    void PlaceLakeInTheMiddle()
    {
        int lakeWidth = Mathf.RoundToInt(Random.Range(mapSize.x * 0.4f, mapSize.x * 0.7f));
        int lakeHeight = Mathf.RoundToInt(Random.Range(mapSize.x * 0.4f, mapSize.y * 0.7f));

        // -+1 to cover the edges
        int xMin = Mathf.RoundToInt((mapSize.x - lakeWidth) * 0.5f) + 1;
        int yMin = Mathf.RoundToInt((mapSize.y - lakeHeight) * 0.5f) + 1;

        int xMax = xMin + lakeWidth - 1;
        int yMax = yMin + lakeHeight - 1;

        for (int x = xMin - 1; x <= xMax + 1; x++)
        {
            for (int y = yMin - 1; y <= yMax + 1; y++)
            {
                ClearTile(new Vector2Int(x, y));
            }
        }


        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                ClearTile(new Vector2Int(x, y));
                TileBase selectedTile = flav.outerTiles[Random.Range(0, flav.outerTiles.Length)];

                // edges
                if (x == xMin)
                    selectedTile = flav.edgeE;
                if (x == xMax)
                    selectedTile = flav.edgeW;
                if (y == yMin)
                    selectedTile = flav.edgeN;
                if (y == yMax)
                    selectedTile = flav.edgeS;

                // corners
                if (y == yMin && x == xMin)
                    selectedTile = flav.inlandCornerSE;
                if (y == yMin && x == xMax)
                    selectedTile = flav.inlandCornerSW;
                if (y == yMax && x == xMin)
                    selectedTile = flav.inlandCornerNE;
                if (y == yMax && x == xMax)
                    selectedTile = flav.inlandCornerNW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }
    }

    void ClearTile(Vector2Int _pos)
    {
        // TODO: I am not certain it is a good idea to place it here
        // It makes sure there won't be obstacles/map additions placed on that tile;
        openGridPositions.Remove(new Vector3(_pos.x, _pos.y, 0f));

        middlegroundTilemap.SetTile(new Vector3Int(_pos.x, _pos.y), null); // getting rid of map additons
        Collider2D col = Physics2D.OverlapCircle(new Vector2(_pos.x + 0.5f, _pos.y + 0.5f), 0.2f);
        if (col == null)
            return;
        DestroyImmediate(col.transform.parent.gameObject); // TODO: destory immediate to make it work in the editor
    }

    void SpawnEnemies(int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Character instantiatedSO = Instantiate(enemyCharacters[Random.Range(0, enemyCharacters.Length)]);
            GameObject newCharacter = Instantiate(enemyGO, GetRandomOpenPosition(Vector2.one)[0], Quaternion.identity); // TODO: get random posiiton is wrong here

            instantiatedSO.Initialize(newCharacter);
            newCharacter.name = instantiatedSO.characterName;

            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

            // face right
            CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();
            characterRendererManager.Face(Vector2.right);
            characterRendererManager.Face(Vector2.zero);
        }
    }
}