using System;
using System.Collections.Generic;
using Lis.Battle.Quest;
using Lis.Core;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle.Land
{
    public class AreaManager : MonoBehaviour
    {
        GameObject _floor;

        [SerializeField] Transform _floorHolder;
        [SerializeField] GameObject _floorPrefab;

        [SerializeField] GameObject _tilePrefab;
        [SerializeField] UpgradeBuilding _homeUpgrade; //HERE: home upgrade
        List<UpgradeBuilding> _unlockedBuildings = new();

        [SerializeField] Quest.Quest[] _quests;
        readonly List<Quest.Quest> _pastQuests = new();

        [HideInInspector] public TileController HomeTileController;
        readonly List<TileController> _tiles = new();
        [HideInInspector] public List<TileController> UnlockedTiles = new();

        VisualElement _questContainer;
        VisualElement _questElementContainer;

        public event Action<TileController> OnTileUnlocked;

        public void Initialize()
        {
            float tileScale = _floorPrefab.transform.localScale.x;
            _floor = Instantiate(_floorPrefab,
                new(-tileScale * 0.5f, 0, -tileScale * 0.5f), // floor offset to make tiles centered
                Quaternion.identity);
            _floor.transform.SetParent(_floorHolder);

            _unlockedBuildings = GameManager.Instance.UpgradeBoard.GetUnlockedBuildings();

            InitializeUI();
            CreateArea();
        }

        void InitializeUI()
        {
            _questContainer = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("quests");
            _questContainer.style.visibility = Visibility.Hidden;
            _questElementContainer =
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
                    UpgradeBuilding upgrade = _unlockedBuildings[Random.Range(0, _unlockedBuildings.Count)];
                    if (pos == Vector3.zero)
                        upgrade = _homeUpgrade;

                    TileController bt = InstantiateTile(pos, upgrade);
                    bt.OnUnlocked += TileUnlocked;
                    _tiles.Add(bt);

                    if (pos == Vector3.zero)
                        HomeTileController = bt;
                }
            }
        }

        public void SecureHomeTile()
        {
            HomeTileController.EnableTile(-1, true);
            HomeTileController.Unlock();
        }

        void TileUnlocked(TileController tileController)
        {
            UnlockedTiles.Add(tileController);
            OnTileUnlocked?.Invoke(tileController);

            List<TileController> adjacentTiles = GetAdjacentTiles(tileController);
            int unlockedCount = 0;
            foreach (TileController t in adjacentTiles)
            {
                if (t.gameObject.activeSelf) continue;
                t.EnableTile(unlockedCount);
                unlockedCount++;
            }
        }

        public Quest.Quest GetQuest()
        {
            // ok, so quests should be semi random
            // I want to take into consideration past quests and their types
            Quest.Quest quest;
            if (_pastQuests.Count % 3 == 0)
                quest = Instantiate(_quests[0]);
            else if (_pastQuests.Count % 3 == 1)
                quest = Instantiate(_quests[1]);
            else
                quest = Instantiate(_quests[2]);

            quest.CreateRandom(1, _pastQuests);
            _pastQuests.Add(quest);

            QuestElement qe = new(quest);
            _questElementContainer.Add(qe);
            _questContainer.style.visibility = Visibility.Visible;

            return quest;
        }

        public TileController GetRandomUnlockedTile()
        {
            return UnlockedTiles[Random.Range(0, UnlockedTiles.Count)];
        }

        // TODO: there must be a smarter way to get adjacent tiles
        public List<TileController> GetAdjacentTiles(TileController tileController)
        {
            List<TileController> adjacentTiles = new List<TileController>();
            Vector3 tilePos = tileController.transform.position;
            foreach (TileController t in _tiles)
            {
                if (t.transform.position == tilePos) continue;

                if (t.transform.position.x == tilePos.x)
                {
                    if (t.transform.position.z == tilePos.z + HomeTileController.Scale ||
                        t.transform.position.z == tilePos.z - HomeTileController.Scale)
                    {
                        adjacentTiles.Add(t);
                    }
                }
                else if (t.transform.position.z == tilePos.z)
                {
                    if (t.transform.position.x == tilePos.x + HomeTileController.Scale ||
                        t.transform.position.x == tilePos.x - HomeTileController.Scale)
                    {
                        adjacentTiles.Add(t);
                    }
                }
            }

            return adjacentTiles;
        }

        public TileController ReplaceTile(TileController tileController, UpgradeBuilding newBuilding)
        {
            TileController newTileController = InstantiateTile(tileController.transform.position, newBuilding);

            _tiles.Insert(_tiles.IndexOf(tileController), newTileController);
            _tiles.Remove(tileController);
            Destroy(tileController.gameObject);

            return newTileController;
        }

        TileController InstantiateTile(Vector3 pos, UpgradeBuilding b)
        {
            GameObject tile = Instantiate(_tilePrefab, _floorHolder);
            tile.transform.position = pos;
            tile.SetActive(false);

            TileController bt = tile.GetComponent<TileController>();
            bt.Initialize(b);

            return bt;
        }

        public TileController GetTileFromPosition(Vector3 pos)
        {
            foreach (TileController tile in _tiles)
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
            foreach (TileController tile in _tiles)
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
            public AStarTile(TileController tileController, float g, float h,
                TileController parent = null)
            {
                TileController = tileController;

                G = g;
                H = h;
                F = G + H;

                Parent = parent;
            }

            public readonly TileController TileController;

            public readonly float F; //F = G + H
            public readonly float G; // G is the distance between the current node and the start node.
            public readonly float H; //H is the heuristic — estimated distance from the current node to the end node.

            public readonly TileController Parent;
        }

        //https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
        public List<TileController> GetTilePathFromTo(TileController startTileController,
            TileController endTileController)
        {
            Debug.Log(
                $"Finding path from {startTileController.transform.position} to {endTileController.transform.position}");
            List<AStarTile> openList = new();
            List<AStarTile> closedList = new();
            openList.Add(new AStarTile(startTileController, 0, 0));

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

                if (currentAStarTile.TileController == endTileController)
                {
                    List<TileController> path = new();
                    path.Add(currentAStarTile.TileController);
                    while (currentAStarTile.TileController != startTileController)
                    {
                        foreach (AStarTile tile in closedList)
                        {
                            if (tile.TileController == currentAStarTile.Parent)
                            {
                                path.Add(tile.TileController);
                                currentAStarTile = tile;
                                break;
                            }
                        }
                    }

                    if (!path.Contains(endTileController)) path.Add(endTileController);
                    path.Reverse();
                    return path;
                }

                List<TileController> adjacentTiles = GetAdjacentTiles(currentAStarTile.TileController);
                foreach (TileController tile in adjacentTiles)
                {
                    if (!UnlockedTiles.Contains(tile)) continue;
                    if (closedList.Exists(t => t.TileController == tile)) continue;

                    float g = currentAStarTile.G + 1;
                    float h = Vector3.Distance(tile.transform.position, endTileController.transform.position);

                    AStarTile child = new(tile, g, h, currentAStarTile.TileController);

                    if (openList.Exists(t => t.TileController == tile))
                    {
                        AStarTile openTile = openList.Find(t => t.TileController == tile);
                        if (g > openTile.G) continue;
                    }

                    openList.Add(child);
                }
            }

            Debug.LogError($"Did not find the path from {startTileController} to {endTileController}");
            return new();
        }
    }
}