using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System;

// https://learn.unity.com/tutorial/level-generation?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f6
public class BoardManager : MonoBehaviour
{
    [Header("Map Setup")]
    public string mapVariantChosen; // TODO: just for dev
    public int seed;
    public Vector2Int mapSize = new(20, 20);
    [Range(0, 1)]
    public float terrainIrregularitiesPercent;

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
    Vector3Int emptyTile;
    float obstaclePercent;
    bool[,] obstacleMap;
    int accessibleTileCount;
    Queue<Vector3Int> floodFillQueue;
    List<Vector3Int> openGridPositions = new();
    int floorTileCount;
    TilemapFlavour flav;

    public void SetupScene()
    {
        InitialSetup();
        BoardSetup();
        InitialiseOpenPositions();

        int mapVariant = Random.Range(1, 5);
        if (mapVariant == 1)
        {
            obstaclePercent = Random.Range(0f, 0.1f);
            mapVariantChosen = "Space";
        }
        if (mapVariant == 2)
        {
            obstaclePercent = 1f;
            mapVariantChosen = "Labirynth";
        }
        if (mapVariant == 3)
        {
            PlaceRiver();
            obstaclePercent = Random.Range(0f, 0.1f);
            mapVariantChosen = "River";
        }
        if (mapVariant == 4)
        {
            PlaceLakeInTheMiddle();
            obstaclePercent = Random.Range(0f, 0.1f);
            mapVariantChosen = "Lake in the middle";
        }

        PlaceTerrainIrregularities();
        HandleLooseTiles();

        HandleEdge();

        // TODO: this should be way smarter
        PlaceSpecialObject();
        LayoutObstacles();
        //LayoutObjectAtRandom(trap, trapCount.minimum, trapCount.maximum);
        LayoutFloorAdditions(Mathf.RoundToInt(floorTileCount * 0.1f), Mathf.RoundToInt(floorTileCount * 0.2f));

        DrawOuter();

        Debug.Log("what are you?");
        Debug.Log("self: " + backgroundTilemap.GetTile(new Vector3Int(9, 1)));
        Debug.Log("left: " + backgroundTilemap.GetTile(new Vector3Int(8, 1)));
        Debug.Log("top: " + backgroundTilemap.GetTile(new Vector3Int(9, 2)));

        Debug.Log("right: " + backgroundTilemap.GetTile(new Vector3Int(10, 1)));
        Debug.Log("bottom: " + backgroundTilemap.GetTile(new Vector3Int(9, 0)));


        // TODO: spawn player chars / set-up player spawn positions;
    }

    void InitialSetup()
    {
        Random.InitState(seed);

        obstacleMap = new bool[mapSize.x, mapSize.y];
        flav = tilemapFlavours[Random.Range(0, tilemapFlavours.Length)];

        backgroundTilemap.ClearAllTiles();
        middlegroundTilemap.ClearAllTiles();
        foregroundTilemap.ClearAllTiles();

        foreach (Transform child in envObjectsHolder.transform)
            DestroyImmediate(child.gameObject); // TODO: destory
    }

    void BoardSetup()
    {
        // +-1 because I am setting edge tiles to unwalkable edges
        for (int x = -1; x < mapSize.x + 1; x++)
            for (int y = -1; y < mapSize.y + 1; y++)
                backgroundTilemap.SetTile(new Vector3Int(x, y), flav.floorTiles[Random.Range(0, flav.floorTiles.Length)]);
    }

    void InitialiseOpenPositions()
    {
        openGridPositions.Clear();

        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                openGridPositions.Add(new Vector3Int(x, y, 0));


        openGridPositions = Utility.ShuffleList<Vector3Int>(openGridPositions, seed);
        // one of the tiles is always empty (used to be corner)
        emptyTile = GetRandomOpenPosition(Vector2.one)[0];
        openGridPositions.Remove(emptyTile);
    }

