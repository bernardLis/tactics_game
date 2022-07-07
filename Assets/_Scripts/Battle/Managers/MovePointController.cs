using UnityEngine;
using UnityEngine.Tilemaps;

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

        UpdateDisplayInformation();
    }


    public void Move(Vector3 pos)
    {
        // block moving out of tile map
        Vector3Int tilePos = _tilemap.WorldToCell(pos);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        // snap movepoint to 0.5 0.5
        Debug.Log($"x: {pos.x}");
        Debug.Log($"y: {pos.y}");
        float snappedX = Mathf.Round(pos.x * 2) * 0.5f;
        float snappedY = Mathf.Round(pos.y * 2) * 0.5f;
        Debug.Log($"snappedX: {snappedX}");
        Debug.Log($"snappedY: {snappedY}");

        _audioManager.PlaySFX("Clicks", transform.position);

        transform.position = new Vector3(snappedX, snappedY);

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


    public void UpdateDisplayInformation()
    {
        UpdateTileInfoUI();
        UpdateCharacterCardInfo();
        UpdateAbilityResult();
    }

    void UpdateTileInfoUI()
    {
        string tileUIText = "";

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
        {
            if (c.TryGetComponent(out IUITextDisplayable uiText))
                tileUIText += uiText.DisplayText();
            if (c.CompareTag(Tags.BoundCollider))
                tileUIText += "Impassable map bounds.";
        }

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
        if (TurnManager.BattleState != BattleState.Deployment)
            _infoCardUI.HideCharacterCard();
    }

    public void UpdateAbilityResult()
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
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
            {
                CharacterStats attacker = _battleCharacterController.SelectedCharacter.GetComponent<CharacterStats>();
                CharacterStats defender = c.GetComponent<CharacterStats>();

                _infoCardUI.ShowInteractionSummary(attacker, defender, selectedAbility);
            }
    }
}

