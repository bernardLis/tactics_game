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

    public int seed;

    public Transform envObjects;

    int mapSizeX = 20;
    int mapSizeY = 20;

    public Tilemap backgroundTilemap;
    public Tilemap middlegroundTilemap;
    public Tilemap foregroundTilemap;

    public TilemapFlavour[] tilemapFlavours;

    // TODO: this could be derived from map size
    public Count floorAdditionsCount = new Count(3, 7);
    [Range(0, 1)]
    public float obstaclePercent;
    // bottom left corner always stays empty
    Vector3 bottomLeftCornerPosition;
    public Count trapCount = new Count(1, 2); // TODO: not used righ now

    // TODO: this
    public Count enemyCount = new Count(1, 3);

    public GameObject obstaclePrefab;
    public GameObject pushableObstaclePrefab;
    public GameObject trap;

    public Character[] enemyCharacters;
    public GameObject enemyGO;

    List<Vector3> openGridPositions = new();
    public string mapVariantChosen; // TODO: just for dev

    public void SetupScene()
    {
        Random.InitState(seed);
        TilemapFlavour flav = tilemapFlavours[Random.Range(0, tilemapFlavours.Length)];

        ClearTilemaps();
        OuterSetup(flav);
        BoardSetup(flav);
        InitialiseOpenPositions();
        ClearObjects();

        // pick a map variant
        /*
        int mapVariant = Random.Range(1, 4);
        if (mapVariant == 1) // river in the middle
        {
            PlaceRiver(flav);
            obstaclePercent = Random.Range(0f, 0.2f);
            mapVariantChosen = "River";
        }
        if (mapVariant == 2) // big space some obstacles
        {
            obstaclePercent = Random.Range(0.1f, 0.2f);
            mapVariantChosen = "Space";

        }
        if (mapVariant == 3) // "labirynth"
        {
            obstaclePercent = 1f;
            mapVariantChosen = "Labirynth";
        }
        */
        obstaclePercent = 1f;
        mapVariantChosen = "Labirynth";

        LayoutObstacles(flav);
        //LayoutObjectAtRandom(trap, trapCount.minimum, trapCount.maximum);
        LayoutFloorAdditions(flav, floorAdditionsCount.minimum, floorAdditionsCount.maximum);

        // TODO: this should be way smarter
        PlaceSpecialObject(flav);

        // TODO: spawn player chars / set-up player spawn positions;
    }


    void ClearTilemaps()
    {
        backgroundTilemap.ClearAllTiles();
        middlegroundTilemap.ClearAllTiles();
        foregroundTilemap.ClearAllTiles();
    }

    void OuterSetup(TilemapFlavour _flav)
    {
        int outerX = mapSizeX * 3;
        int outerY = mapSizeY * 3;
        outerX = Mathf.Clamp(outerX, 30, 10000);
        outerY = Mathf.Clamp(outerY, 30, 10000);

        TileBase[] tiles = _flav.outerTiles;

        for (int x = -outerX; x < outerX; x++)
            for (int y = -outerY; y < outerY; y++)
                backgroundTilemap.SetTile(new Vector3Int(x, y), tiles[Random.Range(0, tiles.Length)]);
    }

    void BoardSetup(TilemapFlavour _flav)
    {

        for (int x = -1; x < mapSizeX + 1; x++)
        {
            for (int y = -1; y < mapSizeY + 1; y++) // +-1 to create an edge;
            {
                TileBase selectedTile = _flav.floorTiles[Random.Range(0, _flav.floorTiles.Length)];

                // edge
                if (x == -1)
                    selectedTile = _flav.edgeW;
                if (x == mapSizeX)
                    selectedTile = _flav.edgeE;
                if (y == -1)
                    selectedTile = _flav.edgeS;
                if (y == mapSizeY)
                    selectedTile = _flav.edgeN;

                // corners
                if (x == -1 && y == -1)
                    selectedTile = _flav.cornerSE;
                if (x == mapSizeX && y == -1)
                    selectedTile = _flav.cornerSW;
                if (x == -1 && y == mapSizeY)
                    selectedTile = _flav.cornerNW;
                if (x == mapSizeX && y == mapSizeY)
                    selectedTile = _flav.cornerNE;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }
    }

    void InitialiseOpenPositions()
    {
        openGridPositions.Clear();

        for (int x = 0; x < mapSizeX; x++)
            for (int y = 0; y < mapSizeY; y++)
                openGridPositions.Add(new Vector3(x, y, 0f));

        // bottom left corner is always empty
        bottomLeftCornerPosition = Vector3.zero;
        openGridPositions.Remove(bottomLeftCornerPosition);
    }

    List<Vector3> GetRandomOpenPosition(Vector2 _size)
    {
        List<Vector3> openPositions = new();
        if (_size == Vector2.one)
        {
            int randomIndex = Random.Range(0, openGridPositions.Count);
            Vector3 randomPosition = openGridPositions[randomIndex];
            openGridPositions.RemoveAt(randomIndex); // only one thing can occupy a position
            openPositions.Add(randomPosition);
            return openPositions;
        }
        // how do I find open positions in size of _size? 
        // what does it mean there are open positions in _size?
        // it means that there are open tiles in _size next to each other, 
        // so I should go through all open positions and check what sizes do they come in
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

            // here I need to check if we have enough open positions to send it, else clear candidate positions
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

    void ClearObjects()
    {
        foreach (Transform t in envObjects)
            DestroyImmediate(t.gameObject); // TODO: use Destroy instead, this is for editor
    }

    void LayoutObstacles(TilemapFlavour _flav)
    {
        bool[,] obstacleMap = new bool[mapSizeX, mapSizeY];

        int objectCount = (int)(mapSizeX * mapSizeY * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            TilemapObject selectedObject = _flav.obstacles[Random.Range(0, _flav.obstacles.Length)];

            List<Vector3> randomPosition = GetRandomOpenPosition(selectedObject.size);
            if (randomPosition == null)
                return;

            foreach (Vector3 pos in randomPosition)
                obstacleMap[(int)pos.x, (int)pos.y] = true;

            currentObstacleCount++; // TODO: improve this

            if (!MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Debug.Log("map is not fully accessbile returning");
                foreach (Vector3 pos in randomPosition)
                    obstacleMap[(int)pos.x, (int)pos.y] = false;
                currentObstacleCount--;
                continue;
            }

            GameObject selectedPrefab = obstaclePrefab;
            if (selectedObject.pushable)
            {
                selectedPrefab = pushableObstaclePrefab;
            }

            GameObject ob = Instantiate(selectedPrefab,
                            new Vector3(randomPosition[0].x + 0.5f, randomPosition[0].y + 0.5f, randomPosition[0].z),
                            Quaternion.identity);
            ob.GetComponent<Obstacle>().Initialise(selectedObject);
            ob.transform.parent = envObjects;

        }
    }
    // https://www.youtube.com/watch?v=2ycN6ZkWgOo&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=11
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        return true;
        // flood fill algo from bottom left corner that stays free;
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Vector3> queue = new(); // TODO: do I need "Coord" like in the video?
        queue.Enqueue(bottomLeftCornerPosition); // always free
        mapFlags[0, 0] = true;// always free

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

                    if (x == 0 ^ y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Vector3(neighbourX, neighbourY, 0f));
                                accessibleTileCount++;
                            }
                        }
                    }

                    /*
                    if (x != 0 ^ y != 0)// (using the XOR operator instead) that way, we'll skip the current tile (: from comment
                        continue;
                    // inside of obstacle map
                    if (neighbourX < 0 || neighbourX >= obstacleMap.GetLength(0) || neighbourY < 0 || neighbourY >= obstacleMap.GetLength(1))
                        continue;

                    Debug.Log("neighbourX + " + neighbourX);
                    Debug.Log("neighbourY + " + neighbourY);

                    if (mapFlags[neighbourX, neighbourY])
                        continue;

                    if (obstacleMap[neighbourX, neighbourY])
                        continue;

                    mapFlags[neighbourX, neighbourY] = true;
                    queue.Enqueue(new Vector3(neighbourX, neighbourY, 0f));
                    accessibleTileCount++;
                    */
                }

            }
        }

        int targetAccessibleTileCount = mapSizeX * mapSizeY - currentObstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;

    }
    void LayoutFloorAdditions(TilemapFlavour _flav, int minimum, int maximum)
    {
        TileBase[] tiles = _flav.floorAdditions;
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

    void PlaceSpecialObject(TilemapFlavour _flav)
    {
        // TODO: this should be way smarter
        TilemapObject ob = _flav.outerObjects[Random.Range(0, _flav.outerObjects.Length)];

        // create a game object with sprite renderer compotenent and set correct layer
        GameObject n = new GameObject(ob.oName);
        n.transform.parent = envObjects;

        float y = mapSizeY + ob.size.y;
        float x = mapSizeX * 0.5f;
        n.transform.position = new Vector3(x, y, 0f);

        n.transform.DOPunchPosition(Vector3.up * 0.5f, 2f, 0, 1f, false).SetLoops(-1, LoopType.Yoyo); // TODO: cool! 

        SpriteRenderer sr = n.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 1;
        sr.sprite = ob.sprite;
    }

    // TODO: is there a way to 
    void PlaceRiver(TilemapFlavour _flav)
    {
        // TODO: is there a way to write one function for both rivers?
        if (Random.Range(0, 2) == 0)
            PlaceVerticalRiver(_flav);
        else
            PlaceHorizontalRiver(_flav);

    }
    void PlaceVerticalRiver(TilemapFlavour _flav)
    {
        // river
        int middleOfMap = Mathf.RoundToInt(mapSizeX * 0.5f);
        int riverWidth = Random.Range(1, mapSizeX / 5);
        int xMin = middleOfMap - riverWidth;
        int xMax = middleOfMap + riverWidth;

        // make sure edge of the river is walkable;
        for (int x = xMin - 1; x <= xMax + 1; x++)
            for (int y = -1; y < mapSizeY + 1; y++) // -+1 to cover the edges
                ClearTile(new Vector2Int(x, y));

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = -1; y < mapSizeY + 1; y++) // -+1 to cover the edges
            {
                TileBase selectedTile = _flav.outerTiles[Random.Range(0, _flav.outerTiles.Length)];

                // edges
                if (x == xMin)
                    selectedTile = _flav.edgeE;
                if (x == xMax)
                    selectedTile = _flav.edgeW;
                // corners
                if (x == xMin && y == -1)
                    selectedTile = _flav.cornerSW;
                if (x == xMin && y == mapSizeY)
                    selectedTile = _flav.cornerNE;
                if (x == xMax && y == -1)
                    selectedTile = _flav.cornerSE;
                if (x == xMax && y == mapSizeY)
                    selectedTile = _flav.cornerNW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }

        // bridge
        int bridgeWidth = Random.Range(1, 4);
        int bridgeY = Random.Range(1, mapSizeY - bridgeWidth - 1); // so the bridge can fit
        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = bridgeY - 1; y <= bridgeY + bridgeWidth; y++) // +-1 for edges
            {
                // middle tiles are normal edge tiles are edges corner tile are corners - wow. smart
                TileBase selectedTile = _flav.floorTiles[Random.Range(0, _flav.floorTiles.Length)];
                // edges
                if (y == bridgeY - 1)
                    selectedTile = _flav.edgeS;
                if (y == bridgeY + bridgeWidth)
                    selectedTile = _flav.edgeN;

                // corners
                if (y == bridgeY - 1 && x == xMin)
                    selectedTile = _flav.inlandCornerNE;
                if (y == bridgeY - 1 && x == xMax)
                    selectedTile = _flav.inlandCornerNW;
                if (y == bridgeY + bridgeWidth && x == xMin)
                    selectedTile = _flav.inlandCornerSE;
                if (y == bridgeY + bridgeWidth && x == xMax)
                    selectedTile = _flav.inlandCornerSW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }
    }

    void PlaceHorizontalRiver(TilemapFlavour _flav)
    {
        // river
        int middleOfMap = Mathf.RoundToInt(mapSizeY * 0.5f);
        int riverWidth = Random.Range(1, mapSizeY / 5);
        int yMin = middleOfMap - riverWidth;
        int yMax = middleOfMap + riverWidth;

        // make sure edge of the river is walkable;
        for (int y = yMin - 1; y <= yMax + 1; y++)
            for (int x = -1; x < mapSizeX + 1; x++)
                ClearTile(new Vector2Int(x, y));


        for (int y = yMin; y <= yMax; y++)
        {
            for (int x = -1; x < mapSizeX + 1; x++) // -+1 to cover the edges
            {

                TileBase selectedTile = _flav.outerTiles[Random.Range(0, _flav.outerTiles.Length)];

                // edges
                if (y == yMin)
                    selectedTile = _flav.edgeN;
                if (y == yMax)
                    selectedTile = _flav.edgeS;
                // corners
                if (y == yMin && x == -1)
                    selectedTile = _flav.cornerNW;
                if (y == yMin && x == mapSizeX)
                    selectedTile = _flav.cornerNE;
                if (y == yMax && x == -1)
                    selectedTile = _flav.cornerSE;
                if (y == yMax && x == mapSizeX)
                    selectedTile = _flav.cornerSW;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }


        // bridge
        int bridgeWidth = Random.Range(1, 4);
        int bridgeX = Random.Range(1, mapSizeX - bridgeWidth - 1); // so the bridge can fit
        for (int y = yMin; y <= yMax; y++)
        {
            for (int x = bridgeX - 1; x <= bridgeX + bridgeWidth; x++) // +-1 for edges
            {
                // middle tiles are normal edge tiles are edges corner tile are corners - wow. smart
                TileBase selectedTile = _flav.floorTiles[Random.Range(0, _flav.floorTiles.Length)];
                // edges
                if (x == bridgeX - 1)
                    selectedTile = _flav.edgeW;
                if (x == bridgeX + bridgeWidth)
                    selectedTile = _flav.edgeE;
                // corners
                if (x == bridgeX - 1 && y == yMin)
                    selectedTile = _flav.inlandCornerSW;
                if (x == bridgeX - 1 && y == yMax)
                    selectedTile = _flav.inlandCornerNW;
                if (x == bridgeX + bridgeWidth && y == yMin)
                    selectedTile = _flav.inlandCornerSE;
                if (x == bridgeX + bridgeWidth && y == yMax)
                    selectedTile = _flav.inlandCornerNE;

                backgroundTilemap.SetTile(new Vector3Int(x, y), selectedTile);
            }
        }


    }

    // clears tile from objects and floor additions on middle ground
    void ClearTile(Vector2Int _pos)
    {
        // TDOO: I am not certain it is a good idea to place it here
        // It makes sure there won't be obstacles/map additions placed on that tile;
        openGridPositions.Remove(new Vector3(_pos.x, _pos.y, 0f));

        middlegroundTilemap.SetTile(new Vector3Int(_pos.x, _pos.y), null); // getting rid of map additons
        Collider2D col = Physics2D.OverlapCircle(new Vector2(_pos.x + 0.5f, _pos.y + 0.5f), 0.2f);
        if (col == null)
            return;
        DestroyImmediate(col.transform.parent.gameObject); // TODO: destory immediate to make it work in the editor
    }


}
