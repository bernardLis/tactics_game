using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.Threading.Tasks;

public enum CharacterState { None, Selected, SelectingInteractionTarget, SelectingFaceDir, ConfirmingInteraction }

public class BattleCharacterController : MonoBehaviour
{
    // tilemap
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;
    WorldTile selectedTile;

    // global utilities
    Highlighter highlighter;
    CharacterUI characterUI;
    BattleInputController battleInputController;
    MovePointController movePointController;

    // I will be caching them for selected character
    public GameObject selectedCharacter;
    CharacterStats playerStats;
    PlayerCharSelection playerCharSelection;
    AILerp aILerp;
    AIDestinationSetter destinationSetter;
    CharacterRendererManager characterRendererManager;

    // state
    public CharacterState characterState { get; private set; }

    // selection
    bool isSelectionBlocked; // block mashing select character coz it breaks the highlight - if you quickly switch between selection of 2 chars.

    // movement
    GameObject tempObject;
    bool hasCharacterStartedMoving;
    bool hasCharacterGoneBack;

    Seeker seeker;
    LineRenderer pathRenderer;

    // interactions
    public Ability selectedAbility { get; private set; }
    bool isInteracting;

    // TODO: I am currently, not using that, but I have a feeling that it will be useful.
    public static event Action<CharacterState> OnCharacterStateChanged;

