using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BattleLandTile : MonoBehaviour
{
    BattleAreaManager _battleAreaManager;
    [SerializeField] GameObject _landPurchaseSignPrefab;
    [SerializeField] GameObject _borderPrefab;

    public List<BattleLandPurchaseSign> _signs = new();
    public List<GameObject> _borders = new();

    public event Action<BattleLandTile> OnEnabled;

    public void EnableTile()
    {
        _battleAreaManager = BattleManager.Instance.GetComponent<BattleAreaManager>();

        gameObject.SetActive(true);
        ShowSigns();
        HandleBorders();
        OnEnabled?.Invoke(this);
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
            if (directionToTile.z != 0) borderRotation = new Vector3(0f, 90f, 0f);

            GameObject border = Instantiate(_borderPrefab,
                                    borderPosition,
                                    Quaternion.Euler(borderRotation));
            border.transform.SetParent(transform);
            border.transform.localScale = borderScale;
            _borders.Add(border);
        }
    }
}
