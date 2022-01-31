using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using Pathfinding;

// https://learn.unity.com/tutorial/level-generation?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f6
public class BoardManager : MonoBehaviour
{
    [Header("Map Setup")]
    MapVariant mapVariantChosen;
    int seed;
    public Vector2Int mapSize = new(20, 20);

    [Header("Enemies")]
    public Character[] enemyCharacters;
    public GameObject enemyGO;

    [Header("Unity objects")]
    public MapVariant[] mapVariants;
    public TilemapBiome[] biomes;
    public Tilemap backgroundTilemap;
    public Tilemap middlegroundTilemap;
    public GameObject envObjectsHolder;
    public GameObject obstaclePrefab;
    public GameObject pushableObstaclePrefab;
    public GameObject outerObjectPrefab;
    public GameObject collectiblePrefab;
    public GameObject chestPrefab;
    public GameObject trap;
    public GameObject globalLightPrefab;
    public TextAsset graphData;

    // other map vars
    Vector3Int emptyTile;
    float terrainIrregularitiesPercent;
    float obstaclePercent;
    float outerAdditionsPercent;
    float trapPercent;
    bool[,] obstacleMap;
    int accessibleTileCount;
    Queue<Vector3Int> floodFillQueue;
    List<Vector3Int> openGridPositions = new();
    int floorTileCount;
    TilemapBiome biome;
    List<GameObject> pushableObstacles;
    List<Vector3Int> openOuterPositions = new();

    void Start()
    {
        seed = System.DateTime.Now.Millisecond;
        GenerateMap();
    }

    public async void GenerateMap()
    {
        InitialSetup();
        await Task.Delay(100);
        BoardSetup();
        await Task.Delay(100);

        InitialiseOpenPositions();
        ResolveMapVariant();
        await Task.Delay(100);

        PlaceTerrainIrregularities();
        await Task.Delay(100);

        HandleLooseTiles();
        await Task.Delay(100);

        HandleEdge();
        await Task.Delay(100);

        await LayoutObstacles();
        await Task.Delay(100);

        PlaceSpecialObjects();
        await Task.Delay(100);

        LayoutFloorAdditions(Mathf.RoundToInt(floorTileCount * 0.1f), Mathf.RoundToInt(floorTileCount * 0.2f)); // TODO: chagne to per flavour 
        await Task.Delay(100);

        DrawOuter();
        await Task.Delay(100);

        GameTiles.instance.SetUp();

        PlaceOuterAdditions();
        await Task.Delay(100);

        if (Random.Range(0, 2) == 0)
            LayoutObjectAtRandom(trap, trapPercent);


        SetupAstar();
        await Task.Delay(100);
        // TODO: spawn enemy chars;
        SpawnEnemies(3);
        // TODO: spawn player chars / set-up player spawn positions;
        await Task.Delay(100);

        CreatePlayerStartingArea();
    }

    void InitialSetup()
    {
        Random.InitState(seed);
        // TODO: change
        MovePointController.instance.transform.position = new Vector3(mapSize.x / 2, mapSize.y / 2);

        biome = biomes[Random.Range(0, biomes.Length)];

        pushableObstacles = new();

        backgroundTilemap.ClearAllTiles();
        middlegroundTilemap.ClearAllTiles();

        var tempList = envObjectsHolder.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
            DestroyImmediate(child.gameObject); // TODO: destory
    }

    void BoardSetup()
    {
        // +-1 because I am setting edge tiles to unwalkable edges
        for (int x = -1; x < mapSize.x + 1; x++)
            for (int y = -1; y < mapSize.y + 1; y++)
                backgroundTilemap.SetTile(new Vector3Int(x, y), biome.floorTiles[Random.Range(0, biome.floorTiles.Length)]);
    }

    void InitialiseOpenPositions()
    {
        openGridPositions.Clear();

        for (int x = 0; x < mapSize.x; x++)
            for (int y = 0; y < mapSize.y; y++)
                openGridPositions.Add(new Vector3Int(x, y, 0));

        openGridPositions = Utility.ShuffleList<Vector3Int>(openGridPositions, seed);
    }

