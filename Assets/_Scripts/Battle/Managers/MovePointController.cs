using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePointController : MonoBehaviour
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

    public static MovePointController instance;
    void Awake()
    {
        #region singleton

        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of TurnManager found");
            return;
        }
        instance = this;
        #endregion

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        _infoCardUI = InfoCardUI.instance;
        _characterUI = CharacterUI.instance;

        // This is our Dictionary of tiles
        _tilemap = BattleManager.instance.GetComponent<TileManager>().Tilemap;

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
        transform.position = Highlighter.instance.HighlightedTiles[Mathf.FloorToInt(Highlighter.instance.HighlightedTiles.Count / 2)].GetMiddleOfTile();
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
        // check if there is a selectable object at selector's position
        // only one selectable object can occypy space: 
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // For placing characters during prep
        if (TurnManager.BattleState == BattleState.Deployment && _battleDeploymentController.CharacterBeingPlaced != null)
        {
            HandleBattlePrepSelectClick();
            return;
        }

        Select(col);
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

    void Select(Collider2D _obj)
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

        _battleCharacterController.Select(_obj);
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

        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col != null)
        {
            IUITextDisplayable textComponent = col.transform.parent.GetComponent<IUITextDisplayable>();
            if (textComponent != null)
                tileUIText += textComponent.DisplayText();
        }

        // hide/show the whole panel
        if (tileUIText == "")
            _infoCardUI.HideTileInfo();
        else
            _infoCardUI.ShowTileInfo(tileUIText);
    }

    void UpdateCharacterCardInfo()
    {
        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col == null)
        {
            _infoCardUI.HideCharacterCard();
            return;
        }

        // don't show card if you are hovering over selected character
        if (_battleCharacterController.SelectedCharacter == col.transform.parent.gameObject)
        {
            _infoCardUI.HideCharacterCard();
            return;
        }

        // show character card if there is a character there
        if (col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
        {
            _infoCardUI.ShowCharacterCard(col.transform.GetComponentInParent<CharacterStats>());
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
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);
        if (col == null)
            return;
        // TODO: maybe check for interfaces?
        if (!(col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider")))
            return;

        CharacterStats attacker = _battleCharacterController.SelectedCharacter.GetComponent<CharacterStats>();
        CharacterStats defender = col.transform.parent.GetComponent<CharacterStats>();

        _infoCardUI.ShowInteractionSummary(attacker, defender, selectedAbility);
    }
}

