using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BattleLandTile : MonoBehaviour
{
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    BattleWaveManager _battleWaveManager;
    [SerializeField] GameObject _landPurchaseSignPrefab;
    [SerializeField] GameObject _borderPrefab;

    public List<BattleLandPurchaseSign> _signs = new();
    public List<GameObject> _borders = new();

    public event Action<BattleLandTile> OnEnabled;

    public void EnableTile()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleWaveManager = _battleManager.GetComponent<BattleWaveManager>();

        gameObject.SetActive(true);
        //    HandleBorders();
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
            switch (i)
            {
                case 0:
                    directionToTile = Vector3.forward;
                    break;
                case 1:
                    directionToTile = Vector3.right;
                    break;
                case 2:
                    directionToTile = Vector3.back;
                    break;
                case 3:
                    directionToTile = Vector3.left;
                    break;
            }

            Vector3 borderPosition = transform.position
                    + directionToTile
                    * transform.localScale.x * 10 * 0.5f; // magic 10
            Vector3 borderRotation = Vector3.zero;
            Vector3 borderScale = new Vector3(0.2f, 1f, 10f); // magic 10
            if (directionToTile.z > 0) borderRotation = new Vector3(0f, 90f, 0f);
            if (directionToTile.x > 0) borderRotation = new Vector3(0f, 180f, 0f); // for the effect to stack nicely...
            if (directionToTile.z < 0) borderRotation = new Vector3(0f, 270f, 0f); // for the effect to stack nicely...

            GameObject border = Instantiate(_borderPrefab,
                                    borderPosition,
                                    Quaternion.Euler(borderRotation));
            border.GetComponent<BattleLandBorder>().EnableBorder(new Color(1f, 0.22f, 0f, 0.2f)); // magic color
            border.transform.SetParent(transform);
            border.transform.localScale = borderScale;
            _borders.Add(border);

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
                                * transform.localScale.x * 10 * 0.4f; // magic 10

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
            Vector3 borderPosition = transform.position
                                + directionToTile
                                * transform.localScale.x * 10 * 0.5f; // magic 10
            Vector3 borderRotation = Vector3.zero;
            Vector3 borderScale = new Vector3(0.2f, 1f, 10f); // magic 10
            if (directionToTile.z > 0) borderRotation = new Vector3(0f, 90f, 0f);
            if (directionToTile.x > 0) borderRotation = new Vector3(0f, 180f, 0f); // for the effect to stack nicely...
            if (directionToTile.z < 0) borderRotation = new Vector3(0f, 270f, 0f); // for the effect to stack nicely...

            GameObject border = Instantiate(_borderPrefab,
                                    borderPosition,
                                    Quaternion.Euler(borderRotation));
            border.GetComponent<BattleLandBorder>().EnableBorder(default);
            border.transform.SetParent(transform);
            border.transform.localScale = borderScale;
            _borders.Add(border);
        }
    }
}
