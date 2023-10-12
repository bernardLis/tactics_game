using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLandTile : MonoBehaviour
{
    BattleAreaManager _battleAreaManager;
    [SerializeField] GameObject _landPurchaseSignPrefab;

    public List<BattleLandPurchaseSign> _signs;

    void Start()
    {
        _battleAreaManager = BattleManager.Instance.GetComponent<BattleAreaManager>();
    }

    [ContextMenu("ShowSigns")]
    void ShowSigns()
    {
        List<BattleLandTile> adjacentTiles = _battleAreaManager.GetAdjacentTiles(this);
        foreach (BattleLandTile tile in adjacentTiles)
        {
            // place them on the tile, but in direction of adjacent tile
            Vector3 directionToTile = (tile.transform.position - transform.position).normalized;
            Vector3 signPosition = directionToTile * transform.localScale.x * 10 * 0.4f;

            BattleLandPurchaseSign sign = Instantiate(_landPurchaseSignPrefab,
                    signPosition, Quaternion.identity).GetComponent<BattleLandPurchaseSign>();
            sign.transform.SetParent(transform);
            Debug.Log($"tile {tile}");
            sign.Initialize(tile);

            _signs.Add(sign);
            sign.OnPurchased += OnTilePurchased;
        }
        // instantiate signs for each adjacent tile that is not owned by the player


    }

    void OnTilePurchased()
    {

    }
}
