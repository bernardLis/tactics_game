using System;
using System.Collections.Generic;
using Lis.Battle.Quest;
using Lis.Core;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle.Tiles
{
    public class AreaManager : MonoBehaviour
    {
        GameObject _floor;

        [SerializeField] Transform _floorHolder;
        [SerializeField] GameObject _floorPrefab;


        [SerializeField] UpgradeTile _homeUpgrade;
        List<UpgradeTile> _unlockedBuildings = new();

        [SerializeField] Quest.Quest[] _quests;
        readonly List<Quest.Quest> _pastQuests = new();

        [FormerlySerializedAs("HomeTileController")] [HideInInspector] public Controller HomeController;
        readonly List<Controller> _tiles = new();
        [HideInInspector] public List<Controller> UnlockedTiles = new();

        VisualElement _questContainer;
        VisualElement _questElementContainer;

        public event Action<Controller> OnTileUnlocked;

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
                    UpgradeTile upgrade = _unlockedBuildings[Random.Range(0, _unlockedBuildings.Count)];
                    if (pos == Vector3.zero)
                        upgrade = _homeUpgrade;

                    Controller bt = InstantiateTile(pos, upgrade);
                    bt.OnTileUnlocked += TileTileUnlocked;
                    _tiles.Add(bt);

                    if (pos == Vector3.zero)
                        HomeController = bt;
                }
            }
        }

        public void SecureHomeTile()
        {
            HomeController.EnableTile(-1, true);
            HomeController.Unlock();
        }

        void TileTileUnlocked(Controller controller)
        {
            UnlockedTiles.Add(controller);
            OnTileUnlocked?.Invoke(controller);

            List<Controller> adjacentTiles = GetAdjacentTiles(controller);
            int unlockedCount = 0;
            foreach (Controller t in adjacentTiles)
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

        public Controller GetRandomUnlockedTile()
        {
            return UnlockedTiles[Random.Range(0, UnlockedTiles.Count)];
        }

        // TODO: there must be a smarter way to get adjacent tiles
        public List<Controller> GetAdjacentTiles(Controller controller)
        {
            List<Controller> adjacentTiles = new List<Controller>();
            Vector3 tilePos = controller.transform.position;
            foreach (Controller t in _tiles)
            {
                if (t.transform.position == tilePos) continue;

                if (t.transform.position.x == tilePos.x)
                {
                    if (t.transform.position.z == tilePos.z + HomeController.Scale ||
                        t.transform.position.z == tilePos.z - HomeController.Scale)
                    {
                        adjacentTiles.Add(t);
                    }
                }
                else if (t.transform.position.z == tilePos.z)
                {
                    if (t.transform.position.x == tilePos.x + HomeController.Scale ||
                        t.transform.position.x == tilePos.x - HomeController.Scale)
                    {
                        adjacentTiles.Add(t);
                    }
                }
            }

            return adjacentTiles;
        }

        public Controller ReplaceTile(Controller controller, UpgradeTile newTile)
        {
            Controller newController = InstantiateTile(controller.transform.position, newTile);

            _tiles.Insert(_tiles.IndexOf(controller), newController);
            _tiles.Remove(controller);
            Destroy(controller.gameObject);

            return newController;
        }

        Controller InstantiateTile(Vector3 pos, UpgradeTile b)
        {
            GameObject tile = Instantiate(b.TilePrefab, _floorHolder);
            tile.transform.position = pos;
            tile.SetActive(false);

            Controller bt = tile.GetComponent<Controller>();
            bt.Initialize(b);

            return bt;
        }

        public Controller GetTileFromPosition(Vector3 pos)
        {
            foreach (Controller tile in _tiles)
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
            foreach (Controller tile in _tiles)
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
            public AStarTile(Controller controller, float g, float h,
                Controller parent = null)
            {
                Controller = controller;

                G = g;
                H = h;
                F = G + H;

                Parent = parent;
            }

            public readonly Controller Controller;

            public readonly float F; //F = G + H
            public readonly float G; // G is the distance between the current node and the start node.
            public readonly float H; //H is the heuristic — estimated distance from the current node to the end node.

            public readonly Controller Parent;
        }

        //https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
        public List<Controller> GetTilePathFromTo(Controller startController,
            Controller endController)
        {
            Debug.Log(
                $"Finding path from {startController.transform.position} to {endController.transform.position}");
            List<AStarTile> openList = new();
            List<AStarTile> closedList = new();
            openList.Add(new AStarTile(startController, 0, 0));

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

                if (currentAStarTile.Controller == endController)
                {
                    List<Controller> path = new();
                    path.Add(currentAStarTile.Controller);
                    while (currentAStarTile.Controller != startController)
                    {
                        foreach (AStarTile tile in closedList)
                        {
                            if (tile.Controller == currentAStarTile.Parent)
                            {
                                path.Add(tile.Controller);
                                currentAStarTile = tile;
                                break;
                            }
                        }
                    }

                    if (!path.Contains(endController)) path.Add(endController);
                    path.Reverse();
                    return path;
                }

                List<Controller> adjacentTiles = GetAdjacentTiles(currentAStarTile.Controller);
                foreach (Controller tile in adjacentTiles)
                {
                    if (!UnlockedTiles.Contains(tile)) continue;
                    if (closedList.Exists(t => t.Controller == tile)) continue;

                    float g = currentAStarTile.G + 1;
                    float h = Vector3.Distance(tile.transform.position, endController.transform.position);

                    AStarTile child = new(tile, g, h, currentAStarTile.Controller);

                    if (openList.Exists(t => t.Controller == tile))
                    {
                        AStarTile openTile = openList.Find(t => t.Controller == tile);
                        if (g > openTile.G) continue;
                    }

                    openList.Add(child);
                }
            }

            Debug.LogError($"Did not find the path from {startController} to {endController}");
            return new();
        }
    }
}