    public static BattleCharacterController instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of TurnManager found");
            return;
        }
        instance = this;
        #endregion
    }

    void Start()
    {
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

        highlighter = Highlighter.instance;
        battleInputController = BattleInputController.instance;
        characterUI = CharacterUI.instance;
        movePointController = MovePointController.instance;

        seeker = GetComponent<Seeker>();
        pathRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // TODO: is there a better way? 
        if (selectedCharacter == null)
            return;

        if (hasCharacterStartedMoving && tempObject != null
            && Vector3.Distance(selectedCharacter.transform.position, tempObject.transform.position) <= 0.1f)
            CharacterReachedDestination();
    }

    // https://www.youtube.com/watch?v=4I0vonyqMi8&t=193s
    public void UpdateCharacterState(CharacterState newState)
    {
        characterState = newState;
        // TODO: this is not really implemented...
        switch (newState)
        {
            case CharacterState.None:
                break;
            case CharacterState.Selected:
                break;
            case CharacterState.SelectingInteractionTarget:
                break;
            case CharacterState.SelectingFaceDir:
                break;
            case CharacterState.ConfirmingInteraction:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

        }
        OnCharacterStateChanged?.Invoke(newState);
    }


    public void Select(Collider2D _col)
    {
        ClearPathRenderer();

        // get the tile movepoint is on
        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            selectedTile = _tile;

        // select character
        if (_col != null && CanSelectCharacter(_col))
        {
            SelectCharacter(_col.transform.parent.gameObject);
            return;
        }

        if (selectedCharacter == null)
            return;

        // when character is selected

        // Move
        if (CanMoveCharacter() && selectedTile.WithinRange)
        {
            Move();
            return;
        }

        Interact();
    }

    bool CanSelectCharacter(Collider2D _col)
    {
        if (isSelectionBlocked)
            return false;
        if (_col == null)
            return false;
        // can select only player characters
        if (!_col.transform.CompareTag("PlayerCollider"))
            return false;
        // character allows selection
        if (!_col.GetComponentInParent<PlayerCharSelection>().CanBeSelected())
            return false;
        // don't allow to select another character if you have moved this character and did not take the action
        if (selectedCharacter != null && playerCharSelection.hasMovedThisTurn && !playerCharSelection.hasFinishedTurn)
            return false;
        // don't allow to select another character if you are triggering ability;
        if (selectedAbility != null)
            return false;

        return true;
    }

    bool CanMoveCharacter()
    {
        if (playerCharSelection.hasMovedThisTurn)
            return false;
        if (playerCharSelection.hasFinishedTurn)
            return false;
        if (selectedAbility != null)
            return false;

        return true;
    }

    // for ability button clicks
    public bool CanSelectAbility()
    {
        if (!battleInputController.IsInputAllowed())
            return false;
        if (selectedCharacter == null)
            return false;
        if (playerCharSelection.hasFinishedTurn)
            return false;
        if (hasCharacterStartedMoving) // don't allow button click when character is moving;
            return false;
        return true;
    }

    async void SelectCharacter(GameObject _character)
    {
        isSelectionBlocked = true;
        UpdateCharacterState(CharacterState.Selected);

        if (_character.GetComponent<PlayerCharSelection>().hasMovedThisTurn)
            return;

        // character specific ui
        if (playerCharSelection != null)
            playerCharSelection.DeselectCharacter();

        // cache
        selectedCharacter = _character;
        playerStats = selectedCharacter.GetComponent<CharacterStats>();
        playerCharSelection = selectedCharacter.GetComponent<PlayerCharSelection>();
        aILerp = selectedCharacter.GetComponent<AILerp>();

        //seeker = selectedCharacter.GetComponent<Seeker>();
        destinationSetter = selectedCharacter.GetComponent<AIDestinationSetter>();
        characterRendererManager = selectedCharacter.GetComponentInChildren<CharacterRendererManager>();

        // character specific ui
        playerCharSelection.SelectCharacter();

        // UI
        characterUI.ShowCharacterUI(playerStats);

        // highlight
        await highlighter.HiglightPlayerMovementRange(selectedCharacter.transform.position, playerStats.movementRange.GetValue(),
                                                new Color(0.53f, 0.52f, 1f, 1f));
        isSelectionBlocked = false;
    }

    void Move()
    {
        hasCharacterStartedMoving = true;

        // TODO: should I make it all async?
        highlighter.ClearHighlightedTiles().GetAwaiter();

        tempObject = new GameObject("Destination");
        tempObject.transform.position = transform.position;
        destinationSetter.target = tempObject.transform;

        characterUI.DisableSkillButtons();

        playerCharSelection.SetCharacterMoved(true);
    }

    public void Back()
    {
        ClearPathRenderer();
        movePointController.UpdateDisplayInformation();

        // if back is called during character movement return
        if (hasCharacterStartedMoving)
            return;

        if (selectedCharacter == null)
            return;

        if (characterState == CharacterState.ConfirmingInteraction)
        {
            BackFromConfirmingInteraction();
            return;
        }

        if (characterState == CharacterState.SelectingFaceDir)
        {
            BackFromFaceDirSelection();
            return;
        }

        if (selectedAbility != null)
        {
            BackFromAbilitySelection();
            return;
        }

        if (!playerCharSelection.hasMovedThisTurn)
        {
            UnselectCharacter();
            return;
        }

        // flag reset
        playerCharSelection.SetCharacterMoved(false);

        hasCharacterGoneBack = true;
        hasCharacterStartedMoving = true;

        // move character to character's starting position quickly.
        aILerp.speed = 15;

        transform.position = playerCharSelection.positionTurnStart;

        tempObject = new GameObject("Back Destination");
        tempObject.transform.position = playerCharSelection.positionTurnStart;
        destinationSetter.target = tempObject.transform;

        characterUI.DisableSkillButtons();
    }

    void CharacterReachedDestination()
    {
        characterUI.EnableSkillButtons();

        if (tempObject != null)
            Destroy(tempObject);

        // reset flag
        hasCharacterStartedMoving = false;

        // check if it was back or normal move
        if (!hasCharacterGoneBack)
            return;

        // highlight movement range if character was going back
        hasCharacterGoneBack = false;
        highlighter.HiglightPlayerMovementRange(selectedCharacter.transform.position, playerStats.movementRange.GetValue(),
                                    new Color(0.53f, 0.52f, 1f, 1f)).GetAwaiter();

    }

    public void SetSelectedAbility(Ability ability)
    {
        ClearPathRenderer();
        selectedAbility = ability;

        movePointController.UpdateDisplayInformation();
    }

    async void Interact()
    {
        // TODO: this is a meh schema
        if (isInteracting)
            return;
        isInteracting = true;

        // highlight aoe
        if (characterState == CharacterState.SelectingInteractionTarget)
        {
            characterRendererManager.Face((transform.position - selectedCharacter.transform.position).normalized);
            await selectedAbility.HighlightAreaOfEffect(transform.position);
            movePointController.UpdateDisplayInformation();
            isInteracting = false;
            return;
        }

        await TriggerAbility();
        isInteracting = false;
        FinishCharacterTurn();
    }

    async Task TriggerAbility()
    {
        int successfullAttacks = 0;
        List<WorldTile> highlightedTiles = highlighter.highlightedTiles;

        // for each tile of highlighted tiles -
        // TODO: kinda sucks to be using highlighted tiles, I could calculate the affected tiles
        foreach (WorldTile t in highlightedTiles)
        {
            Vector3 pos = new Vector3(t.LocalPlace.x + 0.5f, t.LocalPlace.y + 0.5f, transform.position.z);

            // check if there is an object there and try to attack it
            Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);

            if (col == null)
                continue;

            if (await selectedAbility.TriggerAbility(col.transform.parent.gameObject))
                successfullAttacks++;
        }
    }

    void BackFromAbilitySelection()
    {
        isInteracting = false;

        UpdateCharacterState(CharacterState.Selected);

        selectedAbility = null;
        characterUI.HideAbilityTooltip();

        highlighter.ClearHighlightedTiles().GetAwaiter();
    }

    void BackFromFaceDirSelection()
    {
        isInteracting = false;

        // TODO:cache face direction ui if it is the right approach
        selectedCharacter.GetComponent<FaceDirectionUI>().HideUI();
        playerCharSelection.ToggleSelectionArrow(true);

        // abilities that can target self should go back to Select target
        if (selectedAbility.canTargetSelf)
        {
            // it changes the state too
            selectedAbility.HighlightTargetable(selectedCharacter).GetAwaiter();

            return;
        }

        // deselect ability if it was an ability that goes straight to facing dir;
        BackFromAbilitySelection();
        UpdateCharacterState(CharacterState.Selected);
    }

    void BackFromConfirmingInteraction()
    {
        isInteracting = false;
        UpdateCharacterState(CharacterState.SelectingInteractionTarget);
        selectedAbility.HighlightTargetable(selectedCharacter).GetAwaiter();
    }


    void FinishCharacterTurn()
    {
        isInteracting = false;
        
        // necessary for movepoint to correctly update UI;
        UpdateCharacterState(CharacterState.None);

        // update ui through movepoint
        movePointController.UpdateDisplayInformation();

        // set flags in player char selection
        if (playerCharSelection != null)
            playerCharSelection.FinishCharacterTurn();

        // clearing the cache here
        UnselectCharacter();
    }

    public void UnselectCharacter()
    {
        UpdateCharacterState(CharacterState.None);

        // character specific ui
        if (playerCharSelection != null)
            playerCharSelection.DeselectCharacter();

        selectedCharacter = null;
        playerStats = null;
        playerCharSelection = null;
        selectedAbility = null;

        // UI
        characterUI.HideCharacterUI();

        // highlight
        highlighter.ClearHighlightedTiles().GetAwaiter();
    }

    // TODO: this probably shouldn't be here 
    public void DrawPath()
    {
        ClearPathRenderer();

        // only draw path when character is selected
        if (selectedCharacter == null)
            return;

        // don't draw path if ability is selected
        if (selectedAbility != null)
            return;

        // get the tile movepoint is on
        if (!tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            return;
        // don't draw path to tiles you can't reach
        if (!_tile.WithinRange)
            return;

        // https://arongranberg.com/astar/docs_dev/calling-pathfinding.php
        var path = seeker.StartPath(selectedCharacter.transform.position, transform.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error)
            return;

        // draw path with line renderer
        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            pathRenderer.positionCount = p.vectorPath.Count;
            pathRenderer.SetPosition(i, new Vector3(p.vectorPath[i].x, p.vectorPath[i].y, -1f));
        }
    }

    void ClearPathRenderer()
    {
        pathRenderer.positionCount = 0;
    }
}