    void PlaceTerrainIrregularities()
    {
        int irrCount = Mathf.FloorToInt((mapSize.x * 2 + mapSize.y * 2) * terrainIrregularitiesPercent);

        // I want to place irregularities by destrying tiles on the edge
        // I need to make sure map is still accessbile before destroying it
        // To get tiles on the edge I could try getting random open positions
        for (int i = 0; i < irrCount; i++)
        {
            int x = Random.Range(0, mapSize.x);
            int y = Random.Range(0, mapSize.y);

            int edgeX = -1;
            int edgeY = -1;
            if (Random.Range(0, 2) == 1)
            {
                edgeX = mapSize.x;
                edgeY = mapSize.y;
            }

            if (Random.Range(0, 2) == 0)
                ClearTile(new Vector3Int(x, edgeY));
            else
                ClearTile(new Vector3Int(edgeX, y));
        }
    }

    void HandleEdge()
    {
        for (int x = -1; x < mapSize.x + 1; x++)
            for (int y = -1; y < mapSize.y + 1; y++)
                SetEdges(new Vector3Int(x, y));

        for (int x = -1; x < mapSize.x + 1; x++)
            for (int y = -1; y < mapSize.y + 1; y++)
                SetInlandCorners(new Vector3Int(x, y));
    }

    void HandleLooseTiles()
    {
        for (int x = -1; x < mapSize.x + 1; x++)
            for (int y = -1; y < mapSize.y + 1; y++)
                ClearLooseTile(new Vector3Int(x, y));
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

    void LayoutObstacles()
    {
        floorTileCount = 0;
        for (int x = 0; x <= mapSize.x; x++)
            for (int y = 0; y <= mapSize.y; y++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    floorTileCount++;

        int objectCount = Mathf.FloorToInt(floorTileCount * obstaclePercent);
        int obstacledTileCount = 0;
        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            TilemapObject selectedObject = flav.obstacles[Random.Range(0, flav.obstacles.Length)];
            List<Vector3Int> randomPosition = GetRandomOpenPosition(selectedObject.size);
            if (randomPosition == null)
                return;

            foreach (Vector3Int pos in randomPosition)
                obstacleMap[pos.x, pos.y] = true;

            obstacledTileCount += selectedObject.size.x * selectedObject.size.y; // TODO: improve this

            if (!MapIsFullyAccessible(obstacleMap, obstacledTileCount))
            {
                foreach (Vector3Int pos in randomPosition)
                    obstacleMap[pos.x, pos.y] = false;
                obstacledTileCount -= selectedObject.size.x * selectedObject.size.y;// TODO: improve this
                continue;
            }

            GameObject selectedPrefab = obstaclePrefab;
            if (selectedObject.pushable)
                selectedPrefab = pushableObstaclePrefab;

            // TODO: I am not certain why is that so:
            // size 1 object needs to be in the middle of the tile, 
            // while object with a different size are placed in the bottom left corner.
            float posX = randomPosition[0].x;
            float posY = randomPosition[0].y;
            if (selectedObject.size.x % 2 != 0)
                posX += 0.5f;
            if (selectedObject.size.y % 2 != 0)
                posY += 0.5f;

            Vector3 placingPos = new Vector3(posX, posY, randomPosition[0].z);

            GameObject ob = Instantiate(selectedPrefab, placingPos, Quaternion.identity);
            ob.GetComponent<Obstacle>().Initialise(selectedObject);
            ob.name = selectedObject.oName;
            ob.transform.parent = envObjectsHolder.transform;
        }
    }

    void LayoutFloorAdditions(int minimum, int maximum)
    {
        TileBase[] tiles = flav.floorAdditions;
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            List<Vector3Int> randomPosiiton = GetRandomOpenPosition(Vector2.one);
            middlegroundTilemap.SetTile(randomPosiiton[0],
                             tiles[Random.Range(0, tiles.Length)]);
        }
    }

    /* --- MAP TYPES --- */
    void PlaceRiver()
    {
        int middleOfMap = Mathf.RoundToInt(mapSize.x * 0.5f);
        int riverWidth = Random.Range(1, mapSize.x / 5);
        int min = middleOfMap - riverWidth;
        int max = middleOfMap + riverWidth;

        // TODO: is there a way to write one function for both rivers?
        if (Random.Range(0f, 1f) > 0.5f)
            PlaceVerticalRiver(min, max);
        else
            PlaceHorizontalRiver(min, max);
    }

