using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BattleLandTile : MonoBehaviour
{

    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    BattleFightManager _battleWaveManager;
    BattleTooltipManager _battleTooltipManager;

    [SerializeField] Material[] _materials;
    public Material ChosenMaterial { get; private set; } // HERE: testing
    [SerializeField] GameObject _surface;
    [SerializeField] GameObject _borderPrefab;
    [SerializeField] GameObject _landPurchaseSignPrefab;
    [SerializeField] GameObject _rewardChestPrefab;

    public List<BattleLandPurchaseSign> _signs = new();
    public List<GameObject> _borders = new();

    public float Scale { get; private set; }

    public enum MinionSpawningPattern { SurroundMiddle, Random, FewGroups, OneGroup, Grid }

    MinionSpawningPattern _minionSpawningPattern;
    bool _minionPositionExecuteOnce;
    List<Vector3> _minionPositions = new();

    public event Action<BattleLandTile> OnEnabled;
    public void Initialize(float scale)
    {
        Scale = scale;
        MeshRenderer mr = _surface.GetComponent<MeshRenderer>();
        _surface.transform.localScale = new Vector3(scale, 0.1f, scale);
        ChosenMaterial = _materials[Random.Range(0, _materials.Length)];
        mr.material = ChosenMaterial;
    }

    public void EnableTile()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleWaveManager = _battleManager.GetComponent<BattleFightManager>();
        _battleTooltipManager = BattleTooltipManager.Instance;

        gameObject.SetActive(true);
        HandleBorders();
        StartTileFight();
        OnEnabled?.Invoke(this);
    }

    public void StartTileFight()
    {
        _minionPositionExecuteOnce = false;
        _minionPositions = new();

        _minionSpawningPattern = (MinionSpawningPattern)Random.Range(0,
                        Enum.GetNames(typeof(MinionSpawningPattern)).Length);
        if (_battleWaveManager.CurrentDifficulty == 1)
            _minionSpawningPattern = MinionSpawningPattern.SurroundMiddle;

        EnableAllBorders();
        _battleWaveManager.InitializeFight(this);
    }

    public Vector3 GetMinionPosition(int currentIndex, int numberOfMinions)
    {
        if (_minionSpawningPattern == MinionSpawningPattern.SurroundMiddle)
            return GetMinionPositionSurroundMiddle(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.Random)
            return GetMinionPositionRandom(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.FewGroups)
            return GetMinionPositionFewGroups(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.OneGroup)
            return GetMinionPositionOneGroup(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.Grid)
            return GetMinionPositionGrid(currentIndex, numberOfMinions);

        return Vector3.zero;
    }

    Vector3 GetMinionPositionFewGroups(int currentIndex, int numberOfMinions)
    {
        int numberOfGroups = numberOfMinions / 5;

        if (!_minionPositionExecuteOnce)
        {
            _minionPositions = new();
            for (int i = 0; i < numberOfGroups; i++)
                _minionPositions.Add(GetMinionPositionRandom(0, 0));

            _minionPositionExecuteOnce = true;
        }

        int minionsPerGroup = numberOfMinions / numberOfGroups;
        int groupIndex = currentIndex / minionsPerGroup;
        if (groupIndex >= numberOfGroups) groupIndex = numberOfGroups - 1;

        return _minionPositions[groupIndex] + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
    }


    Vector3 GetMinionPositionOneGroup(int currentIndex, int numberOfMinions)
    {
        return transform.position + new Vector3(Random.Range(-2, 2), 1, Random.Range(-2, 2));
    }

    Vector3 GetMinionPositionGrid(int currentIndex, int numberOfMinions)
    {
        int numberOfGroups = 5;
        int minionsPerGroup = numberOfMinions / numberOfGroups;
        float rowOffset = Scale / numberOfGroups;
        float columnOffset = Scale / minionsPerGroup;

        Vector3 startPoint = transform.position
                            + Vector3.left * Scale * 0.4f
                            + Vector3.back * Scale * 0.4f;
        startPoint.y = 1f;

        int groupIndex = currentIndex / minionsPerGroup;
        if (groupIndex >= numberOfGroups) groupIndex = numberOfGroups - 1;

        return startPoint
                + Vector3.forward * rowOffset * groupIndex
                + Vector3.right * columnOffset * (currentIndex % minionsPerGroup);
    }

    Vector3 GetMinionPositionRandom(int currentIndex, int numberOfMinions)
    {
        float halfScale = Scale * 0.5f;
        return transform.position +
                new Vector3(Random.Range(-halfScale, halfScale), 1,
                                                Random.Range(-halfScale, halfScale));
    }

    Vector3 GetMinionPositionSurroundMiddle(int currentIndex, int numberOfMinions)
    {
        float theta = currentIndex * 2 * Mathf.PI / numberOfMinions;
        float radius = Scale * 0.5f - 1;
        Vector3 center = transform.position;
        float x = Mathf.Cos(theta) * radius + center.x;
        float y = 1f;
        float z = Mathf.Sin(theta) * radius + center.z;

        return new(x, y, z);
    }

    public void Secured()
    {
        // battle wave manager calls this when the fight is finished

        HandleBorders();
        SpawnReward();
        ShowSigns();
    }

    void SpawnReward()
    {
        // TODO: make sure that position is empty
        Vector3 chestPosition = transform.position + Vector3.up * 3.5f;
        GameObject chest = Instantiate(_rewardChestPrefab, chestPosition, Quaternion.identity);
        chest.transform.localScale = Vector3.zero;
        chest.transform.DOScale(2f, 1f);
        chest.transform.SetParent(transform);
    }

    public void ShowSigns()
    {
        List<BattleLandTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleLandTile tile in adjacentTiles)
        {
            if (tile.gameObject.activeSelf) continue;

            Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
            Vector3 signPosition = transform.position
                                + directionToTile
                                * Scale * 0.4f;

            BattleLandPurchaseSign sign = Instantiate(_landPurchaseSignPrefab,
                    signPosition, Quaternion.identity).GetComponent<BattleLandPurchaseSign>();
            sign.transform.SetParent(transform);
            sign.Initialize(tile);

            _signs.Add(sign);
            sign.OnPurchased += OnTilePurchased;
        }
    }

    void OnTilePurchased()
    {
        foreach (BattleLandPurchaseSign sign in _signs)
        {
            sign.gameObject.SetActive(false);
            sign.OnPurchased -= OnTilePurchased;
        }

        HandleBorders();
    }

    /* BORDERS */
    void EnableAllBorders()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 directionToTile = Vector3.zero;
            if (i == 0) directionToTile = Vector3.forward;
            if (i == 1) directionToTile = Vector3.right;
            if (i == 2) directionToTile = Vector3.back;
            if (i == 3) directionToTile = Vector3.left;

            Vector3 borderPosition = Scale * 0.5f * directionToTile;

            InstantiateBorder(borderPosition, new Color(1f, 0.22f, 0f, 0.2f)); // magic color
        }
    }

    void HandleBorders()
    {
        UpdateTileBorders();

        List<BattleLandTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleLandTile tile in adjacentTiles)
        {
            if (!tile.gameObject.activeSelf) continue;
            tile.UpdateTileBorders();
        }
    }

    public void UpdateTileBorders()
    {
        foreach (GameObject b in _borders)
            Destroy(b);

        List<BattleLandTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleLandTile tile in adjacentTiles)
        {
            if (tile.gameObject.activeSelf) continue;

            Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
            Vector3 borderPosition = Scale * 0.5f * directionToTile;

            InstantiateBorder(borderPosition, default);
        }
    }

    void InstantiateBorder(Vector3 borderPosition, Color color)
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

        border.GetComponent<BattleLandBorder>().EnableBorder(color);
        _borders.Add(border);
    }
}
