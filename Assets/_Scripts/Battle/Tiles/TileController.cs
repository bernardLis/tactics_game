using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Tiles.Building;
using Lis.Core;
using Lis.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Lis.Battle.Tiles
{
    public class TileController : MonoBehaviour, IPointerDownHandler
    {
        BattleManager _battleManager;
        AreaManager _areaManager;
        TooltipManager _tooltipManager;

        [Header("Tile")] ObjectShaders _objectShaders;

        [Header("Base")] [SerializeField] Material[] _materials;
        [SerializeField] GameObject _floor;
        [SerializeField] GameObject _surface;
        [SerializeField] GameObject _borderPrefab;

        [Header("Enabled")] [SerializeField] GameObject _enabledEffect;
        [SerializeField] GameObject _canvas;
        [SerializeField] TMP_Text _questText;
        [SerializeField] Image _questImage;

        [Header("Unlocked")]
        public List<TileBorderController> Borders = new();

        public float Scale { get; private set; }

        [HideInInspector] public UpgradeTile Upgrade;

        int _totalTime;
        int _timePassed;

        Quest.Quest _quest;

        public event Action<UpgradeTile> OnTileEnabled;
        public event Action<TileController> OnTileUnlocked;

        // at the beginning of the game
        public void Initialize(UpgradeTile upgrade)
        {
            Debug.Log($"Initialize {upgrade.name}");
            _battleManager = BattleManager.Instance;
            _areaManager = _battleManager.GetComponent<AreaManager>();
            _tooltipManager = _battleManager.GetComponent<TooltipManager>();

            Upgrade = upgrade;

            Scale = _surface.transform.localScale.x;

            _objectShaders = GetComponent<ObjectShaders>();
            MeshRenderer mr = _surface.GetComponent<MeshRenderer>();
            mr.material = _materials[Random.Range(0, _materials.Length)];
        }

        // when tile next to it is unlocked
        public void EnableTile(int questTimerIndex, bool isHomeTile = false)
        {
            gameObject.SetActive(true);

            if (isHomeTile) return;

            _floor.SetActive(false);
            _enabledEffect.SetActive(true);
            _canvas.SetActive(true);

            SetTimer(questTimerIndex);
            OnTileEnabled?.Invoke(Upgrade);
        }

        void SetTimer(int questTimerIndex)
        {
            // HERE: balance 
            _totalTime = 60 * questTimerIndex + _areaManager.UnlockedTiles.Count * Random.Range(7, 13);

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

            _quest = _areaManager.GetQuest();
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

        // when player finishes the task 
        public void Unlock()
        {
            StartCoroutine(UnlockTileCoroutine());

            _enabledEffect.SetActive(false);
            _canvas.SetActive(false);

            if (_quest == null) return;
            _quest.OnQuestUpdated -= UpdateQuestInfo;
            _quest.OnQuestCompleted -= Unlock;
        }

        IEnumerator UnlockTileCoroutine()
        {
            OnTileUnlocked?.Invoke(this);

            _tooltipManager.ShowGameInfo("Tile Unlocked!", 2f);

            _floor.SetActive(true);
            _objectShaders.Dissolve(5f, true);

            yield return new WaitForSeconds(1.5f);
            GetComponentInChildren<PlayerUnitTracker>().gameObject.SetActive(true);

            HandleBorders();
            yield return new WaitForSeconds(1.5f);
        }

        /* BORDERS */
        void HandleBorders()
        {
            List<TileController> adjacentTiles = _areaManager.GetAdjacentTiles(this);
            foreach (TileController tile in adjacentTiles)
            {
                if (!_areaManager.UnlockedTiles.Contains(tile)) continue;
                tile.UpdateTileBorders();
            }

            UpdateTileBorders();
        }

        void UpdateTileBorders()
        {
            List<TileController> adjacentTiles = _areaManager.GetAdjacentTiles(this);
            foreach (TileController tile in adjacentTiles)
            {
                Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
                Vector3 borderPosition = Scale * 0.5f * directionToTile + Vector3.up;
                TileBorderController tileBorderController = BorderAtPosition(borderPosition);

                if (_areaManager.UnlockedTiles.Contains(tile) && tileBorderController != null)
                {
                    tileBorderController.DestroySelf();
                    continue;
                }

                if (!_areaManager.UnlockedTiles.Contains(tile) && tileBorderController == null)
                    InstantiateBorder(borderPosition);
            }

            if (adjacentTiles.Count < 4)
                UpdateGameBorders(adjacentTiles);
        }

        TileBorderController BorderAtPosition(Vector3 position)
        {
            foreach (TileBorderController b in Borders)
                if (b != null && b.transform.localPosition == position)
                    return b;

            return null;
        }

        void UpdateGameBorders(List<TileController> adjacentTiles)
        {
            List<Vector3> directions = new() { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
            foreach (TileController tile in adjacentTiles)
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

            TileBorderController b = border.GetComponent<TileBorderController>();
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