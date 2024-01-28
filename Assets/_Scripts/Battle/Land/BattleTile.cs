using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleTile : MonoBehaviour
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;
        BattleTooltipManager _tooltipManager;

        [Header("Tile")] ObjectShaders _objectShaders;

        [SerializeField] Material[] _materials;
        [SerializeField] GameObject _floor;
        [SerializeField] GameObject _surface;
        [SerializeField] GameObject _borderPrefab;
        [SerializeField] GameObject _securedEffect;

        [FormerlySerializedAs("_borders")] public List<BattleTileBorder> Borders = new();

        public float Scale { get; private set; }

        public Building Building; //{ get; private set; }
        public BattleBuilding BattleBuilding { get; private set; }

        GameObject _tileIndicator;

        IEnumerator _securingCoroutine;
        IntVariable _currentSecuringTimeVariable;
        IntVariable _totalSecuringTimeVariable;

        public event Action<BattleTile> OnEnabled;

        // at the beginning of the game
        public void Initialize(Building building)
        {
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

            _tooltipManager = BattleTooltipManager.Instance;

            _currentSecuringTimeVariable = ScriptableObject.CreateInstance<IntVariable>();
            _currentSecuringTimeVariable.SetValue(0);
            _totalSecuringTimeVariable = ScriptableObject.CreateInstance<IntVariable>();
            _totalSecuringTimeVariable.SetValue(10);

            Building = building;

            Scale = _surface.transform.localScale.x;

            _objectShaders = GetComponent<ObjectShaders>();
            MeshRenderer mr = _surface.GetComponent<MeshRenderer>();
            mr.material = _materials[Random.Range(0, _materials.Length)];
        }

        // when tile next to it is secured
        public void EnableTile()
        {
            _floor.SetActive(false);
            gameObject.SetActive(true);
            ShowTileIndicator(); }

        void ShowTileIndicator()
        {
            if (Building == null) return;
            if (Building.TileIndicatorPrefab == null) return;

            _tileIndicator = Instantiate(Building.TileIndicatorPrefab, transform);
            _tileIndicator.transform.localPosition = Vector3.up * 6f;
            _tileIndicator.transform.localScale = Vector3.one * 2f;
            
            
        }

        // when player finishes the task 
        public void Secure()
        {
            _tooltipManager.HideTileSecureBar();
            if (_tileIndicator != null) Destroy(_tileIndicator);

            StartCoroutine(SecureTileCoroutine());
        }

        IEnumerator SecureTileCoroutine()
        {
            _securedEffect.SetActive(true);
            _objectShaders.Dissolve(5f, true);

            yield return new WaitForSeconds(1.5f);
            HandleBorders();
            yield return new WaitForSeconds(1.5f);

            BattleBuilding = GetComponentInChildren<BattleBuilding>();
            if (Building != null)
            {
                BattleBuilding = Instantiate(Building.BuildingPrefab, transform).GetComponent<BattleBuilding>();
                Vector3 pos = new(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                BattleBuilding.Initialize(pos, Building);
            }

            OnEnabled?.Invoke(this);
        }

        /* BORDERS */
        public void HandleBorders()
        {
            List<BattleTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
            foreach (BattleTile tile in adjacentTiles)
            {
                if (!tile.gameObject.activeSelf) continue;
                tile.UpdateTileBorders();
            }

            UpdateTileBorders();
        }

        void UpdateTileBorders()
        {
            List<BattleTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
            foreach (BattleTile tile in adjacentTiles)
            {
                Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
                Vector3 borderPosition = Scale * 0.5f * directionToTile + Vector3.up;
                BattleTileBorder battleTileBorder = BorderAtPosition(borderPosition);

                if (tile.gameObject.activeSelf && battleTileBorder != null)
                {
                    battleTileBorder.DestroySelf();
                    continue;
                }

                if (!tile.gameObject.activeSelf && battleTileBorder == null)
                    InstantiateBorder(borderPosition);
            }

            if (adjacentTiles.Count < 4)
                UpdateGameBorders(adjacentTiles);
        }

        BattleTileBorder BorderAtPosition(Vector3 position)
        {
            foreach (BattleTileBorder b in Borders)
                if (b != null && b.transform.localPosition == position)
                    return b;

            return null;
        }

        void UpdateGameBorders(List<BattleTile> adjacentTiles)
        {
            List<Vector3> directions = new() { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
            foreach (BattleTile tile in adjacentTiles)
            {
                Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
                directions.Remove(directionToTile);
            }

            foreach (Vector3 dir in directions)
            {
                Vector3 borderPosition = Scale * 0.5f * dir;
                InstantiateBorder(borderPosition);
            }
        }

        void InstantiateBorder(Vector3 borderPosition)
        {
            // for the effect to stack nicely
            Vector3 borderRotation = Vector3.zero;
            if (borderPosition.z > 0) borderRotation = new(0f, 90f, 0f);
            if (borderPosition.x > 0) borderRotation = new(0f, 180f, 0f);
            if (borderPosition.z < 0) borderRotation = new(0f, 270f, 0f);

            GameObject border = Instantiate(_borderPrefab, transform);
            border.transform.localPosition = borderPosition;
            border.transform.localEulerAngles = borderRotation;
            Vector3 borderScale = new(0.05f, 2f, Scale);
            border.transform.localScale = borderScale;

            BattleTileBorder b = border.GetComponent<BattleTileBorder>();
            b.EnableBorder();
            Borders.Add(b);
        }

        public Vector3 GetRandomPositionOnTile()
        {
            float halfScale = Scale * 0.5f - 2;

            Vector3 point = transform.position +
                            new Vector3(Random.Range(-halfScale, halfScale), 1,
                                Random.Range(-halfScale, halfScale));

            if (IsPositionOnNavMesh(point, out Vector3 result))
                return result;
            return GetRandomPositionOnTile();
        }


        static bool IsPositionOnNavMesh(Vector3 point, out Vector3 result)
        {
            //https://docs.unity3d.com/540/Documentation/ScriptReference/NavMesh.SamplePosition.html
            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }
    }
}