    void PlaceVerticalRiver(int _min, int _max)
    {
        // river
        for (int x = _min; x <= _max; x++)
            for (int y = -1; y < mapSize.y + 1; y++) // -+1 to cover the edges
                ClearTile(new Vector3Int(x, y));

        // bridge
        int bridgeWidth = Random.Range(1, mapSize.x - 3);
        int bridgeY = Random.Range(2, mapSize.y - bridgeWidth - 1); // so the bridge can fit
        for (int x = _min; x <= _max; x++)
            for (int y = bridgeY - 1; y <= bridgeY + bridgeWidth; y++) // +-1 for edges
                SetBackgroundFloorTile(new Vector3Int(x, y));
    }

    void PlaceHorizontalRiver(int _min, int _max)
    {
        // river
        for (int y = _min; y <= _max; y++)
            for (int x = -1; x < mapSize.x + 1; x++) // -+1 to cover the edges
                ClearTile(new Vector3Int(x, y));

        // bridge
        int bridgeWidth = Random.Range(1, mapSize.x - 3);
        int bridgeX = Random.Range(2, mapSize.x - bridgeWidth - 1); // so the bridge can fit
        for (int y = _min; y <= _max; y++)
            for (int x = bridgeX - 1; x <= bridgeX + bridgeWidth; x++) // +-1 for edges
                SetBackgroundFloorTile(new Vector3Int(x, y));
    }

    void PlaceLakeInTheMiddle()
    {
        int lakeWidth = Mathf.RoundToInt(Random.Range(mapSize.x * 0.2f, mapSize.x * 0.4f));
        int lakeHeight = Mathf.RoundToInt(Random.Range(mapSize.x * 0.2f, mapSize.y * 0.4f));

        // -+1 to cover the edges
        int xMin = Mathf.RoundToInt((mapSize.x - lakeWidth) * 0.5f) + 1;
        int yMin = Mathf.RoundToInt((mapSize.y - lakeHeight) * 0.5f) + 1;

        int xMax = xMin + lakeWidth - 2;
        int yMax = yMin + lakeHeight - 2;

        for (int x = xMin; x <= xMax; x++)
            for (int y = yMin; y <= yMax; y++)
                ClearTile(new Vector3Int(x, y));
    }

    /* TODO: unfinished
        void CarveHourglass()
        {
            // vertical hourglass
            int yMiddle = Mathf.FloorToInt(mapSize.y * 0.5f);

            for (int y = 0; y < yMiddle; y++)
            {
                // I want to clear # of tiles on each end of the row, depending on what row we are on
                // until we reach the middle, I want to be clearing more and more tiles
                // than I want to be clearing less and less tiles
                for (int x = 0; x < y; x++)
                {
                    ClearTile(new Vector3Int(x, y));
                    ClearTile(new Vector3Int(mapSize.x - x, y));
                }
            }

            for (int y = mapSize.y; y > yMiddle; y--)
            {
                // I want to clear # of tiles on each end of the row, depending on what row we are on
                // until we reach the middle, I want to be clearing more and more tiles
                // than I want to be clearing less and less tiles
                for (int x = 0; x < mapSize.y-y; x++)
                {
                    ClearTile(new Vector3Int(x, y));
                    ClearTile(new Vector3Int(mapSize.x - x, y));
                }
            }


        }
    */

