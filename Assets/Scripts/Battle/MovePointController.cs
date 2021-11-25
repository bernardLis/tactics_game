using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePointController : MonoBehaviour
{
    // TODO: movepoint should only be using battle ui
    BasicCameraFollow basicCameraFollow;
    GameUI gameUI;
    InfoCardUI infoCardUI;

    BattlePreparationController battlePreparationController;
    BattleCharacterController battleCharacterController;

    // tiles
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;

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

        basicCameraFollow = BasicCameraFollow.instance;
        gameUI = GameUI.instance;
        infoCardUI = InfoCardUI.instance;

        // This is our Dictionary of tiles
        tiles = GameTiles.instance.tiles;
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();

        battlePreparationController = GetComponent<BattlePreparationController>();
        battleCharacterController = GetComponent<BattleCharacterController>();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }


    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
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
        if (TurnManager.battleState == BattleState.Preparation && battlePreparationController.characterBeingPlaced != null)
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

    void Select(Collider2D obj)
    {
        battleCharacterController.Select(obj);
        UpdateDisplayInformation();
    }

    void HandlePlayerTurn()
    {
        GameObject[] playerChars = GameObject.FindGameObjectsWithTag("Player");
        if (playerChars.Length > 0)
            transform.position = playerChars[0].transform.position;

        // camera follows the movepoint again
        basicCameraFollow.followTarget = transform;

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
            infoCardUI.HideTileInfo();
            //gameUI.HideTileInfoUI();
        }
        else
        {
            infoCardUI.ShowTileInfo(tileUIText);
           // gameUI.UpdateTileInfoUI(tileUIText);
            //gameUI.ShowTileInfoUI();
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

    void ShowAbilityResult()
    {
        if (battleCharacterController.characterState != CharacterState.SelectingInteractionTarget)
            return;

        if (battleCharacterController.selectedAbility == null)
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

        if (battleCharacterController.selectedAbility.aType == AbilityType.ATTACK)
            infoCardUI.ShowDamage(defender, CalculateInteractionResult(attacker, defender));
        if (battleCharacterController.selectedAbility.aType == AbilityType.HEAL)
            infoCardUI.ShowHeal(defender, CalculateInteractionResult(attacker, defender));
    }

    int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender)
    {
        int result = 0;

        // TODO: differentiate between abilities that calculate value from int/str
        if (battleCharacterController.selectedAbility.aType == AbilityType.ATTACK)
            result = battleCharacterController.selectedAbility.value + attacker.strength.GetValue() - defender.armor.GetValue();

        if (battleCharacterController.selectedAbility.aType == AbilityType.HEAL)
            result = battleCharacterController.selectedAbility.value + attacker.intelligence.GetValue();


        return result;
    }
}

