using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class BattleTile : MonoBehaviour
{
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    protected BattleFightManager _battleWaveManager;

    [Header("Tile")]
    ObjectShaders _objectShaders;

    [SerializeField] Material[] _materials;
    [SerializeField] GameObject _surface;
    [SerializeField] GameObject _borderPrefab;
    [SerializeField] GameObject _landPurchaseSignPrefab;
    [SerializeField] GameObject _rewardChestPrefab;
    public GameObject TileIndicationPrefab;


    public List<BattleTilePurchaseSign> _signs = new();
    public List<GameObject> _borders = new();

    public float Scale { get; private set; }

    MinionSpawningPattern _minionSpawningPattern;
    bool _minionPositionExecuteOnce;
    List<Vector3> _minionPositions = new();

    public event Action<BattleTile> OnEnabled;
    public void Initialize(float scale)
    {
        _objectShaders = GetComponent<ObjectShaders>();
        Scale = scale;
        MeshRenderer mr = _surface.GetComponent<MeshRenderer>();
        _surface.transform.localScale = new Vector3(scale, 0.1f, scale);
        mr.material = _materials[Random.Range(0, _materials.Length)];
    }

    public virtual void EnableTile()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleWaveManager = _battleManager.GetComponent<BattleFightManager>();

        gameObject.SetActive(true);
        StartCoroutine(EnableTileCoroutine());
    }

    IEnumerator EnableTileCoroutine()
    {
        _objectShaders.Dissolve(5f, true);
        HandleBorders(new Color(1f, 0.22f, 0f, 0.2f));  // magic color

        yield return new WaitForSeconds(3f);
        StartTileFight();
        OnEnabled?.Invoke(this);
    }

    public virtual void StartTileFight()
    {
        _minionPositionExecuteOnce = false;
        _minionPositions = new();

        _minionSpawningPattern = (MinionSpawningPattern)Random.Range(0,
                        Enum.GetNames(typeof(MinionSpawningPattern)).Length);
        if (_battleWaveManager.CurrentDifficulty == 1)
            _minionSpawningPattern = MinionSpawningPattern.SurroundMiddle;

        _battleWaveManager.InitializeFight(this);
    }



    public virtual void Secured()
    {
        // battle wave manager calls this when the fight is finished
        SpawnReward();
        ShowSigns();
    }

    void SpawnReward()
    {
        // TODO: make sure that position is empty
        Vector3 chestPosition = GetPositionOne(0, 0) + Vector3.up * 3.5f;
        GameObject chest = Instantiate(_rewardChestPrefab, chestPosition, Quaternion.identity);
        chest.transform.localScale = Vector3.zero;
        chest.transform.DOScale(2f, 1f);
        chest.transform.SetParent(transform);
    }

    public void ShowSigns()
    {
        List<BattleTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleTile tile in adjacentTiles)
        {
            if (tile.gameObject.activeSelf) continue;

            Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
            Vector3 signPosition = transform.position
                                + directionToTile
                                * Scale * 0.4f;

            BattleTilePurchaseSign sign = Instantiate(_landPurchaseSignPrefab,
                    signPosition, Quaternion.identity).GetComponent<BattleTilePurchaseSign>();
            sign.transform.SetParent(transform);
            sign.Initialize(tile);

            _signs.Add(sign);
            sign.OnPurchased += OnTilePurchased;
        }
    }

    void OnTilePurchased()
    {
        foreach (BattleTilePurchaseSign sign in _signs)
        {
            sign.DestroySelf();
            sign.OnPurchased -= OnTilePurchased;
        }
    }

    /* BORDERS */

    public void HandleBorders(Color color)
    {
        List<BattleTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleTile tile in adjacentTiles)
        {
            if (!tile.gameObject.activeSelf) continue;
            tile.UpdateTileBorders(color);
        }

        UpdateTileBorders(color);
    }

    public void UpdateTileBorders(Color color)
    {
        foreach (GameObject b in _borders)
            Destroy(b);

        List<BattleTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleTile tile in adjacentTiles)
        {
            if (tile.gameObject.activeSelf) continue;

            Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
            Vector3 borderPosition = Scale * 0.5f * directionToTile;

            InstantiateBorder(borderPosition, color);
        }

        if (adjacentTiles.Count < 4)
            UpdateGameBorders(adjacentTiles);
    }

    void UpdateGameBorders(List<BattleTile> adjacentTiles)
    {
        // check which tile is missing and instantiate border there
        List<Vector3> directions = new() { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
        foreach (BattleTile tile in adjacentTiles)
        {
            Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
            directions.Remove(directionToTile);
        }

        foreach (Vector3 dir in directions)
        {
            Vector3 borderPosition = Scale * 0.5f * dir;
            InstantiateBorder(borderPosition, Color.magenta, true);
        }
    }

    void InstantiateBorder(Vector3 borderPosition, Color color, bool isGameBorder = false)
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

        border.GetComponent<BattleTileBorder>().EnableBorder(color, isGameBorder);
        _borders.Add(border);
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

    Vector3 GetPositionRandom(int currentIndex, int totalCount)
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
        float theta = currentIndex * 2 * Mathf.PI / totalCount;
        float radius = Scale * 0.5f - 1;
        Vector3 center = transform.position;
        float x = Mathf.Cos(theta) * radius + center.x;
        float y = 1f;
        float z = Mathf.Sin(theta) * radius + center.z;

        return new(x, y, z);
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