    /* --- HELPERS --- */
    void SetEdges(Vector3Int _pos)
    {
        // I am swapping tiles that exist
        if (backgroundTilemap.GetTile(_pos) == null)
            return;

        bool[] neighbours = new bool[4]; // left, top, right, bottom 

        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y)) != null)
            neighbours[0] = true;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1)) != null)
            neighbours[1] = true;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y)) != null)
            neighbours[2] = true;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1)) != null)
            neighbours[3] = true;

        // TODO: this seems stupid
        if (neighbours[0] && !neighbours[1] && neighbours[2] && neighbours[3])// edge N
            SetEdgeTile(_pos, flav.edgeN);
        if (neighbours[0] && neighbours[1] && neighbours[2] && !neighbours[3])// edge S
            SetEdgeTile(_pos, flav.edgeS);
        if (!neighbours[0] && neighbours[1] && neighbours[2] && neighbours[3])// edge W
            SetEdgeTile(_pos, flav.edgeW);
        if (neighbours[0] && neighbours[1] && !neighbours[2] && neighbours[3])// edge E
            SetEdgeTile(_pos, flav.edgeE);

        // corners
        if (!neighbours[0] && !neighbours[1] && neighbours[2] && neighbours[3])// corner NW
            SetEdgeTile(_pos, flav.cornerNW);
        if (neighbours[0] && !neighbours[1] && !neighbours[2] && neighbours[3])// corner NE
            SetEdgeTile(_pos, flav.cornerNE);
        if (!neighbours[0] && neighbours[1] && neighbours[2] && !neighbours[3])// corner SW
            SetEdgeTile(_pos, flav.cornerSE);
        if (neighbours[0] && neighbours[1] && !neighbours[2] && !neighbours[3])// corner SE
            SetEdgeTile(_pos, flav.cornerSW);
    }

    void SetInlandCorners(Vector3Int _pos)
    {
        // I am only swapping tiles that exist
        if (backgroundTilemap.GetTile(_pos) == null)
            return;

        // TODO: this whole function seems verbose;
        // inland corners is surrounded by 2 edge tiles and 2 normal tiles
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y)) == null)
            return;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1)) == null)
            return;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y)) == null)
            return;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1)) == null)
            return;

        TileBase[] surroundingTiles = new TileBase[4]; // left, top, right, bottom 
        surroundingTiles[0] = backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y));
        surroundingTiles[1] = backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1));
        surroundingTiles[2] = backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y));
        surroundingTiles[3] = backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1));

        if (surroundingTiles[0] == flav.edgeS && surroundingTiles[3] == flav.edgeW)
            SetEdgeTile(_pos, flav.inlandCornerNW);
        if (surroundingTiles[2] == flav.edgeS && surroundingTiles[3] == flav.edgeE)
            SetEdgeTile(_pos, flav.inlandCornerNE);
        if (surroundingTiles[0] == flav.edgeN && surroundingTiles[1] == flav.edgeW)
            SetEdgeTile(_pos, flav.inlandCornerSW);
        if (surroundingTiles[1] == flav.edgeE && surroundingTiles[2] == flav.edgeN)
            SetEdgeTile(_pos, flav.inlandCornerSE);

        // inland corners need to consider corners too
        if (surroundingTiles[2] == flav.edgeS && surroundingTiles[3] == flav.cornerSW)
            SetEdgeTile(_pos, flav.inlandCornerNE);
        if (surroundingTiles[0] == flav.edgeS && surroundingTiles[3] == flav.cornerSE)
            SetEdgeTile(_pos, flav.inlandCornerNW);
        if (surroundingTiles[0] == flav.cornerNW && surroundingTiles[1] == flav.edgeW)
            SetEdgeTile(_pos, flav.inlandCornerSW);
        if (surroundingTiles[0] == flav.cornerSE && surroundingTiles[3] == flav.edgeW)
            SetEdgeTile(_pos, flav.inlandCornerNW);
        if (surroundingTiles[1] == flav.cornerNE && surroundingTiles[2] == flav.edgeN)
            SetEdgeTile(_pos, flav.inlandCornerSE);
        if (surroundingTiles[0] == flav.edgeN && surroundingTiles[1] == flav.cornerNW)
            SetEdgeTile(_pos, flav.inlandCornerSW);
        if (surroundingTiles[2] == flav.cornerSW && surroundingTiles[3] == flav.edgeE)
            SetEdgeTile(_pos, flav.inlandCornerNE);
        if (surroundingTiles[1] == flav.edgeE && surroundingTiles[2] == flav.cornerNE)
            SetEdgeTile(_pos, flav.inlandCornerSE);

        // normal corners next to other normal corners
        if (surroundingTiles[2] == flav.cornerSE && surroundingTiles[3] == flav.cornerSE)
        {
            SetEdgeTile(_pos, flav.cornerSE);
            Debug.Log("bla");
        }
        if (surroundingTiles[0] == flav.cornerSW && surroundingTiles[3] == flav.cornerSW)
            SetEdgeTile(_pos, flav.cornerSW);
    }

    void ClearLooseTile(Vector3Int _pos)
    {
        int voidTiles = 0;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y)) == null)
            voidTiles++;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y)) == null)
            voidTiles++;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1)) == null)
            voidTiles++;
        if (backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1)) == null)
            voidTiles++;

        if (voidTiles >= 3)
            ClearTile(_pos);
    }

    void DrawOuter()
    {
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

    void SetEdgeTile(Vector3Int _pos, TileBase _tile)
    {
        ClearTile(new Vector3Int(_pos.x, _pos.y));
        backgroundTilemap.SetTile(_pos, _tile);
    }

    void SetBackgroundFloorTile(Vector3Int _pos)
    {
        backgroundTilemap.SetTile(_pos, flav.floorTiles[Random.Range(0, flav.floorTiles.Length)]);
    }

    void ClearTile(Vector3Int _pos)
    {
        // It makes sure there won't be obstacles/map additions placed on that tile;
        openGridPositions.Remove(new Vector3Int(_pos.x, _pos.y, 0));
        backgroundTilemap.SetTile(new Vector3Int(_pos.x, _pos.y), null);
    }

    bool IsFloorTile(Vector3Int _pos)
    {
        return Array.IndexOf(flav.floorTiles, backgroundTilemap.GetTile(_pos)) != -1;
    }

    // TODO: improve this?
    List<Vector3Int> GetRandomOpenPosition(Vector2 _size)
    {
        List<Vector3Int> candidatePositions = new();
        foreach (Vector3Int pos in openGridPositions)
        {
            candidatePositions.Add(pos);
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    Vector3Int posToCheck = new Vector3Int(pos.x - x, pos.y - y, 0); // - to go North West on the map
                    if (openGridPositions.Contains(posToCheck) && !candidatePositions.Contains(posToCheck))
                        candidatePositions.Add(posToCheck);
                }
            }

            if (candidatePositions.Count >= _size.x * _size.y)
            {
                foreach (Vector3Int posToRemove in candidatePositions)
                    openGridPositions.Remove(posToRemove);

                return candidatePositions; // winner
            }
            candidatePositions.Clear(); // try again
        }
        return null; // no open positions of required size
    }

    // https://www.youtube.com/watch?v=2ycN6ZkWgOo&list=PLFt_AvWsXl0ctd4dgE1F8g3uec4zKNRV0&index=11
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        // flood fill algo from corner that stays free;
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        mapFlags[emptyTile.x, emptyTile.y] = true; // always free

        floodFillQueue = new();
        floodFillQueue.Enqueue(emptyTile); // always free
        accessibleTileCount = 1; // one tile is always free

        while (floodFillQueue.Count > 0)
            CheckAccessibility(floodFillQueue.Dequeue(), mapFlags);

        int targetAccessibleTileCount = floorTileCount - currentObstacleCount;

        return targetAccessibleTileCount == accessibleTileCount;
    }

    void CheckAccessibility(Vector3Int _pos, bool[,] _mapFlags)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int neighbourX = _pos.x + x;
                int neighbourY = _pos.y + y;

                // TODO: Improve this? I have 'reversed' if statements to get rid of indentation.
                if (!(x == 0 ^ y == 0))// (using the XOR operator instead) that way, we'll skip the current tile (: from comment
                    continue;

                if (!(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) &&
                    neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)))
                    continue; // make sure it is in bounds

                if (_mapFlags[neighbourX, neighbourY] || obstacleMap[neighbourX, neighbourY])
                    continue;

                if (!IsFloorTile(new Vector3Int(neighbourX, neighbourY)))
                    continue;

                floodFillQueue.Enqueue(new Vector3Int(neighbourX, neighbourY));
                accessibleTileCount++;
                _mapFlags[neighbourX, neighbourY] = true;
            }
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
}