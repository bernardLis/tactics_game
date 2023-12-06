using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class BattleTile : MonoBehaviour
{
    protected GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    protected BattleMinionManager _battleMinionManager;
    BattleTooltipManager _tooltipManager;

    [Header("Tile")]
    ObjectShaders _objectShaders;

    [SerializeField] Material[] _materials;
    [SerializeField] GameObject _surface;
    [SerializeField] GameObject _borderPrefab;
    [SerializeField] GameObject _rewardChestPrefab;

    public List<BattleTileBorder> _borders = new();

    public float Scale { get; private set; }

    public Building Building; //{ get; private set; }
    public BattleBuilding BattleBuilding { get; private set; }

    MinionSpawningPattern _minionSpawningPattern;
    bool _minionPositionExecuteOnce;
    List<Vector3> _minionPositions = new();

    GameObject _tileIndicator;

    bool _blockSecuring;
    bool _isSecured;
    IEnumerator _securingCoroutine;
    IntVariable _currentSecuringTimeVariable;
    IntVariable _totalSecuringTimeVariable;


    public event Action<BattleTile> OnEnabled;
    public void Initialize(Building building)
    {
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

    public virtual void EnableTile()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleMinionManager = _battleManager.GetComponent<BattleMinionManager>();

        _battleAreaManager.OnBossTileUnlocked += (a) => _blockSecuring = true;

        gameObject.SetActive(true);
        StartCoroutine(EnableTileCoroutine());
    }

    IEnumerator EnableTileCoroutine()
    {
        _objectShaders.Dissolve(5f, true);

        yield return new WaitForSeconds(1.5f);

        HandleBorders();
        yield return new WaitForSeconds(1.5f);
        ShowTileIndicator();
        OnEnabled?.Invoke(this);
    }

    void ShowTileIndicator()
    {
        if (Building == null) return;
        if (Building.TileIndicatorPrefab == null) return;

        _tileIndicator = Instantiate(Building.TileIndicatorPrefab, transform);
        _tileIndicator.transform.localPosition = Vector3.up * 6f;
        _tileIndicator.transform.localScale = Vector3.one * 2f;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;
        if (_isSecured) return;

        StartSecuring();
    }

    void OnTriggerExit(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;
        if (_isSecured) return;

        StopSecuring();
    }

    void StartSecuring()
    {
        if (_blockSecuring) return;
        _securingCoroutine = SecuringCoroutine();
        StartCoroutine(_securingCoroutine);

        _tooltipManager.ShowTileSecureBar(_currentSecuringTimeVariable, _totalSecuringTimeVariable);
    }

    IEnumerator SecuringCoroutine()
    {
        while (_currentSecuringTimeVariable.Value < _totalSecuringTimeVariable.Value)
        {
            _currentSecuringTimeVariable.ApplyChange(1);
            yield return new WaitForSeconds(1f);
        }

        Secured();
    }

    void StopSecuring()
    {
        if (_securingCoroutine != null) StopCoroutine(_securingCoroutine);
        _securingCoroutine = null;
        _tooltipManager.HideTileSecureBar();
    }

    public virtual void Secured()
    {
        _isSecured = true;

        BattleBuilding = GetComponentInChildren<BattleBuilding>();
        if (Building != null)
        {
            BattleBuilding = Instantiate(Building.BuildingPrefab, transform).GetComponent<BattleBuilding>();
            Vector3 pos = new(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            BattleBuilding.Initialize(pos, Building);
        }

        _tooltipManager.HideTileSecureBar();
        if (_tileIndicator != null) Destroy(_tileIndicator);

        SpawnReward();
    }


    void SpawnReward()
    {
        Vector3 chestPosition = GetPositionOne(0, 0);
        chestPosition.y = 0f;
        GameObject chest = Instantiate(_rewardChestPrefab, chestPosition, Quaternion.identity);
        chest.transform.localScale = Vector3.zero;
        chest.transform.DOScale(2f, 1f);
        chest.transform.SetParent(transform);
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

    public void UpdateTileBorders()
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
        foreach (BattleTileBorder b in _borders)
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
        _borders.Add(b);
    }

    /* POSITIONS ON TILE */
    public Vector3 GetMinionPosition(int currentIndex, int numberOfMinions)
    {
        if (_minionSpawningPattern == MinionSpawningPattern.SurroundMiddle)
            return GetPositionAroundMiddle(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.Random)
            return GetPositionRandom(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.FewGroups)
            return GetPositionFewGroups(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.OneGroup)
            return GetPositionOne(currentIndex, numberOfMinions);
        if (_minionSpawningPattern == MinionSpawningPattern.Grid)
            return GetPositionGrid(currentIndex, numberOfMinions);

        return Vector3.zero;
    }

    Vector3 GetPositionFewGroups(int currentIndex, int numberOfMinions)
    {
        int numberOfGroups = numberOfMinions / 5;

        if (!_minionPositionExecuteOnce)
        {
            _minionPositions = new();
            for (int i = 0; i < numberOfGroups; i++)
                _minionPositions.Add(GetPositionRandom(0, 0));

            _minionPositionExecuteOnce = true;
        }

        int minionsPerGroup = numberOfMinions / numberOfGroups;
        int groupIndex = currentIndex / minionsPerGroup;
        if (groupIndex >= numberOfGroups) groupIndex = numberOfGroups - 1;

        return _minionPositions[groupIndex] + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
    }


    Vector3 GetPositionOne(int currentIndex, int totalCount)
    {
        if (!_minionPositionExecuteOnce)
        {
            _minionPositions = new();
            _minionPositions.Add(GetPositionRandom(0, 0));
            _minionPositionExecuteOnce = true;
        }

        return _minionPositions[0] + new Vector3(Random.Range(-2, 2), 1, Random.Range(-2, 2));
    }

    Vector3 GetPositionGrid(int currentIndex, int totalCount)
    {
        int numberOfGroups = 5;
        int minionsPerGroup = totalCount / numberOfGroups;
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

    public Vector3 GetPositionRandom(int currentIndex, int totalCount)
    {
        float halfScale = Scale * 0.5f - 2;

        Vector3 point = transform.position +
                new Vector3(Random.Range(-halfScale, halfScale), 1,
                            Random.Range(-halfScale, halfScale));

        if (IsPositionOnNavMesh(point, out Vector3 result))
            return result;
        return GetPositionRandom(0, 0);
    }

    Vector3 GetPositionAroundMiddle(int currentIndex, int totalCount)
    {
        float radius = Scale * 0.5f - 1;
        Vector3 pos = Helpers.GetPositionOnCircle(transform.position, radius, currentIndex, totalCount);
        pos.y = 1f;

        return pos;
    }

    bool IsPositionOnNavMesh(Vector3 point, out Vector3 result)
    {
        NavMeshHit hit;
        //https://docs.unity3d.com/540/Documentation/ScriptReference/NavMesh.SamplePosition.html
        if (NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

}
