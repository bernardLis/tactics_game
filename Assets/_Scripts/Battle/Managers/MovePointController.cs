using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class MovePointController : Singleton<MovePointController>
{
    AudioManager _audioManager;

    // TODO: movepoint should only be using battle ui
    InfoCardUI _infoCardUI;
    CharacterUI _characterUI;

    // tiles
    Tilemap _tilemap;
    WorldTile _tile;

    BattleDeploymentController _battleDeploymentController;
    BattleCharacterController _battleCharacterController;
    SpriteRenderer _spriteRenderer;

    // delegates
    public static event Action<Vector3> OnMove;


    protected override void Awake()
    {
        base.Awake();
        _audioManager = AudioManager.Instance;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        _infoCardUI = InfoCardUI.Instance;
        _characterUI = CharacterUI.Instance;

        // This is our Dictionary of tiles
        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _battleDeploymentController = GetComponent<BattleDeploymentController>();
        _battleCharacterController = GetComponent<BattleCharacterController>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Deployment)
            HandleDeployment();
        if (state == BattleState.PlayerTurn)
            Invoke("HandlePlayerTurn", 0.1f); // gives time for stats to resolve modifiers => UI displays correct numbers\
        if (state == BattleState.EnemyTurn)
            HandleEnemyTurn(); // gives time for stats to resolve modifiers => UI displays correct numbers
    }

    void HandleDeployment()
    {
        _spriteRenderer.enabled = true;
        transform.position = HighlightManager.Instance.HighlightedTiles[Mathf.FloorToInt(HighlightManager.Instance.HighlightedTiles.Count / 2)].GetMiddleOfTile();
        _battleDeploymentController.InstantiateCharacter(0);
    }

    void HandleEnemyTurn()
    {
        _infoCardUI.HideTileInfo();
        _spriteRenderer.enabled = false;
    }

    void HandlePlayerTurn()
    {
        _spriteRenderer.enabled = true;

        GameObject[] playerChars = GameObject.FindGameObjectsWithTag("Player");
        if (playerChars.Length > 0)
            transform.position = playerChars[0].transform.position;

        OnMove?.Invoke(playerChars[0].transform.position);
    }


    public void Move(Vector3 pos)
    {
        // block moving out of tile map
        Vector3Int tilePos = _tilemap.WorldToCell(pos);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        
        // snap movepoint to 0.5 0.5
        float snappedX = Mathf.Round(pos.x * 2) * 0.5f;
        float snappedY = Mathf.Round(pos.y * 2) * 0.5f;

        _audioManager.PlaySFX("Clicks", transform.position);

        transform.position = new Vector3(snappedX, snappedY);

        OnMove?.Invoke(pos);
    }

    public void HandleSelectClick()
    {
        // For placing characters during prep
        if (TurnManager.BattleState == BattleState.Deployment && _battleDeploymentController.CharacterBeingPlaced != null)
        {
            HandleBattlePrepSelectClick();
            return;
        }

        if (TurnManager.BattleState == BattleState.PlayerTurn)
            Select();
    }

    void HandleBattlePrepSelectClick()
    {
        Vector3Int tilePos = _tilemap.WorldToCell(transform.position);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;
        if (!_tile.WithinRange)
            return;

        _battleDeploymentController.PlaceCharacter();
    }

    void Select()
    {
        // only within range tiles when selecting interaction target
        if (_battleCharacterController.CharacterState == CharacterState.SelectingInteractionTarget)
        {
            Vector3Int tilePos = _tilemap.WorldToCell(transform.position);
            if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
                return;
            if (!_tile.WithinRange)
                return;
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        _battleCharacterController.Select(cols);
    }
}

