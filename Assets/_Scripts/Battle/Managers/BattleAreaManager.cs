using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

namespace Lis
{
    public class BattleAreaManager : MonoBehaviour
    {
        [SerializeField] Transform _floorHolder;
        [SerializeField] GameObject _floorPrefab;

        [SerializeField] GameObject _tilePrefab;
        [SerializeField] Building _homeBuilding;
        public List<Building> _unlockedBuildings = new();

        [SerializeField] Quest[] _quests;
        readonly List<Quest> _pastQuests = new();

        [SerializeField] List<GameObject> _cornerTileIndicators = new();

        GameObject _floor;

        [HideInInspector] public BattleTile HomeTile;

        [HideInInspector] public List<BattleTile> CornerTiles = new();

        readonly List<BattleTile> _tiles = new();

        [FormerlySerializedAs("SecuredTiles")] [HideInInspector]
        public List<BattleTile> UnlockedTiles = new();

        VisualElement _questContainer;

        public event Action<BattleTile> OnTileSecured;

        public void Initialize()
        {
            float tileScale = _floorPrefab.transform.localScale.x;
            _floor = Instantiate(_floorPrefab,
                new Vector3(-tileScale * 0.5f, 0, -tileScale * 0.5f), // floor offset to make tiles centered
                Quaternion.identity);
            _floor.transform.SetParent(_floorHolder);

            _unlockedBuildings = GameManager.Instance.GameDatabase.GetUnlockedBuildings();

            InitializeUI();
            CreateArea();
        }

        void InitializeUI()
        {
            _questContainer =
                GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("activeQuestsContainer");
        }

        void CreateArea()
        {
            // TODO: create area magic numbers
            // current set-up only works when
            // I get 100 tiles with surface scaled to floor scale
            float tileScale = _floorPrefab.transform.localScale.x;
            for (int x = -5; x < 5; x++)
            {
                for (int z = -5; z < 5; z++)
                {
                    Vector3 pos = new(x * tileScale, 0, z * tileScale);
                    Building building = _unlockedBuildings[Random.Range(0, _unlockedBuildings.Count)];
                    if (pos == Vector3.zero)
                        building = _homeBuilding;

                    BattleTile bt = InstantiateTile(pos, building);
                    bt.OnUnlocked += TileUnlocked;
                    _tiles.Add(bt);

                    if (pos == Vector3.zero)
                        HomeTile = bt;

                    // put corner tiles into the list
                    if (pos.x == -5 * tileScale && pos.z == -5 * tileScale ||
                        pos.x == -5 * tileScale && pos.z == 4 * tileScale ||
                        pos.x == 4 * tileScale && pos.z == -5 * tileScale ||
                        pos.x == 4 * tileScale && pos.z == 4 * tileScale)
                    {
                        CornerTiles.Add(bt);
                    }
                }
            }


            SetCornerTileIndicators();
        }


        void SetCornerTileIndicators()
        {
            for (int i = 0; i < 4; i++)
                Instantiate(_cornerTileIndicators[i], CornerTiles[i].transform.position,
                    Quaternion.Euler(270f, 0, 0));
        }


        public void SecureHomeTile()
        {
            HomeTile.EnableTile(-1, true);
            HomeTile.Unlock();
        }

        void TileUnlocked(BattleTile tile)
        {
            UnlockedTiles.Add(tile);
            OnTileSecured?.Invoke(tile);

            List<BattleTile> adjacentTiles = GetAdjacentTiles(tile);
            int unlockedCount = 0;
            foreach (BattleTile t in adjacentTiles)
            {
                if (t.gameObject.activeSelf) continue;
                t.EnableTile(unlockedCount);
                unlockedCount++;
            }
        }

        public Quest GetQuest()
        {
            // ok, so quests should be semi random
            // I want to take into consideration past quests and their types
            Quest quest;
            if (_pastQuests.Count % 3 == 0)
                quest = Instantiate(_quests[0]);
            else if (_pastQuests.Count % 3 == 1)
                quest = Instantiate(_quests[1]);
            else
                quest = Instantiate(_quests[2]);

            quest.CreateRandom(1, _pastQuests);
            _pastQuests.Add(quest);

            QuestElement qe = new(quest);

            _questContainer.Add(qe);

            return quest;
        }

        public BattleTile GetRandomUnlockedTile()
        {
            return UnlockedTiles[Random.Range(0, UnlockedTiles.Count)];
        }

        // TODO: there must be a smarter way to get adjacent tiles
        public List<BattleTile> GetAdjacentTiles(BattleTile tile)
        {
            List<BattleTile> adjacentTiles = new List<BattleTile>();
            Vector3 tilePos = tile.transform.position;
            foreach (BattleTile t in _tiles)
            {
                if (t.transform.position == tilePos) continue;

                if (t.transform.position.x == tilePos.x)
                {
                    if (t.transform.position.z == tilePos.z + HomeTile.Scale ||
                        t.transform.position.z == tilePos.z - HomeTile.Scale)
                    {
                        adjacentTiles.Add(t);
                    }
                }
                else if (t.transform.position.z == tilePos.z)
                {
                    if (t.transform.position.x == tilePos.x + HomeTile.Scale ||
                        t.transform.position.x == tilePos.x - HomeTile.Scale)
                    {
                        adjacentTiles.Add(t);
                    }
                }
            }

            return adjacentTiles;
        }

