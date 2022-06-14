using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using Pathfinding;

public enum EnemySpawnDirection { Left, Right, Top, Bottom }

// https://learn.unity.com/tutorial/level-generation?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f6
public class BoardManager : Singleton<BoardManager>
{
    // global
    GameManager _gameManager;
    HighlightManager _highlighter;
    TurnManager _turnManager;
    BattleManager _battleManger;
    AudioManager _audioManager;

    [Header("Map Setup")]
    BattleNode _battleNode;
    MapVariant _mapVariant;
    int _seed;
    public Vector2Int MapSize;

    [Header("Enemies")]
    [SerializeField] GameObject _enemyGO;
    List<EnemySpawnDirection> _allowedEnemySpawnDirections = new();
    public EnemySpawnDirection EnemySpawnDirection { get; private set; }

    [Header("Unity objects")]
    [SerializeField] Tilemap _backgroundTilemap;
    [SerializeField] Tilemap _middlegroundTilemap;
    [SerializeField] GameObject _envObjectsHolder;
    [SerializeField] GameObject _obstaclePrefab;
    [SerializeField] GameObject _pushableObstaclePrefab;
    [SerializeField] GameObject _outerObjectPrefab;
    [SerializeField] GameObject _collectiblePrefab;
    [SerializeField] GameObject _chestPrefab;
    [SerializeField] GameObject _mapResetPrefab;
    [SerializeField] GameObject _globalLightPrefab;
    [SerializeField] TextAsset _graphData;

    // other map vars
    Vector3Int _emptyTile;
    float _terrainIrregularitiesPercent;
    float _obstaclePercent;
    float _outerAdditionsPercent;
    bool[,] _obstacleMap;
    int _accessibleTileCount;
    Queue<Vector3Int> _floodFillQueue;
    List<Vector3Int> _openGridPositions = new();
    int _floorTileCount;
    TilemapBiome _biome;
    List<GameObject> _pushableObstacles;
    List<Vector3Int> _openOuterPositions = new();

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _highlighter = HighlightManager.Instance;
        _turnManager = TurnManager.Instance;
        _battleManger = BattleManager.Instance;
        _audioManager = AudioManager.Instance;

        _battleNode = (BattleNode)_gameManager.CurrentNode;
        _biome = _battleNode.Biome;
        _mapVariant = _battleNode.MapVariant;
        MapSize = _battleNode.MapSize;

