using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePointController : Singleton<MovePointController>
{
    // TODO: movepoint should only be using battle ui
    InfoCardUI _infoCardUI;
    CharacterUI _characterUI;

    // tiles
    Tilemap _tilemap;
    WorldTile _tile;

    BattleDeploymentController _battleDeploymentController;
    BattleCharacterController _battleCharacterController;
    SpriteRenderer _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

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


    void TurnManager_OnBattleStateChanged(BattleState _state)
    {
        if (_state == BattleState.Deployment)
            HandleDeployment();
        if (_state == BattleState.PlayerTurn)
            Invoke("HandlePlayerTurn", 0.1f); // gives time for stats to resolve modifiers => UI displays correct numbers\
        if (_state == BattleState.EnemyTurn)
            HandleEnemyTurn(); // gives time for stats to resolve modifiers => UI displays correct numbers
    }

    void HandleDeployment()
    {
        _spriteRenderer.enabled = true;
        transform.position = Highlighter.Instance.HighlightedTiles[Mathf.FloorToInt(Highlighter.Instance.HighlightedTiles.Count / 2)].GetMiddleOfTile();
        _battleDeploymentController.InstantiateCharacter(0);
    }

    void HandleEnemyTurn()
    {
        _spriteRenderer.enabled = false;
    }

    public void Move(Vector3 _pos)
    {
        // block moving out form tile map
        Vector3Int tilePos = _tilemap.WorldToCell(_pos);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        transform.position = _pos;

        // TODO: dunno if this is the correct way to handle this.
        _battleCharacterController.DrawPath();

        UpdateDisplayInformation();

        // TODO: character being placed
        if (_battleDeploymentController.CharacterBeingPlaced != null)
            _battleDeploymentController.UpdateCharacterBeingPlacedPosition();
    }

    public void HandleSelectClick()
    {
        // For placing characters during prep
        if (TurnManager.BattleState == BattleState.Deployment && _battleDeploymentController.CharacterBeingPlaced != null)
        {
            HandleBattlePrepSelectClick();
            return;
        }

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
        UpdateDisplayInformation();

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

    void HandlePlayerTurn()
    {
        _spriteRenderer.enabled = true;

        GameObject[] playerChars = GameObject.FindGameObjectsWithTag("Player");
        if (playerChars.Length > 0)
            transform.position = playerChars[0].transform.position;

        UpdateDisplayInformation();
    }

    public void UpdateDisplayInformation()
    {
        UpdateTileInfoUI();
        UpdateCharacterCardInfo();
        ShowAbilityResult();
    }

    // TODO: needs a rewrite
    void UpdateTileInfoUI()
    {
        // tile info
        Vector3Int tilePos = _tilemap.WorldToCell(transform.position);
        string tileUIText = "";

        // if it is not a tile, return
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        if (_tile.IsObstacle)
            tileUIText = "Obstacle. ";

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out IUITextDisplayable uiText))
                tileUIText += uiText.DisplayText();

        // hide/show the whole panel
        if (tileUIText == "")
            _infoCardUI.HideTileInfo();
        else
            _infoCardUI.ShowTileInfo(tileUIText);
    }

    void UpdateCharacterCardInfo()
    {
        // show character card if there is character standing there
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (_battleCharacterController.SelectedCharacter != c.gameObject &&
                (c.transform.CompareTag(Tags.Player) || c.transform.CompareTag(Tags.Enemy)))
            {
                _infoCardUI.ShowCharacterCard(c.transform.GetComponentInParent<CharacterStats>());
                return;
            }

        // hide if it is something else
        _infoCardUI.HideCharacterCard();
    }

    void ShowAbilityResult()
    {
        _infoCardUI.HideInteractionSummary();
        _characterUI.HideHealthChange();
        _characterUI.HideManaChange();

        // only show interaction result when we are selecting a target
        if (_battleCharacterController.CharacterState != CharacterState.ConfirmingInteraction)
            return;
        // and the ability is selected
        if (_battleCharacterController.SelectedAbility == null)
            return;
        Ability selectedAbility = _battleCharacterController.SelectedAbility;

        // mana use
        if (selectedAbility.ManaCost != 0)
            _characterUI.ShowManaChange(-1 * selectedAbility.ManaCost);

        // don't show interaction summary if not in range of interaction
        Vector3Int tilePos = _tilemap.WorldToCell(transform.position);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;
        if (!_tile.WithinRange)
            return;


        // check if there is a character standing there
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        /*
        if (col == null)
            return;
        // TODO: maybe check for interfaces?
        if (!(col.transform.CompareTag(Tags.Player) || col.transform.CompareTag(Tags.Enemy)))
            return;
        */
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
            {
                CharacterStats attacker = _battleCharacterController.SelectedCharacter.GetComponent<CharacterStats>();
                CharacterStats defender = c.GetComponent<CharacterStats>();

                _infoCardUI.ShowInteractionSummary(attacker, defender, selectedAbility);
            }
    }
}