    void ResolveMapVariant()
    {
        mapVariantChosen = mapVariants[Random.Range(0, mapVariants.Length)];
        obstaclePercent = Random.Range(mapVariantChosen.obstaclePercent.x, mapVariantChosen.obstaclePercent.y);
        terrainIrregularitiesPercent = Random.Range(mapVariantChosen.terrainIrregularitiesPercent.x,
                                                    mapVariantChosen.terrainIrregularitiesPercent.y);
        outerAdditionsPercent = Random.Range(biome.outerAdditionsPercent.x, biome.outerAdditionsPercent.y);
        trapPercent = Random.Range(mapVariantChosen.trapPercent.x,
                                            mapVariantChosen.trapPercent.y);

        Light2D l = Instantiate(globalLightPrefab, Vector3.zero, Quaternion.identity).GetComponent<Light2D>();
        l.transform.parent = envObjectsHolder.transform;
        l.color = biome.lightColor;
        l.intensity = biome.lightIntensity;

        if (mapVariantChosen.mapType == MapType.Circle)
            CarveCircle();
        if (mapVariantChosen.mapType == MapType.River)
            PlaceRiver();
        if (mapVariantChosen.mapType == MapType.Lake)
            PlaceLake();
        if (mapVariantChosen.mapType == MapType.Hourglass)
            CarveHourglass();
    }

    void PlaceTerrainIrregularities()
    {
        int irrCount = Mathf.FloorToInt((mapSize.x * 2 + mapSize.y * 2) * terrainIrregularitiesPercent);

        for (int i = 0; i < irrCount; i++)
        {
            int x = Random.Range(0, mapSize.x);
            int y = Random.Range(0, mapSize.y);

            // OK, this is weird, but I don't have a tile to handle map corners that look like:
            // [tile][empty][tile]
            // [tile][tile][empty]
            // [tile][tile][tile]
            // or
            // [empty][tile][tile]
            // [tile][tile][tile]
            // [tile][tile][empty]
            if (x == 1 || x == mapSize.x - 1)
                continue;
            if (y == 1 || y == mapSize.y - 1)
                continue;
            if (x == 2 || x == mapSize.x - 2)
                continue;
            if (y == 2 || y == mapSize.y - 2)
                continue;

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
        // go twice to smooth things over TODO: I am not certain if that's correct
        for (int i = 0; i < 2; i++)
            for (int x = -1; x < mapSize.x + 1; x++)
                for (int y = -1; y < mapSize.y + 1; y++)
                    ClearLooseTile(new Vector3Int(x, y));
    }

    async Task LayoutObstacles()
    {
        if (biome.obstacles.Length == 0)
            return;

        obstacleMap = new bool[mapSize.x, mapSize.y];

        // one of the tiles is always empty
        emptyTile = GetRandomOpenPosition(Vector2.one, openGridPositions)[0];
        openGridPositions.Remove(emptyTile);

        floorTileCount = 0;
        for (int x = 0; x <= mapSize.x; x++)
            for (int y = 0; y <= mapSize.y; y++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    floorTileCount++;

        int objectCount = Mathf.FloorToInt(floorTileCount * obstaclePercent);
        int obstacledTileCount = 0;
        for (int i = 0; i < objectCount; i++)
        {
            await Task.Delay(25);
            if (openGridPositions.Count <= 0)
                return;

            TilemapObject selectedObject = biome.obstacles[Random.Range(0, biome.obstacles.Length)];
            List<Vector3Int> randomPosition = GetRandomOpenPosition(selectedObject.size, openGridPositions);
            if (randomPosition == null)
                return;

            foreach (Vector3Int pos in randomPosition)
                obstacleMap[pos.x, pos.y] = true;
            obstacledTileCount += selectedObject.size.x * selectedObject.size.y;

            if (!MapIsFullyAccessible(obstacleMap, obstacledTileCount))
            {
                foreach (Vector3Int pos in randomPosition)
                    obstacleMap[pos.x, pos.y] = false;
                obstacledTileCount -= selectedObject.size.x * selectedObject.size.y;
                continue;
            }
            PlaceObject(selectedObject, randomPosition[0]);
        }
    }

    void PlaceSpecialObjects()
    {
        PlaceChest();
        PlaceCollectible();
    }

    void PlaceChest()
    {
        List<GameObject> shuffledObstacles = Utility.ShuffleList(pushableObstacles, seed);
        foreach (GameObject obs in shuffledObstacles)
        {
            Vector3Int tileSouth = new Vector3Int((int)obs.transform.position.x, (int)obs.transform.position.y - 1);
            if (!openGridPositions.Contains(tileSouth))
                continue;

            GameObject chest = Instantiate(chestPrefab, obs.transform.position, Quaternion.identity);
            chest.transform.parent = envObjectsHolder.transform;
            pushableObstacles.Remove(obs);
            DestroyImmediate(obs); // TODO: destroy immediate
            return;
        }
    }

    void PlaceCollectible()
    {
        if (pushableObstacles.Count == 0)
            return;
        GameObject chosenObstacle = pushableObstacles[Random.Range(0, pushableObstacles.Count)];
        GameObject collectible = Instantiate(collectiblePrefab, chosenObstacle.transform.position, Quaternion.identity);
        collectible.transform.parent = envObjectsHolder.transform;
    }

    void LayoutFloorAdditions(int minimum, int maximum)
    {
        TileBase[] tiles = biome.floorAdditions;
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            List<Vector3Int> randomPosiiton = GetRandomOpenPosition(Vector2.one, openGridPositions);
            middlegroundTilemap.SetTile(randomPosiiton[0], tiles[Random.Range(0, tiles.Length)]);
        }
    }

    /* --- MAP TYPES --- */
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
        int middleOfMap = Mathf.RoundToInt(mapSize.x * 0.5f);
        int riverWidth = Mathf.RoundToInt(Random.Range(1, mapSize.x * 0.2f));
        int min = middleOfMap - riverWidth;
        int max = middleOfMap + riverWidth;

        // river
        for (int x = min; x <= max; x++)
            for (int y = -1; y < mapSize.y + 1; y++) // -+1 to cover the edges
                ClearTile(new Vector3Int(x, y));

        // bridge
        int bridgeWidth = Random.Range(1, mapSize.y - 3);
        bridgeWidth = Mathf.Clamp(bridgeWidth, 1, 5);
        int bridgeY = Random.Range(2, mapSize.y - bridgeWidth - 1); // so the bridge can fit
        for (int x = min; x <= max; x++)
            for (int y = bridgeY - 1; y <= bridgeY + bridgeWidth; y++) // +-1 for edges
                SetBackgroundFloorTile(new Vector3Int(x, y));
    }

