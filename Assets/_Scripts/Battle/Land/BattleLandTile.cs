using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;
public class BattleLandTile : MonoBehaviour
{

    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    BattleFightManager _battleWaveManager;

    [SerializeField] Material[] _materials;
    [SerializeField] GameObject _landPurchaseSignPrefab;
    [SerializeField] GameObject _borderPrefab;
    [SerializeField] GameObject _surface;

    public List<BattleLandPurchaseSign> _signs = new();
    public List<GameObject> _borders = new();

    public float Scale { get; private set; }

    public event Action<BattleLandTile> OnEnabled;

    void Start()
    {
    }

    public void Initialize(float scale)
    {
        Scale = scale;
        MeshRenderer mr = _surface.GetComponent<MeshRenderer>();
        _surface.transform.localScale = new Vector3(scale, 0.1f, scale);
        mr.material = _materials[Random.Range(0, _materials.Length)];
    }

    public void EnableTile()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleWaveManager = _battleManager.GetComponent<BattleFightManager>();

        gameObject.SetActive(true);
        HandleBorders();
        StartTileFight();
        OnEnabled?.Invoke(this);
    }

    public void StartTileFight()
    {
        // TODO: make sure player is on the tile
        EnableAllBorders();

        // start spawning waves via wave manager
        _battleWaveManager.InitializeWaves(this);

        // on waves end:
        // TileSecured();
    }

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

            InstantiateBorder(borderPosition, new Color(1f, 0.22f, 0f, 0.2f));// magic color
        }
    }

    public void Secured()
    {
        HandleBorders();
        ShowSigns();
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
                                * transform.localScale.x * 0.4f;

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
        Vector3 borderRotation = Vector3.zero;
        if (borderPosition.z > 0) borderRotation = new Vector3(0f, 90f, 0f);
        if (borderPosition.x > 0) borderRotation = new Vector3(0f, 180f, 0f); // for the effect to stack nicely...
        if (borderPosition.z < 0) borderRotation = new Vector3(0f, 270f, 0f); // for the effect to stack nicely...

        GameObject border = Instantiate(_borderPrefab, transform);
        border.transform.localPosition = borderPosition;
        border.transform.localEulerAngles = borderRotation;
        Vector3 borderScale = new Vector3(0.05f, 2f, Scale);
        border.transform.localScale = borderScale;

        border.GetComponent<BattleLandBorder>().EnableBorder(color);
        _borders.Add(border);
    }
}
