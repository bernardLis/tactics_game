using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class CharacterSelection : MonoBehaviour
{
    protected Highlighter highlighter;

    // https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    protected Tilemap tilemap;
    protected WorldTile _tile;
    protected Dictionary<Vector3, WorldTile> tiles;

    // local
    protected CharacterStats myStats;
    SpriteRenderer[] spriteRenderers;
    public Color grayOutColor;

    public bool hasFinishedTurn { get; protected set; }


    //
    public virtual void Awake()
    {
        highlighter = Highlighter.instance;

        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

        myStats = GetComponent<CharacterStats>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    protected void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.EnemyTurn)
            HandleEnemyTurn();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    protected virtual void HandlePlayerTurn()
    {
        // meant to be overwritten
    }
    protected virtual void HandleEnemyTurn()
    {
        // meant to be overwritten
    }

    public virtual void FinishCharacterTurn()
    {
        myStats.SetAttacker(false);
        hasFinishedTurn = true;
        GrayOutCharacter();
    }

    protected void GrayOutCharacter()
    {
        foreach (SpriteRenderer rend in spriteRenderers)
            if (rend != null)
                rend.DOColor(grayOutColor, 1f);
    }

    void ReturnCharacterColor()
    {
        // shieet that looks way better than changing it right away;
        foreach (SpriteRenderer rend in spriteRenderers)
            if (rend != null)
                rend.DOColor(Color.white, 2f);
    }


}