    void PlaceHorizontalRiver()
    {
        int middleOfMap = Mathf.RoundToInt(mapSize.y * 0.5f);
        int riverWidth = Mathf.RoundToInt(Random.Range(1, mapSize.y * 0.2f));
        int min = middleOfMap - riverWidth;
        int max = middleOfMap + riverWidth;

        // river
        for (int y = min; y <= max; y++)
            for (int x = -1; x < mapSize.x + 1; x++) // -+1 to cover the edges
                ClearTile(new Vector3Int(x, y));

        // bridge
        int bridgeWidth = Random.Range(1, mapSize.x - 3);
        bridgeWidth = Mathf.Clamp(bridgeWidth, 1, 5);
        int bridgeX = Random.Range(2, mapSize.x - bridgeWidth - 1); // so the bridge can fit
        for (int y = min; y <= max; y++)
            for (int x = bridgeX - 1; x <= bridgeX + bridgeWidth; x++) // +-1 for edges
                SetBackgroundFloorTile(new Vector3Int(x, y));
    }

    void PlaceLake()
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

    void CarveCircle()
    {
        // force a square - it works better
        if (mapSize.x > mapSize.y)
            mapSize.y = mapSize.x;
        else
            mapSize.x = mapSize.y;

        float centerX = mapSize.x * 0.5f;
        float centerY = mapSize.y * 0.5f;

        for (int x = -1; x < mapSize.x + 1; x++)
        {
            for (int y = -1; y < mapSize.y + 1; y++)
            {

                float sqrtX = (x - centerX) * (x - centerX);
                float sqrtY = (y - centerY) * (y - centerY);
                float sqrtR = centerX * centerX;

                if (sqrtX + sqrtY > sqrtR)
                    ClearTile(new Vector3Int(x, y));
            }
        }
    }