        GenerateMap();
    }

    public async void GenerateMap()
    {
        _seed = System.DateTime.Now.Millisecond;

        // TODO: without the delays it breaks the game, 
        // preparation is called before map building, I just need a few miliseconds to set everything up.
        InitialSetup();
        BoardSetup();
        PlayAmbience();
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
        PlaceSpecialObjects();
        await Task.Delay(100);
        LayoutFloorAdditions(Mathf.RoundToInt(_floorTileCount * 0.1f), Mathf.RoundToInt(_floorTileCount * 0.2f)); // TODO: chagne to per flavour 
        await Task.Delay(100);
        DrawOuter();
        await Task.Delay(100);
        _battleManger.GetComponent<TileManager>().SetUp();
        PlaceOuterAdditions();
        await SetupAstar();
        await SpawnEnemies();
        PlaceMapReset();
        await Task.Delay(250);
        CreatePlayerStartingArea();
        PlayMusic();
    }

    void InitialSetup()
    {
        Random.InitState(_seed);

        _highlighter.ClearHighlightedTiles().GetAwaiter();
        _turnManager.UpdateBattleState(BattleState.MapBuilding);
        MovePointController.Instance.transform.position = new Vector3(MapSize.x / 2, MapSize.y / 2);

        _pushableObstacles = new();

        _backgroundTilemap.ClearAllTiles();
        _middlegroundTilemap.ClearAllTiles();

        var tempList = _envObjectsHolder.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList)
            Destroy(child.gameObject);
    }

    void BoardSetup()
    {
        // +-1 because I am setting edge tiles to unwalkable edges
        for (int x = -1; x < MapSize.x + 1; x++)
            for (int y = -1; y < MapSize.y + 1; y++)
                _backgroundTilemap.SetTile(new Vector3Int(x, y), _biome.FloorTiles[Random.Range(0, _biome.FloorTiles.Length)]);
    }

    void InitialiseOpenPositions()
    {
        _openGridPositions.Clear();

        for (int x = 0; x < MapSize.x; x++)
            for (int y = 0; y < MapSize.y; y++)
                _openGridPositions.Add(new Vector3Int(x, y, 0));

        _openGridPositions = Utility.ShuffleList<Vector3Int>(_openGridPositions, _seed);
    }

    void ResolveMapVariant()
    {
        _obstaclePercent = Random.Range(_mapVariant.ObstaclePercent.x, _mapVariant.ObstaclePercent.y);
        _terrainIrregularitiesPercent = Random.Range(_mapVariant.TerrainIrregularitiesPercent.x,
                                                    _mapVariant.TerrainIrregularitiesPercent.y);
        _outerAdditionsPercent = Random.Range(_biome.OuterAdditionsPercent.x, _biome.OuterAdditionsPercent.y);

        Light2D globalLight = Instantiate(_globalLightPrefab, Vector3.zero, Quaternion.identity).GetComponent<Light2D>();
        globalLight.transform.parent = _envObjectsHolder.transform;
        globalLight.color = _biome.LightColor;
        globalLight.intensity = _biome.LightIntensity;
        LightManager.Instance.Initialize(globalLight);

        // TODO: Correct? I want to remove some values on some map types - river / hourglass
        _allowedEnemySpawnDirections = System.Enum.GetValues(typeof(EnemySpawnDirection)).Cast<EnemySpawnDirection>().ToList();

        if (_mapVariant.MapType == MapType.Circle)
            CarveCircle();
        if (_mapVariant.MapType == MapType.River)
            PlaceRiver();
        if (_mapVariant.MapType == MapType.Lake)
            PlaceLake();
        if (_mapVariant.MapType == MapType.Hourglass)
            CarveHourglass();
    }

    void PlaceTerrainIrregularities()
    {
        int irrCount = Mathf.FloorToInt((MapSize.x * 2 + MapSize.y * 2) * _terrainIrregularitiesPercent);

        for (int i = 0; i < irrCount; i++)
        {
            int x = Random.Range(0, MapSize.x);
            int y = Random.Range(0, MapSize.y);

            // pick edge
            int edgeX = -1;
            int edgeY = -1;
            if (Random.Range(0, 2) == 1)
            {
                edgeX = MapSize.x;
                edgeY = MapSize.y;
            }
            Vector3Int tileToClear = new();
            if (Random.Range(0, 2) == 0)
                tileToClear = new Vector3Int(x, edgeY);
            else
                tileToClear = new Vector3Int(edgeX, y);

            if (CanPlaceTerrainIrregularity(tileToClear))
                ClearTile(tileToClear);
        }
    }

    bool CanPlaceTerrainIrregularity(Vector3Int tile)
    {
        // OK, this is weird, but I don't have a tile to handle map corners that look like:
        // [tile][empty][tile]
        // [tile][tile][empty]
        // [tile][tile][tile]
        // or
        // [empty][tile][tile]
        // [tile][tile][tile]
        // [tile][tile][empty]
        if (tile.x == 1 || tile.x == MapSize.x - 1)
            return false;
        if (tile.y == 1 || tile.y == MapSize.y - 1)
            return false;
        if (tile.x == 2 || tile.x == MapSize.x - 2)
            return false;
        if (tile.y == 2 || tile.y == MapSize.y - 2)
            return false;

        // TODO: https://www.notion.so/455f7c47ef3747d68f1daf1bb00dcb16?v=f18df9eaf80e4f258bef240b3b9e1ed5&p=9169adc9288840569e8a315aec224f6d
        // also, characters can get stuck because of terrain irregularities 
        // so, I need to check whether map is fully accessible before placing irregularity;
        // so, to place I need to check whether there are at least 4 tiles in the row / column where irregularity will be placed
        // but it can be a bit smarter coz I am placing them on the edges so I will only need to check up/down left/right
        // depending on which edge
        // ok, ok, ok
        // need to check 

        return true;

    }

    void HandleEdge()
    {
        for (int x = -1; x < MapSize.x + 1; x++)
            for (int y = -1; y < MapSize.y + 1; y++)
                SetEdges(new Vector3Int(x, y));

        for (int x = -1; x < MapSize.x + 1; x++)
            for (int y = -1; y < MapSize.y + 1; y++)
                SetInlandCorners(new Vector3Int(x, y));
    }

    void HandleLooseTiles()
    {
        // go twice to smooth things over TODO: I am not certain if that's correct
        for (int i = 0; i < 2; i++)
            for (int x = -1; x < MapSize.x + 1; x++)
                for (int y = -1; y < MapSize.y + 1; y++)
                    ClearLooseTile(new Vector3Int(x, y));
    }

    async Task LayoutObstacles()
    {
        if (_biome.Obstacles.Length == 0)
            return;

        _obstacleMap = new bool[MapSize.x, MapSize.y];

        // one of the tiles is always empty
        _emptyTile = GetRandomOpenPosition(Vector2.one, _openGridPositions)[0];
        _openGridPositions.Remove(_emptyTile);

        _floorTileCount = 0;
        for (int x = 0; x <= MapSize.x; x++)
            for (int y = 0; y <= MapSize.y; y++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    _floorTileCount++;

        int objectCount = Mathf.FloorToInt(_floorTileCount * _obstaclePercent);
        int obstacledTileCount = 0;
        for (int i = 0; i < objectCount; i++)
        {
            if (_openGridPositions.Count <= 0)
                return;

            TilemapObject selectedObject = _biome.Obstacles[Random.Range(0, _biome.Obstacles.Length)];
            List<Vector3Int> randomPosition = GetRandomOpenPosition(selectedObject.Size, _openGridPositions);
            if (randomPosition == null)
                return;

            foreach (Vector3Int pos in randomPosition)
                _obstacleMap[pos.x, pos.y] = true;
            obstacledTileCount += selectedObject.Size.x * selectedObject.Size.y;

            if (!MapIsFullyAccessible(_obstacleMap, obstacledTileCount))
            {
                foreach (Vector3Int pos in randomPosition)
                    _obstacleMap[pos.x, pos.y] = false;
                obstacledTileCount -= selectedObject.Size.x * selectedObject.Size.y;
                continue;
            }
            await Task.Delay(30);

            PlaceObject(selectedObject, randomPosition[0]);
        }
        await Task.Delay(10);
    }

    void PlaceSpecialObjects()
    {
        PlaceChest();
        PlaceCollectible();
    }

    void PlaceChest()
    {
        List<GameObject> shuffledObstacles = Utility.ShuffleList(_pushableObstacles, _seed);
        foreach (GameObject obs in shuffledObstacles)
        {
            Vector3Int tileSouth = new Vector3Int((int)obs.transform.position.x, (int)obs.transform.position.y - 1);
            if (!_openGridPositions.Contains(tileSouth))
                continue;

            GameObject chest = Instantiate(_chestPrefab, obs.transform.position, Quaternion.identity);
            chest.transform.parent = _envObjectsHolder.transform;
            _pushableObstacles.Remove(obs);
            Destroy(obs);
            return;
        }
    }

    void PlaceCollectible()
    {
        if (_pushableObstacles.Count == 0)
            return;
        GameObject chosenObstacle = _pushableObstacles[Random.Range(0, _pushableObstacles.Count)];
        GameObject collectible = Instantiate(_collectiblePrefab, chosenObstacle.transform.position, Quaternion.identity);
        collectible.transform.parent = _envObjectsHolder.transform;
    }

    void LayoutFloorAdditions(int minimum, int maximum)
    {
        TileBase[] tiles = _biome.FloorAdditions;
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            if (_openGridPositions.Count <= 0)
                return;

            List<Vector3Int> randomPosiiton = GetRandomOpenPosition(Vector2.one, _openGridPositions);
            _middlegroundTilemap.SetTile(randomPosiiton[0], tiles[Random.Range(0, tiles.Length)]);
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
        _allowedEnemySpawnDirections.Remove(EnemySpawnDirection.Top);
        _allowedEnemySpawnDirections.Remove(EnemySpawnDirection.Bottom);

        int middleOfMap = Mathf.RoundToInt(MapSize.x * 0.5f);
        int riverWidth = Mathf.RoundToInt(Random.Range(1, MapSize.x * 0.2f));
        int min = middleOfMap - riverWidth;
        int max = middleOfMap + riverWidth;

        // river
        for (int x = min; x <= max; x++)
            for (int y = -1; y < MapSize.y + 1; y++) // -+1 to cover the edges
                ClearTile(new Vector3Int(x, y));

        // bridge
        int bridgeWidth = Random.Range(1, MapSize.y - 3);
        bridgeWidth = Mathf.Clamp(bridgeWidth, 1, 5);
        int bridgeY = Random.Range(2, MapSize.y - bridgeWidth - 1); // so the bridge can fit
        for (int x = min; x <= max; x++)
            for (int y = bridgeY - 1; y <= bridgeY + bridgeWidth; y++) // +-1 for edges
                SetBackgroundFloorTile(new Vector3Int(x, y));
    }

    void PlaceHorizontalRiver()
    {
        _allowedEnemySpawnDirections.Remove(EnemySpawnDirection.Left);
        _allowedEnemySpawnDirections.Remove(EnemySpawnDirection.Right);

        int middleOfMap = Mathf.RoundToInt(MapSize.y * 0.5f);
        int riverWidth = Mathf.RoundToInt(Random.Range(1, MapSize.y * 0.2f));
        int min = middleOfMap - riverWidth;
        int max = middleOfMap + riverWidth;

        // river
        for (int y = min; y <= max; y++)
            for (int x = -1; x < MapSize.x + 1; x++) // -+1 to cover the edges
                ClearTile(new Vector3Int(x, y));

        // bridge
        int bridgeWidth = Random.Range(1, MapSize.x - 3);
        bridgeWidth = Mathf.Clamp(bridgeWidth, 1, 5);
        int bridgeX = Random.Range(2, MapSize.x - bridgeWidth - 1); // so the bridge can fit
        for (int y = min; y <= max; y++)
            for (int x = bridgeX - 1; x <= bridgeX + bridgeWidth; x++) // +-1 for edges
                SetBackgroundFloorTile(new Vector3Int(x, y));
    }

    void PlaceLake()
    {
        int lakeWidth = Mathf.RoundToInt(Random.Range(MapSize.x * 0.2f, MapSize.x * 0.4f));
        int lakeHeight = Mathf.RoundToInt(Random.Range(MapSize.x * 0.2f, MapSize.y * 0.4f));

        // -+1 to cover the edges
        int xMin = Mathf.RoundToInt((MapSize.x - lakeWidth) * 0.5f) + 1;
        int yMin = Mathf.RoundToInt((MapSize.y - lakeHeight) * 0.5f) + 1;

        int xMax = xMin + lakeWidth - 2;
        int yMax = yMin + lakeHeight - 2;

        for (int x = xMin; x <= xMax; x++)
            for (int y = yMin; y <= yMax; y++)
                ClearTile(new Vector3Int(x, y));
    }

    void CarveCircle()
    {
        // force a square - it works better
        if (MapSize.x > MapSize.y)
            MapSize.y = MapSize.x;
        else
            MapSize.x = MapSize.y;

        float centerX = MapSize.x * 0.5f;
        float centerY = MapSize.y * 0.5f;

        for (int x = -1; x < MapSize.x + 1; x++)
        {
            for (int y = -1; y < MapSize.y + 1; y++)
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
        _openOuterPositions.Clear();

        TileBase[] tiles = _biome.OuterTiles;

        for (int x = -MapSize.x; x < MapSize.x * 2; x++)
            for (int y = -MapSize.y; y < MapSize.y * 2; y++)
                if (!_backgroundTilemap.GetTile(new Vector3Int(x, y)))
                {
                    _backgroundTilemap.SetTile(new Vector3Int(x, y), tiles[Random.Range(0, tiles.Length)]);
                    _openOuterPositions.Add(new Vector3Int(x, y));
                }
    }

    void PlaceOuterAdditions()
    {
        if (_biome.OuterAdditions.Length == 0)
            return;

        int outerAdditionsCount = Mathf.FloorToInt(_openOuterPositions.Count * _outerAdditionsPercent);
        for (int i = 0; i < outerAdditionsCount; i++)
        {
            if (_openOuterPositions.Count <= 0)
                return;

            TilemapObject selectedObject = _biome.OuterAdditions[Random.Range(0, _biome.OuterAdditions.Length)];
            Vector3Int randomPosition = GetRandomOuterPosition(selectedObject.Size);
            if (randomPosition == Vector3Int.zero) // TODO: ehh...
                continue;

            PlaceObject(selectedObject, randomPosition);
        }
    }

    Vector3Int GetRandomOuterPosition(Vector2Int _size)
    {
        Vector3Int randPos = _openOuterPositions[Random.Range(0, _openOuterPositions.Count)];

        // outer positions are not shuffled
        // + 1 coz if size is '1' I don't want to move any tiles down
        if (Array.IndexOf(_biome.OuterTiles, _backgroundTilemap.GetTile(new Vector3Int(randPos.x - _size.x + 1, randPos.y))) == -1)
            return Vector3Int.zero;
        if (Array.IndexOf(_biome.OuterTiles, _backgroundTilemap.GetTile(new Vector3Int(randPos.x, randPos.y - _size.y + 1))) == -1)
            return Vector3Int.zero;
        if (Array.IndexOf(_biome.OuterTiles, _backgroundTilemap.GetTile(new Vector3Int(randPos.x - _size.x + 1, randPos.y - _size.y + 1))) == -1)
            return Vector3Int.zero;

        _openOuterPositions.Remove(randPos);
        // remove positions from open outer positions;
        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                _openOuterPositions.Remove(new Vector3Int(randPos.x - x, randPos.y - y));
            }
        }
        // need to check whether 
        return randPos;
    }

    void CarveHourglass()
    {
        // it's always vertical
        _allowedEnemySpawnDirections.Remove(EnemySpawnDirection.Left);
        _allowedEnemySpawnDirections.Remove(EnemySpawnDirection.Right);

        // force a square - it only works on square maps
        if (MapSize.x > MapSize.y)
            MapSize.y = MapSize.x;
        else
            MapSize.x = MapSize.y;

        // I actually need +1 column on each side to create an edge
        int numberOfTilesLeft = 1;
        if (MapSize.x % 2 == 0)
            numberOfTilesLeft = 3;

        for (int y = Mathf.FloorToInt(MapSize.y * 0.5f); y < MapSize.y; y++)
        {
            numberOfTilesLeft += 2;
            for (int x = -1; x < MapSize.x + 1; x++) // +-1 to clear the edges
            {
                // I want to keep number of tiles in the middle
                int middleXTile = Mathf.RoundToInt(MapSize.x / 2);
                if (x < middleXTile - Mathf.FloorToInt(numberOfTilesLeft / 2))
                {
                    ClearTile(new Vector3Int(x, y));
                    ClearTile(new Vector3Int(x, MapSize.y - y));
                }
                if (x > middleXTile + Mathf.FloorToInt(numberOfTilesLeft / 2))
                {
                    ClearTile(new Vector3Int(x, y));
                    ClearTile(new Vector3Int(x, MapSize.y - y));
                }
            }
        }
    }

    // supports only 1x1 objects and GameObjects not tilemap objects
    void LayoutObjectAtRandom(GameObject _obj, float _density)
    {
        int objectCount = Mathf.FloorToInt(_openGridPositions.Count * _density);
        for (int i = 0; i < objectCount; i++)
        {
            if (_openGridPositions.Count <= 0)
                return;

            List<Vector3Int> randomPosition = GetRandomOpenPosition(Vector2.one, _openGridPositions);
            if (randomPosition == null)
                return;

            GameObject o = Instantiate(_obj, new Vector3(randomPosition[0].x + 0.5f, randomPosition[0].y + 0.5f), Quaternion.identity);
            o.transform.parent = _envObjectsHolder.transform;
        }
    }

    /* --- HELPERS --- */
    void PlaceObject(TilemapObject _obj, Vector3Int _pos)
    {
        GameObject selectedPrefab = _obstaclePrefab;
        if (_obj.IsPushable)
            selectedPrefab = _pushableObstaclePrefab;
        if (_obj.ObjectType == TileMapObjectType.Outer)
            selectedPrefab = _outerObjectPrefab;

        // we are getting SE corner of the most NW tile of all positions and need to adjust the position to fit the tilemap
        float posX = _pos.x;
        float posY = _pos.y;

        if (_obj.Size.x == 1)
            posX += 0.5f;
        if (_obj.Size.y == 1)
            posY += 0.5f;

        if (_obj.Size.x >= 3)
            posX -= 0.5f * (_obj.Size.x - 2);
        if (_obj.Size.y >= 3)
            posY -= 0.5f * (_obj.Size.y - 2);

        Vector3 placingPos = new Vector3(posX, posY, _pos.z);

        GameObject ob = Instantiate(selectedPrefab, placingPos, Quaternion.identity);
        if (_obj.ObjectType == TileMapObjectType.Obstacle)
            ob.GetComponent<Obstacle>().Initialise(_obj);
        if (_obj.ObjectType == TileMapObjectType.Outer)
            ob.GetComponent<OuterObject>().Initialise(_obj);

        if (_obj.IsPushable)
            _pushableObstacles.Add(ob);

        ob.name = _obj.name;
        ob.transform.parent = _envObjectsHolder.transform;
    }

    void SetEdges(Vector3Int _pos)
    {
        // I am swapping tiles that exist
        if (_backgroundTilemap.GetTile(_pos) == null)
            return;

        bool[] neighbours = new bool[4]; // left, top, right, bottom 

        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y)) != null)
            neighbours[0] = true;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1)) != null)
            neighbours[1] = true;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y)) != null)
            neighbours[2] = true;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1)) != null)
            neighbours[3] = true;

        // TODO: this seems stupid
        if (neighbours[0] && !neighbours[1] && neighbours[2] && neighbours[3])// edge N
            SetEdgeTile(_pos, _biome.EdgeN);
        if (neighbours[0] && neighbours[1] && neighbours[2] && !neighbours[3])// edge S
            SetEdgeTile(_pos, _biome.EdgeS);
        if (!neighbours[0] && neighbours[1] && neighbours[2] && neighbours[3])// edge W
            SetEdgeTile(_pos, _biome.EdgeW);
        if (neighbours[0] && neighbours[1] && !neighbours[2] && neighbours[3])// edge E
            SetEdgeTile(_pos, _biome.EdgeE);

        // corners
        if (!neighbours[0] && !neighbours[1] && neighbours[2] && neighbours[3])// corner NW
            SetEdgeTile(_pos, _biome.CornerNW);
        if (neighbours[0] && !neighbours[1] && !neighbours[2] && neighbours[3])// corner NE
            SetEdgeTile(_pos, _biome.CornerNE);
        if (!neighbours[0] && neighbours[1] && neighbours[2] && !neighbours[3])// corner SW
            SetEdgeTile(_pos, _biome.CornerSE);
        if (neighbours[0] && neighbours[1] && !neighbours[2] && !neighbours[3])// corner SE
            SetEdgeTile(_pos, _biome.CornerSW);
    }

    void SetInlandCorners(Vector3Int _pos)
    {
        // I am only swapping tiles that exist
        if (_backgroundTilemap.GetTile(_pos) == null)
            return;

        // TODO: this whole function seems verbose;
        // inland corner is surrounded by 2 edge tiles and 2 normal tiles
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y)) == null)
            return;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1)) == null)
            return;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y)) == null)
            return;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1)) == null)
            return;

        TileBase[] surroundingTiles = new TileBase[4]; // left, top, right, bottom 
        surroundingTiles[0] = _backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y));
        surroundingTiles[1] = _backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1));
        surroundingTiles[2] = _backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y));
        surroundingTiles[3] = _backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1));

        // inland corners need to consider corners too
        if ((surroundingTiles[0] == _biome.EdgeN && surroundingTiles[1] == _biome.EdgeW)
            || (surroundingTiles[0] == _biome.CornerNW && surroundingTiles[1] == _biome.EdgeW)
            || (surroundingTiles[0] == _biome.CornerNW && surroundingTiles[1] == _biome.CornerNW)
            || (surroundingTiles[0] == _biome.EdgeN && surroundingTiles[1] == _biome.CornerNW))
            SetEdgeTile(_pos, _biome.InlandCornerSW);

        if ((surroundingTiles[0] == _biome.EdgeS && surroundingTiles[3] == _biome.EdgeW)
            || (surroundingTiles[0] == _biome.EdgeS && surroundingTiles[3] == _biome.CornerSE)
            || (surroundingTiles[0] == _biome.CornerSE && surroundingTiles[3] == _biome.EdgeW)
            || (surroundingTiles[0] == _biome.CornerSE && surroundingTiles[3] == _biome.CornerSE))
            SetEdgeTile(_pos, _biome.InlandCornerNW);

        if ((surroundingTiles[1] == _biome.EdgeE && surroundingTiles[2] == _biome.EdgeN)
            || (surroundingTiles[1] == _biome.CornerNE && surroundingTiles[2] == _biome.EdgeN)
            || (surroundingTiles[1] == _biome.EdgeE && surroundingTiles[2] == _biome.CornerNE)
            || (surroundingTiles[1] == _biome.CornerNE && surroundingTiles[2] == _biome.CornerNE))
            SetEdgeTile(_pos, _biome.InlandCornerSE);

        if ((surroundingTiles[2] == _biome.EdgeS && surroundingTiles[3] == _biome.EdgeE)
            || (surroundingTiles[2] == _biome.EdgeS && surroundingTiles[3] == _biome.CornerSW)
            || (surroundingTiles[2] == _biome.CornerSW && surroundingTiles[3] == _biome.EdgeE)
            || (surroundingTiles[2] == _biome.CornerSW && surroundingTiles[3] == _biome.CornerSW))
            SetEdgeTile(_pos, _biome.InlandCornerNE);
    }

    void ClearLooseTile(Vector3Int _pos)
    {
        int voidTiles = 0;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x + 1, _pos.y)) == null)
            voidTiles++;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x - 1, _pos.y)) == null)
            voidTiles++;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y + 1)) == null)
            voidTiles++;
        if (_backgroundTilemap.GetTile(new Vector3Int(_pos.x, _pos.y - 1)) == null)
            voidTiles++;

        if (voidTiles >= 3)
            ClearTile(_pos);
    }

    void SetEdgeTile(Vector3Int _pos, TileBase _tile)
    {
        ClearTile(new Vector3Int(_pos.x, _pos.y));
        _backgroundTilemap.SetTile(_pos, _tile);
    }

    void SetBackgroundFloorTile(Vector3Int _pos)
    {
        _backgroundTilemap.SetTile(_pos, _biome.FloorTiles[Random.Range(0, _biome.FloorTiles.Length)]);
    }

    void ClearTile(Vector3Int _pos)
    {
        // It makes sure there won't be obstacles/map additions placed on that tile;
        _openGridPositions.Remove(new Vector3Int(_pos.x, _pos.y, 0));
        _backgroundTilemap.SetTile(new Vector3Int(_pos.x, _pos.y), null);
    }

    bool IsFloorTile(Vector3Int _pos)
    {
        return Array.IndexOf(_biome.FloorTiles, _backgroundTilemap.GetTile(_pos)) != -1;
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
        // flood fill algo from empty tile that stays free;
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        mapFlags[_emptyTile.x, _emptyTile.y] = true; // always free

        _floodFillQueue = new();
        _floodFillQueue.Enqueue(_emptyTile); // always free
        _accessibleTileCount = 1; // one tile is always free

        while (_floodFillQueue.Count > 0)
            CheckAccessibility(_floodFillQueue.Dequeue(), mapFlags);

        int targetAccessibleTileCount = _floorTileCount - currentObstacleCount;
        return targetAccessibleTileCount == _accessibleTileCount;
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

                if (!(neighbourX >= 0 && neighbourX < _obstacleMap.GetLength(0) &&
                    neighbourY >= 0 && neighbourY < _obstacleMap.GetLength(1)))
                    continue; // make sure it is in bounds

                if (_mapFlags[neighbourX, neighbourY])
                    continue;

                if (_obstacleMap[neighbourX, neighbourY])
                    continue;

                if (!IsFloorTile(new Vector3Int(neighbourX, neighbourY)))
                    continue;

                _floodFillQueue.Enqueue(new Vector3Int(neighbourX, neighbourY));
                _accessibleTileCount++;
                _mapFlags[neighbourX, neighbourY] = true;
            }
        }
    }

    async Task SetupAstar()
    {
        AstarData data = AstarPath.active.data;

        data.DeserializeGraphs(_graphData.bytes);
        GridGraph gg = data.gridGraph;

        // Setup a grid graph with some values
        int width = MapSize.x + 2;
        int depth = MapSize.y + 2;
        float nodeSize = 1;
        gg.center = new Vector3(MapSize.x / 2f, MapSize.y / 2f, 0);

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        // Scans all graphs
        AstarPath.active.Scan();

        await Task.Delay(10);
    }

    async Task SpawnEnemies()
    {
        EnemySpawnDirection = _allowedEnemySpawnDirections[Random.Range(0, _allowedEnemySpawnDirections.Count)];

        foreach (Brain brain in _battleNode.Enemies)
        {
            // TODO: this is wrong
            Vector3Int randomPos = GetRandomOpenPosition(Vector2.one, _openGridPositions)[0];
            Vector3Int chosenPos = randomPos;
            foreach (Vector3Int pos in _openGridPositions)
            {
                if (EnemySpawnDirection == EnemySpawnDirection.Left && pos.x <= GetLeftmostColumnIndex() + 3)
                    chosenPos = pos;
                if (EnemySpawnDirection == EnemySpawnDirection.Top && pos.y >= GetMostTopRowIndex() - 3)
                    chosenPos = pos;
                if (EnemySpawnDirection == EnemySpawnDirection.Right && pos.x >= GetRightmostColumnIndex() - 3)
                    chosenPos = pos;
                if (EnemySpawnDirection == EnemySpawnDirection.Bottom && pos.y <= GetMostBottomRowIndex() + 3)
                    chosenPos = pos;

                if (chosenPos != randomPos)
                {
                    _openGridPositions.Remove(chosenPos);
                    break;
                }
            }

            EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();

            int playerLevel = _gameManager.PlayerTroops[0].Level;
            enemySO.CreateEnemy(playerLevel + 2, brain);

            Vector3 spawnPos = new Vector3(chosenPos.x + 0.5f, chosenPos.y + 0.5f);
            Character instantiatedSO = Instantiate(enemySO);
            GameObject newCharacter = Instantiate(_enemyGO, spawnPos, Quaternion.identity);

            instantiatedSO.Initialize(newCharacter);
            newCharacter.name = instantiatedSO.CharacterName;
            newCharacter.transform.parent = _envObjectsHolder.transform;

            newCharacter.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

            // face correct dir
            CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();

            if (EnemySpawnDirection == EnemySpawnDirection.Left)
                characterRendererManager.Face(Vector2.right);
            if (EnemySpawnDirection == EnemySpawnDirection.Top)
                characterRendererManager.Face(Vector2.down);
            if (EnemySpawnDirection == EnemySpawnDirection.Right)
                characterRendererManager.Face(Vector2.left);
            if (EnemySpawnDirection == EnemySpawnDirection.Bottom)
                characterRendererManager.Face(Vector2.up);

            characterRendererManager.Face(Vector2.zero);
        }

        await Task.Delay(10);
    }

    void PlaceMapReset()
    {
        GameObject mr = Instantiate(_mapResetPrefab, new Vector3(-MapSize.x + 0.5f, -MapSize.y + 0.5f), Quaternion.identity);
        mr.transform.parent = _envObjectsHolder.transform;
    }

    async void CreatePlayerStartingArea()
    {
        Vector2 SWCorner = Vector2.zero;
        int width = 0;
        int height = 0;
        if (EnemySpawnDirection == EnemySpawnDirection.Left)
        {
            width = 3;
            height = MapSize.y;
            SWCorner = new Vector2(GetRightmostColumnIndex() - width + 1, 0);
        }
        if (EnemySpawnDirection == EnemySpawnDirection.Top)
        {
            width = MapSize.x;
            height = 3;
            SWCorner = new Vector2(0, GetMostBottomRowIndex());
        }
        if (EnemySpawnDirection == EnemySpawnDirection.Right)
        {
            width = 3;
            height = MapSize.y;
            SWCorner = new Vector2(GetLeftmostColumnIndex(), 0);
        }
        if (EnemySpawnDirection == EnemySpawnDirection.Bottom)
        {
            width = MapSize.x;
            height = 3;
            SWCorner = new Vector2(0, GetMostTopRowIndex() - height + 1);
        }

        await _highlighter.HighlightRectanglePlayer(SWCorner, width, height, Color.blue);
        // TODO: this is wrong
        if (!IsEnoughSpaceToDeploy())
            await _highlighter.HighlightRectanglePlayer(SWCorner, width + 2, height + 2, Color.blue);

        _turnManager.UpdateBattleState(BattleState.Deployment);
    }

    int GetRightmostColumnIndex()
    {
        for (int x = MapSize.x; x > 0; x--)
            for (int y = 0; y <= MapSize.y; y++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    return x;

        return 0;
    }

    int GetLeftmostColumnIndex()
    {
        for (int x = 0; x < MapSize.x; x++)
            for (int y = 0; y <= MapSize.y; y++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    return x;

        return 0;
    }

    int GetMostTopRowIndex()
    {
        for (int y = MapSize.y; y > 0; y--)
            for (int x = 0; x <= MapSize.x; x++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    return y;

        return 0;
    }

    int GetMostBottomRowIndex()
    {
        for (int y = 0; y < MapSize.y; y++)
            for (int x = 0; x <= MapSize.x; x++)
                if (IsFloorTile(new Vector3Int(x, y)))
                    return y;

        return 0;
    }

    bool IsEnoughSpaceToDeploy()
    {
        return _highlighter.HighlightedTiles.Count > 6;
    }

    public void AddEnvObject(Transform t)
    {
        t.parent = _envObjectsHolder.transform;
    }
    
    public void PlayAmbience()
    {
        _audioManager.PlayAmbience(_biome.Ambience);
    }

    public void PlayMusic()
    {
        _audioManager.PlayMusic(_biome.Music);
    }
}
