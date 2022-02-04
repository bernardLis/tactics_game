using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePointController : MonoBehaviour
{
    // TODO: movepoint should only be using battle ui
    CameraManager basicCameraFollow;
    InfoCardUI infoCardUI;
    CharacterUI characterUI;

    // tiles
    Tilemap tilemap;
    WorldTile _tile;

    BattleDeploymentController battleDeploymentController;
    BattleCharacterController battleCharacterController;
    SpriteRenderer spriteRenderer;



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

        basicCameraFollow = CameraManager.instance;
        infoCardUI = InfoCardUI.instance;
        characterUI = CharacterUI.instance;

        // This is our Dictionary of tiles
        tilemap = GameManager.instance.GetComponent<TileManager>().tilemap;

        battleDeploymentController = GetComponent<BattleDeploymentController>();
        battleCharacterController = GetComponent<BattleCharacterController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }


    void TurnManager_OnBattleStateChanged(BattleState _state)
    {
        if (_state == BattleState.MapBuilding)
            HandleMapBuilding();

        if (_state == BattleState.Deployment)
            HandleDeployment();

        if (_state == BattleState.PlayerTurn)
            Invoke("HandlePlayerTurn", 0.1f); // gives time for stats to resolve modifiers => UI displays correct numbers\

        if (_state == BattleState.EnemyTurn)
            HandleEnemyTurn(); // gives time for stats to resolve modifiers => UI displays correct numbers

    }

    void HandleMapBuilding()
    {

    }

    void HandleDeployment()
    {
        spriteRenderer.enabled = true;
        transform.position = Highlighter.instance.highlightedTiles[Mathf.FloorToInt(Highlighter.instance.highlightedTiles.Count / 2)].GetMiddleOfTile();
        battleDeploymentController.InstantiateCharacter(0);
    }

    void HandleEnemyTurn()
    {
        spriteRenderer.enabled = false;
    }

    public void Move(Vector3 _pos)
    {
        // block moving out form tile map
        Vector3Int tilePos = tilemap.WorldToCell(_pos);
        if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
            return;

        transform.position = _pos;

        // TODO: dunno if this is the correct way to handle this.
        battleCharacterController.DrawPath();

        UpdateDisplayInformation();

        // TODO: character being placed
        if (battleDeploymentController.characterBeingPlaced != null)
            battleDeploymentController.UpdateCharacterBeingPlacedPosition();
    }

    public void HandleSelectClick()
    {
        // check if there is a selectable object at selector's position
        // only one selectable object can occypy space: 
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // For placing characters during prep
        if (TurnManager.battleState == BattleState.Deployment && battleDeploymentController.characterBeingPlaced != null)
        {
            HandleBattlePrepSelectClick();
            return;
        }

        Select(col);
    }

    void HandleBattlePrepSelectClick()
    {
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
            return;
        if (!_tile.WithinRange)
            return;

        battleDeploymentController.PlaceCharacter();
    }

    void Select(Collider2D _obj)
    {
        UpdateDisplayInformation();

        // only within range tiles when selecting interaction target
        if (battleCharacterController.characterState == CharacterState.SelectingInteractionTarget)
        {
            Vector3Int tilePos = tilemap.WorldToCell(transform.position);
            if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
                return;
            if (!_tile.WithinRange)
                return;
        }

        battleCharacterController.Select(_obj);
    }

    void HandlePlayerTurn()
    {
        spriteRenderer.enabled = true;

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
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        string tileUIText = "";

        // if it is not a tile, return
        if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
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
            infoCardUI.HideTileInfo();
        else
            infoCardUI.ShowTileInfo(tileUIText);
    }

    void UpdateCharacterCardInfo()
    {
        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col == null)
        {
            infoCardUI.HideCharacterCard();
            return;
        }

        // don't show card if you are hovering over selected character
        if (battleCharacterController.selectedCharacter == col.transform.parent.gameObject)
        {
            infoCardUI.HideCharacterCard();
            return;
        }

        // show character card if there is a character there
        if (col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
        {
            infoCardUI.ShowCharacterCard(col.transform.GetComponentInParent<CharacterStats>());
            return;
        }

        // hide if it is something else
        infoCardUI.HideCharacterCard();
    }

    void ShowAbilityResult()
    {
        infoCardUI.HideInteractionSummary();
        characterUI.HideDamage();
        characterUI.HideManaUse();

        // only show interaction result when we are selecting a target
        if (battleCharacterController.characterState != CharacterState.ConfirmingInteraction)
            return;
        // and the ability is selected
        if (battleCharacterController.selectedAbility == null)
            return;
        Ability selectedAbility = battleCharacterController.selectedAbility;

        // mana use
        if (selectedAbility.manaCost != 0)
            characterUI.ShowManaUse(selectedAbility.manaCost);

        // don't show interaction summary if not in range of interaction
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
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

        CharacterStats attacker = battleCharacterController.selectedCharacter.GetComponent<CharacterStats>();
        CharacterStats defender = col.transform.parent.GetComponent<CharacterStats>();

        infoCardUI.ShowInteractionSummary(attacker, defender, selectedAbility);
    }

}