    void DrawOuter()
    {
        openOuterPositions.Clear();

        TileBase[] tiles = biome.outerTiles;

        for (int x = -mapSize.x; x < mapSize.x * 2; x++)
            for (int y = -mapSize.y; y < mapSize.y * 2; y++)
                if (!backgroundTilemap.GetTile(new Vector3Int(x, y)))
                {
                    backgroundTilemap.SetTile(new Vector3Int(x, y), tiles[Random.Range(0, tiles.Length)]);
                    openOuterPositions.Add(new Vector3Int(x, y));
                }
    }

    void PlaceOuterAdditions()
    {
        if (biome.outerAdditions.Length == 0)
            return;

        int outerAdditionsCount = Mathf.FloorToInt(openOuterPositions.Count * outerAdditionsPercent);
        for (int i = 0; i < outerAdditionsCount; i++)
        {
            if (openOuterPositions.Count <= 0)
                return;

            TilemapObject selectedObject = biome.outerAdditions[Random.Range(0, biome.outerAdditions.Length)];
            Vector3Int randomPosition = GetRandomOuterPosition(selectedObject.size);
            if (randomPosition == Vector3Int.zero) // TODO: ehh...
                continue;

            PlaceObject(selectedObject, randomPosition);
        }
    }

    Vector3Int GetRandomOuterPosition(Vector2Int _size)
    {
        Vector3Int randPos = openOuterPositions[Random.Range(0, openOuterPositions.Count)];

        // outer positions are not shuffled
        // + 1 coz if size is '1' I don't want to move any tiles down
        if (Array.IndexOf(biome.outerTiles, backgroundTilemap.GetTile(new Vector3Int(randPos.x - _size.x + 1, randPos.y))) == -1)
            return Vector3Int.zero;
        if (Array.IndexOf(biome.outerTiles, backgroundTilemap.GetTile(new Vector3Int(randPos.x, randPos.y - _size.y + 1))) == -1)
            return Vector3Int.zero;
        if (Array.IndexOf(biome.outerTiles, backgroundTilemap.GetTile(new Vector3Int(randPos.x - _size.x + 1, randPos.y - _size.y + 1))) == -1)
            return Vector3Int.zero;

        openOuterPositions.Remove(randPos);
        // remove positions from open outer positions;
        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                openOuterPositions.Remove(new Vector3Int(randPos.x - x, randPos.y - y));
            }
        }
        // need to check whether 
        return randPos;
    }

    void CarveHourglass()
    {
        // force a square - it only works on square maps
        if (mapSize.x > mapSize.y)
            mapSize.y = mapSize.x;
        else
            mapSize.x = mapSize.y;

        // I actually need +1 column on each side to create an edge
        int numberOfTilesLeft = 1;
        if (mapSize.x % 2 == 0)
            numberOfTilesLeft = 3;

        for (int y = Mathf.FloorToInt(mapSize.y * 0.5f); y < mapSize.y; y++)
        {
            numberOfTilesLeft += 2;
            for (int x = -1; x < mapSize.x + 1; x++) // +-1 to clear the edges
            {
                // I want to keep number of tiles in the middle
                int middleXTile = Mathf.RoundToInt(mapSize.x / 2);
                if (x < middleXTile - Mathf.FloorToInt(numberOfTilesLeft / 2))
                {
                    ClearTile(new Vector3Int(x, y));
                    ClearTile(new Vector3Int(x, mapSize.y - y));
                }
                if (x > middleXTile + Mathf.FloorToInt(numberOfTilesLeft / 2))
                {
                    ClearTile(new Vector3Int(x, y));
                    ClearTile(new Vector3Int(x, mapSize.y - y));
                }
            }
        }
    }

    // supports only 1x1 objects and GameObjects not tilemap objects
    void LayoutObjectAtRandom(GameObject _obj, float _density)
    {
        int objectCount = Mathf.FloorToInt(openGridPositions.Count * _density);
        for (int i = 0; i < objectCount; i++)
        {
            if (openGridPositions.Count <= 0)
                return;

            List<Vector3Int> randomPosition = GetRandomOpenPosition(Vector2.one, openGridPositions);
            if (randomPosition == null)
                return;

            GameObject o = Instantiate(_obj, new Vector3(randomPosition[0].x + 0.5f, randomPosition[0].y + 0.5f), Quaternion.identity);
            o.transform.parent = envObjectsHolder.transform;
        }
    }

    /* --- HELPERS --- */
    void PlaceObject(TilemapObject _obj, Vector3Int _pos)
    {
        GameObject selectedPrefab = obstaclePrefab;
        if (_obj.pushable)
            selectedPrefab = pushableObstaclePrefab;
        if (_obj.objectType == TileMapObjectType.Outer)
            selectedPrefab = outerObjectPrefab;

        // we are getting SE corner of the most NW tile of all positions and need to adjust the position to fit the tilemap
        float posX = _pos.x;
        float posY = _pos.y;

        if (_obj.size.x == 1)
            posX += 0.5f;
        if (_obj.size.y == 1)
            posY += 0.5f;

        if (_obj.size.x >= 3)
            posX -= 0.5f * (_obj.size.x - 2);
        if (_obj.size.y >= 3)
            posY -= 0.5f * (_obj.size.y - 2);

        Vector3 placingPos = new Vector3(posX, posY, _pos.z);

        GameObject ob = Instantiate(selectedPrefab, placingPos, Quaternion.identity);
        if (_obj.objectType == TileMapObjectType.Obstacle)
            ob.GetComponent<Obstacle>().Initialise(_obj);
        if (_obj.objectType == TileMapObjectType.Outer)
            ob.GetComponent<OuterObject>().Initialise(_obj);

        if (_obj.pushable)
            pushableObstacles.Add(ob);

        ob.name = _obj.name;
        ob.transform.parent = envObjectsHolder.transform;
    }

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
            SetEdgeTile(_pos, biome.edgeN);
        if (neighbours[0] && neighbours[1] && neighbours[2] && !neighbours[3])// edge S
            SetEdgeTile(_pos, biome.edgeS);
        if (!neighbours[0] && neighbours[1] && neighbours[2] && neighbours[3])// edge W
            SetEdgeTile(_pos, biome.edgeW);
        if (neighbours[0] && neighbours[1] && !neighbours[2] && neighbours[3])// edge E
            SetEdgeTile(_pos, biome.edgeE);

        // corners
        if (!neighbours[0] && !neighbours[1] && neighbours[2] && neighbours[3])// corner NW
            SetEdgeTile(_pos, biome.cornerNW);
        if (neighbours[0] && !neighbours[1] && !neighbours[2] && neighbours[3])// corner NE
            SetEdgeTile(_pos, biome.cornerNE);
        if (!neighbours[0] && neighbours[1] && neighbours[2] && !neighbours[3])// corner SW
            SetEdgeTile(_pos, biome.cornerSE);
        if (neighbours[0] && neighbours[1] && !neighbours[2] && !neighbours[3])// corner SE
            SetEdgeTile(_pos, biome.cornerSW);
    }

    void SetInlandCorners(Vector3Int _pos)
    {
        // I am only swapping tiles that exist
        if (backgroundTilemap.GetTile(_pos) == null)
            return;

        // TODO: this whole function seems verbose;
        // inland corner is surrounded by 2 edge tiles and 2 normal tiles
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

        // inland corners need to consider corners too
        if ((surroundingTiles[0] == biome.edgeN && surroundingTiles[1] == biome.edgeW)
            || (surroundingTiles[0] == biome.cornerNW && surroundingTiles[1] == biome.edgeW)
            || (surroundingTiles[0] == biome.cornerNW && surroundingTiles[1] == biome.cornerNW)
            || (surroundingTiles[0] == biome.edgeN && surroundingTiles[1] == biome.cornerNW))
            SetEdgeTile(_pos, biome.inlandCornerSW);

        if ((surroundingTiles[0] == biome.edgeS && surroundingTiles[3] == biome.edgeW)
            || (surroundingTiles[0] == biome.edgeS && surroundingTiles[3] == biome.cornerSE)
            || (surroundingTiles[0] == biome.cornerSE && surroundingTiles[3] == biome.edgeW)
            || (surroundingTiles[0] == biome.cornerSE && surroundingTiles[3] == biome.cornerSE))
            SetEdgeTile(_pos, biome.inlandCornerNW);

        if ((surroundingTiles[1] == biome.edgeE && surroundingTiles[2] == biome.edgeN)
            || (surroundingTiles[1] == biome.cornerNE && surroundingTiles[2] == biome.edgeN)
            || (surroundingTiles[1] == biome.edgeE && surroundingTiles[2] == biome.cornerNE)
            || (surroundingTiles[1] == biome.cornerNE && surroundingTiles[2] == biome.cornerNE))
            SetEdgeTile(_pos, biome.inlandCornerSE);

        if ((surroundingTiles[2] == biome.edgeS && surroundingTiles[3] == biome.edgeE)
            || (surroundingTiles[2] == biome.edgeS && surroundingTiles[3] == biome.cornerSW)
            || (surroundingTiles[2] == biome.cornerSW && surroundingTiles[3] == biome.edgeE)
            || (surroundingTiles[2] == biome.cornerSW && surroundingTiles[3] == biome.cornerSW))
            SetEdgeTile(_pos, biome.inlandCornerNE);
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

    void SetEdgeTile(Vector3Int _pos, TileBase _tile)
    {
        ClearTile(new Vector3Int(_pos.x, _pos.y));
        backgroundTilemap.SetTile(_pos, _tile);
    }

    void SetBackgroundFloorTile(Vector3Int _pos)
    {
        backgroundTilemap.SetTile(_pos, biome.floorTiles[Random.Range(0, biome.floorTiles.Length)]);
    }

    void ClearTile(Vector3Int _pos)
    {
        // It makes sure there won't be obstacles/map additions placed on that tile;
        openGridPositions.Remove(new Vector3Int(_pos.x, _pos.y, 0));
        backgroundTilemap.SetTile(new Vector3Int(_pos.x, _pos.y), null);
    }

    bool IsFloorTile(Vector3Int _pos)
    {
        return Array.IndexOf(biome.floorTiles, backgroundTilemap.GetTile(_pos)) != -1;
    }

    // TODO: improve this?
    List<Vector3Int> GetRandomOpenPosition(Vector2 _size, List<Vector3Int> _positions)
    {
        List<Vector3Int> candidatePositions = new();
        foreach (Vector3Int pos in _positions)
        {
            candidatePositions.Add(pos);
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    Vector3Int posToCheck = new Vector3Int(pos.x - x, pos.y - y, 0); // - to go North West on the map
                    if (_positions.Contains(posToCheck) && !candidatePositions.Contains(posToCheck))
                        candidatePositions.Add(posToCheck);
                }
            }

            if (candidatePositions.Count >= _size.x * _size.y)
            {
                foreach (Vector3Int posToRemove in candidatePositions)
                    _positions.Remove(posToRemove);

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

                if (_mapFlags[neighbourX, neighbourY])
                    continue;

                if (obstacleMap[neighbourX, neighbourY])
                    continue;

                if (!IsFloorTile(new Vector3Int(neighbourX, neighbourY)))
                    continue;

                floodFillQueue.Enqueue(new Vector3Int(neighbourX, neighbourY));
                accessibleTileCount++;
                _mapFlags[neighbourX, neighbourY] = true;
            }
        }
    }

    void SetupAstar()
    {
        AstarData data = AstarPath.active.data;

        data.DeserializeGraphs(graphData.bytes);
        GridGraph gg = data.gridGraph;

        // Setup a grid graph with some values
        int width = mapSize.x + 2;
        int depth = mapSize.y + 2;
        float nodeSize = 1;
        gg.center = new Vector3(mapSize.x / 2, mapSize.y / 2, 0);

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        // Scans all graphs
        AstarPath.active.Scan();
    }

    void SpawnEnemies(int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3Int randPos = GetRandomOpenPosition(Vector2.one, openGridPositions)[0];
            Vector3 pos = new Vector3(randPos.x + 0.5f, randPos.y + 0.5f);
            Character instantiatedSO = Instantiate(enemyCharacters[Random.Range(0, enemyCharacters.Length)]);
            GameObject newCharacter = Instantiate(enemyGO, pos, Quaternion.identity); // TODO: get random posiiton is wrong here

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
        Vector2 SWCorner = new Vector2(1, 1);
        Highlighter.instance.HighlightRectanglePlayer(SWCorner, 5, mapSize.y, Color.blue);
        TurnManager.instance.UpdateBattleState(BattleState.Preparation);
    }

}