        public BattleTile ReplaceTile(BattleTile tile, Building newBuilding)
        {
            BattleTile newBattleTile = InstantiateTile(tile.transform.position, newBuilding);

            _tiles.Insert(_tiles.IndexOf(tile), newBattleTile);
            _tiles.Remove(tile);
            Destroy(tile.gameObject);

            return newBattleTile;
        }

        BattleTile InstantiateTile(Vector3 pos, Building b)
        {
            GameObject tile = Instantiate(_tilePrefab, _floorHolder);
            tile.transform.position = pos;
            tile.SetActive(false);

            Building buildingInstance = Instantiate(b);
            BattleTile bt = tile.GetComponent<BattleTile>();
            bt.Initialize(buildingInstance);

            return bt;
        }

        public BattleTile GetTileFromPosition(Vector3 pos)
        {
            foreach (BattleTile tile in _tiles)
            {
                // tile pos is a middle of tile
                // so we need to check if pos is inside tile
                if (pos.x > tile.transform.position.x - tile.Scale * 0.5f &&
                    pos.x < tile.transform.position.x + tile.Scale * 0.5f &&
                    pos.z > tile.transform.position.z - tile.Scale * 0.5f &&
                    pos.z < tile.transform.position.z + tile.Scale * 0.5f)
                {
                    return tile;
                }
            }

            return null;
        }

        public Vector3 GetRandomPositionWithinRangeOnActiveTile(Vector3 center, float range)
        {
            int tries = 0;
            while (tries < 100)
            {
                tries++;
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                randomPoint.y = 0;
                if (IsPositionOnActiveTile(randomPoint))
                    return randomPoint;
            }

            Debug.LogError($"Could not find random position within range {range} of {center} on active tile");
            return Vector3.zero;
        }

        public Vector3 GetRandomPositionInRangeOnActiveTile(Vector3 center, float range)
        {
            int tries = 0;
            while (tries < 100)
            {
                tries++;
                range -= 0.1f;
                Vector2 r = Random.insideUnitCircle * range;
                Vector3 randomPoint = center + new Vector3(r.x, 0, r.y);
                randomPoint.y = 0;
                if (IsPositionOnActiveTile(randomPoint))
                    return randomPoint;
            }

            Debug.LogError($"Could not find random position within range {range} on active tile");
            return Vector3.zero;
        }

        bool IsPositionOnActiveTile(Vector3 pos)
        {
            foreach (BattleTile tile in _tiles)
            {
                if (!UnlockedTiles.Contains(tile)) continue;
                if (pos.x > tile.transform.position.x - tile.Scale * 0.5f &&
                    pos.x < tile.transform.position.x + tile.Scale * 0.5f &&
                    pos.z > tile.transform.position.z - tile.Scale * 0.5f &&
                    pos.z < tile.transform.position.z + tile.Scale * 0.5f)
                {
                    return true;
                }
            }

            return false;
        }

        /* ASTAR */
        struct AStarTile
        {
            public AStarTile(BattleTile tile, float g, float h,
                BattleTile parent = null)
            {
                Tile = tile;

                G = g;
                H = h;
                F = G + H;

                Parent = parent;
            }

            public readonly BattleTile Tile;

            public readonly float F; //F = G + H
            public readonly float G; // G is the distance between the current node and the start node.
            public readonly float H; //H is the heuristic — estimated distance from the current node to the end node.

            public readonly BattleTile Parent;
        }

        //https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
        public List<BattleTile> GetTilePathFromTo(BattleTile startTile, BattleTile endTile)
        {
            List<AStarTile> openList = new();
            List<AStarTile> closedList = new();
            openList.Add(new AStarTile(startTile, 0, 0));

            while (openList.Count > 0)
            {
                AStarTile currentAStarTile = openList[0];
                foreach (AStarTile tile in openList)
                {
                    if (tile.F < currentAStarTile.F)
                        currentAStarTile = tile;
                }

                openList.Remove(currentAStarTile);
                closedList.Add(currentAStarTile);

                if (currentAStarTile.Tile == endTile)
                {
                    List<BattleTile> path = new();
                    path.Add(currentAStarTile.Tile);
                    while (currentAStarTile.Tile != startTile)
                    {
                        foreach (AStarTile tile in closedList)
                        {
                            if (tile.Tile == currentAStarTile.Parent)
                            {
                                path.Add(tile.Tile);
                                currentAStarTile = tile;
                                break;
                            }
                        }
                    }

                    path.Reverse();
                    return path;
                }

                List<BattleTile> adjacentTiles = GetAdjacentTiles(currentAStarTile.Tile);
                foreach (BattleTile tile in adjacentTiles)
                {
                    if (!UnlockedTiles.Contains(tile)) continue;
                    if (closedList.Exists(t => t.Tile == tile)) continue;

                    float g = currentAStarTile.G + 1;
                    float h = Vector3.Distance(tile.transform.position, endTile.transform.position);

                    AStarTile child = new(tile, g, h, currentAStarTile.Tile);

                    if (openList.Exists(t => t.Tile == tile))
                    {
                        AStarTile openTile = openList.Find(t => t.Tile == tile);
                        if (g > openTile.G) continue;
                    }

                    openList.Add(child);
                }
            }

            Debug.LogError($"Did not find the path from {startTile} to {endTile}");
            return new();
        }
    }
}