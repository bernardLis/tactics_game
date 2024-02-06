using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleTile : MonoBehaviour, IPointerDownHandler
    {
        BattleManager _battleManager;
        BattleAreaManager _battleAreaManager;
        BattleTooltipManager _battleTooltipManager;

        [Header("Tile")] ObjectShaders _objectShaders;

        [Header("Base")] [SerializeField] Material[] _materials;

        [SerializeField] GameObject _floor;
        [SerializeField] GameObject _surface;
        [SerializeField] GameObject _borderPrefab;

        [Header("Enabled")] [SerializeField] GameObject _enabledEffect;
        [SerializeField] GameObject _canvas;
        [SerializeField] TMP_Text _questText;
        [SerializeField] Image _questImage;

        [FormerlySerializedAs("_borders")] public List<BattleTileBorder> Borders = new();

        public float Scale { get; private set; }

        public Building Building; //{ get; private set; }
        public BattleBuilding BattleBuilding { get; private set; }

        GameObject _tileIndicator;

        int _totalTime;
        int _timePassed;

        Quest _quest;

        public event Action<BattleTile> OnUnlocked;

        // at the beginning of the game
        public void Initialize(Building building)
        {
            _battleManager = BattleManager.Instance;
            _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
            _battleTooltipManager = _battleManager.GetComponent<BattleTooltipManager>();

            Building = building;

            Scale = _surface.transform.localScale.x;

            _objectShaders = GetComponent<ObjectShaders>();
            MeshRenderer mr = _surface.GetComponent<MeshRenderer>();
            mr.material = _materials[Random.Range(0, _materials.Length)];

            BattleBuilding = GetComponentInChildren<BattleBuilding>();
            if (Building == null) return;
            BattleBuilding = Instantiate(Building.BuildingPrefab, transform).GetComponent<BattleBuilding>();
            Vector3 pos = new(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            BattleBuilding.Initialize(pos, Building);
        }

        // when tile next to it is unlocked
        public void EnableTile(int questTimerIndex, bool isHomeTile = false)
        {
            gameObject.SetActive(true);

            if (isHomeTile) return;

            ShowTileIndicator();

            _floor.SetActive(false);
            _enabledEffect.SetActive(true);
            _canvas.SetActive(true);

            SetTimer(questTimerIndex);
        }

        void SetTimer(int questTimerIndex)
        {
            // HERE: balance 
            _totalTime = 60 * questTimerIndex + _battleAreaManager.UnlockedTiles.Count * Random.Range(7, 13);

            StartCoroutine(QuestTimer());
            _questText.fontSize = 48;
            _questText.text = questTimerIndex.ToString();
        }

        IEnumerator QuestTimer()
        {
            while (_timePassed < _totalTime)
            {
                _timePassed++;
                int timeLeft = _totalTime - _timePassed;
                int minutes = Mathf.FloorToInt(timeLeft / 60f);
                int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
                _questText.text = $"{minutes:00}:{seconds:00}";

                yield return new WaitForSeconds(1f);
            }

            SetQuest();
        }

        void SetQuest()
        {
            _questText.fontSize = 64;

            _quest = _battleAreaManager.GetQuest();
            _questImage.sprite = _quest.GetIcon();
            _quest.StartQuest();
            _quest.OnQuestUpdated += UpdateQuestInfo;
            _quest.OnQuestCompleted += Unlock;

            UpdateQuestInfo();
        }

        void UpdateQuestInfo()
        {
            _questText.text = $"{_quest.TotalAmount - _quest.CurrentAmount}";
        }

        void ShowTileIndicator()
        {
            if (Building == null) return;
            if (Building.TileIndicatorPrefab == null) return;

            _tileIndicator = Instantiate(Building.TileIndicatorPrefab, transform);
            _tileIndicator.transform.localPosition = Vector3.up * 6f;
            _tileIndicator.transform.localScale = Vector3.one * 2f;
        }

        // when player finishes the task 
        public void Unlock()
        {
            HideTileIndicator();
            StartCoroutine(UnlockTileCoroutine());

            if (_quest == null) return;
            _quest.OnQuestUpdated -= UpdateQuestInfo;
            _quest.OnQuestCompleted -= Unlock;
        }

        void HideTileIndicator()
        {
            _enabledEffect.SetActive(false);
            _canvas.SetActive(false);

            Debug.Log("Hide tile indicator to be implemented");
            if (_tileIndicator != null) Destroy(_tileIndicator);
        }

        IEnumerator UnlockTileCoroutine()
        {
            OnUnlocked?.Invoke(this);
            _battleTooltipManager.ShowGameInfo("Tile Unlocked!", 2f);

            _floor.SetActive(true);
            _objectShaders.Dissolve(5f, true);

            yield return new WaitForSeconds(1.5f);
            HandleBorders();
            yield return new WaitForSeconds(1.5f);
            BattleBuilding.ShowBuilding();
        }

        /* BORDERS */
        void HandleBorders()
        {
            List<BattleTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
            foreach (BattleTile tile in adjacentTiles)
            {
                if (!_battleAreaManager.UnlockedTiles.Contains(tile)) continue;
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

                if (_battleAreaManager.UnlockedTiles.Contains(tile) && battleTileBorder != null)
                {
                    battleTileBorder.DestroySelf();
                    continue;
                }

                if (!_battleAreaManager.UnlockedTiles.Contains(tile) && battleTileBorder == null)
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

        // HERE: tile unlocking debug
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_floor.activeSelf) return;

            Unlock();
        }
    }
}