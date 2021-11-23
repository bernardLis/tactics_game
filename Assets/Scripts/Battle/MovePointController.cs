using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePointController : MonoBehaviour
{
    // TODO: movepoint should only be using battle ui
    BasicCameraFollow basicCameraFollow;
    GameUI gameUI;
    CharacterUI characterUI;
    InfoCardUI infoCardUI;

    BattlePreparationController battlePreparationController;
    BattleCharacterController battleCharacterController;

    // tiles
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;

    // TODO: display some enemy info when you are hovering on them
    bool firstEnable = false;

    // highlighting enemy movmeent range on hover
    EnemyCharSelection enemyCharSelection;

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

        FindObjectOfType<TurnManager>().EnemyTurnEndEvent += OnEnemyTurnEnd;
        FindObjectOfType<TurnManager>().PlayerTurnEndEvent += OnPlayerTurnEnd;

        basicCameraFollow = BasicCameraFollow.instance;
        gameUI = GameUI.instance;
        characterUI = CharacterUI.instance;
        infoCardUI = InfoCardUI.instance;

        // This is our Dictionary of tiles
        tiles = GameTiles.instance.tiles;
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();

        battlePreparationController = GetComponent<BattlePreparationController>();
        battleCharacterController = GetComponent<BattleCharacterController>();
    }

    void Start()
    {
        UpdateTileInfoUI();
    }

    void OnEnable()
    {
        // TODO: THIS SUCKS but document ui is not ready on the first enable.
        if (!firstEnable)
            UpdateTileInfoUI();

        firstEnable = true;
    }

    void OnDisable()
    {
        gameUI.HideTileInfoUI();
    }

    public void Move(Vector3 pos)
    {
        // block moving out form tile map
        Vector3Int tilePos = tilemap.WorldToCell(pos);
        if (!tiles.TryGetValue(tilePos, out _tile))
            return;

        transform.position = pos;

        // TODO: dunno if this is the correct way to handle this.
        battleCharacterController.DrawPath();

        UpdateDisplayInformation();

        // TODO: character being placed
        if (battlePreparationController.characterBeingPlaced != null)
            battlePreparationController.UpdateCharacterBeingPlacedPosition();
    }

    public void HandleSelectClick()
    {
        // check if there is a selectable object at selector's position
        // only one selectable object can occypy space: 
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // For placing characters during prep
        if (TurnManager.battleState == BattleState.PREPARATION && battlePreparationController.characterBeingPlaced != null)
        {
            HandleBattlePrepSelectClick();
            return;
        }

        Select(col);
    }

    void HandleBattlePrepSelectClick()
    {
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        if (!tiles.TryGetValue(tilePos, out _tile))
            return;
        if (!_tile.WithinRange)
            return;

        battlePreparationController.PlaceCharacter();
    }

    public void UpdateDisplayInformation()
    {
        UpdateTileInfoUI();
        UpdateCharacterCardInfo();
    }

    void UpdateTileInfoUI()
    {
        // tile info
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        string tileUIText = "";

        // if it is not a tile, return
        if (!tiles.TryGetValue(tilePos, out _tile))
            return;

        if (_tile.IsObstacle)
            tileUIText = "Obstacle. ";


        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col == null)
            return;

        if (col.transform.CompareTag("Obstacle") || col.transform.CompareTag("Trap"))
        {
            UIText textScript = col.transform.GetComponent<UIText>();
            if (textScript != null)
                tileUIText = tileUIText + textScript.displayText;
        }

        // hide/show the whole panel
        if (tileUIText == "")
        {
            gameUI.HideTileInfoUI();
        }
        else
        {
            gameUI.UpdateTileInfoUI(tileUIText);
            gameUI.ShowTileInfoUI();
        }

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

    void Select(Collider2D obj)
    {
        battleCharacterController.Select(obj);
        UpdateCharacterCardInfo();
    }

    void OnEnemyTurnEnd()
    {
        GameObject[] playerChars = GameObject.FindGameObjectsWithTag("Player");
        if (playerChars.Length > 0)
            transform.position = playerChars[0].transform.position;

        // camera follows the movepoint again
        basicCameraFollow.followTarget = transform;

        UpdateTileInfoUI();
    }

    void OnPlayerTurnEnd()
    {
    }

}

