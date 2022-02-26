using System.Collections.Generic;
using UnityEngine;

public class BattleDeploymentController : MonoBehaviour
{
    // global utility
    Highlighter highlighter;
    InfoCardUI infoCardUI;

    List<Character> charactersToPlace;
    public GameObject characterTemplate;
    public GameObject characterBeingPlaced { get; private set; }
    int characterBeingPlacedIndex = 0;

    void Awake()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }
    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void Start()
    {
        highlighter = Highlighter.instance;
        infoCardUI = InfoCardUI.instance;

        // TODO: here I can actually create characters from save data
        // no, actually characters are created by journey manager by loading save data, so you should just use them;
        string path = "Characters";
        Object[] playerCharacters = Resources.LoadAll(path, typeof(Character));
        charactersToPlace = new();
        foreach (Character character in playerCharacters)
            charactersToPlace.Add(character);
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.battleState == BattleState.MapBuilding)
            HandleMapBuilding();
    }

    void HandleMapBuilding()
    {
        if (characterBeingPlaced != null)
            Destroy(characterBeingPlaced);
    }

    public void SelectNextCharacter()
    {
        characterBeingPlacedIndex += 1;
        if (characterBeingPlacedIndex >= charactersToPlace.Count)
            characterBeingPlacedIndex = 0;

        InstantiateCharacter(characterBeingPlacedIndex);
    }

    public void SelectPreviousCharacter()
    {
        characterBeingPlacedIndex -= 1;
        if (characterBeingPlacedIndex < 0)
            characterBeingPlacedIndex = charactersToPlace.Count - 1;

        InstantiateCharacter(characterBeingPlacedIndex);
    }

    public void InstantiateCharacter(int index)
    {
        if (characterBeingPlaced != null)
            Destroy(characterBeingPlaced);

        characterBeingPlaced = Instantiate(characterTemplate, transform.position, Quaternion.identity);
        characterBeingPlaced.name = charactersToPlace[index].CharacterName;
        Character instantiatedSO = Instantiate(charactersToPlace[index]);
        instantiatedSO.Initialize(characterBeingPlaced);
        characterBeingPlaced.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

        EnemySpawnDirection dir = BattleManager.instance.GetComponent<BoardManager>().enemySpawnDirection;
        Vector2 faceDir = Vector2.left;
        if (dir == EnemySpawnDirection.Left)
            faceDir = Vector2.left;
        if (dir == EnemySpawnDirection.Right)
            faceDir = Vector2.right;
        if (dir == EnemySpawnDirection.Bottom)
            faceDir = Vector2.down;
        if (dir == EnemySpawnDirection.Top)
            faceDir = Vector2.up;

        characterBeingPlaced.GetComponentInChildren<CharacterRendererManager>().Face(faceDir);

        infoCardUI.ShowCharacterCard(characterBeingPlaced.GetComponent<CharacterStats>());
    }

    public void UpdateCharacterBeingPlacedPosition()
    {
        characterBeingPlaced.transform.position = transform.position;
        infoCardUI.ShowCharacterCard(characterBeingPlaced.GetComponent<CharacterStats>());
    }

    public void PlaceCharacter()
    {
        // remove the tile from highlighted tiles = block placement of other characters on the same tile
        Highlighter.instance.ClearHighlightedTile(characterBeingPlaced.transform.position);

        // nullify character being placed so it doesn't get destroyed;
        characterBeingPlaced = null;

        // remove character from the list
        charactersToPlace.RemoveAt(characterBeingPlacedIndex);

        // start the battle
        if (charactersToPlace.Count == 0)
        {
            // TODO: should I make it all async?
            highlighter.ClearHighlightedTiles().GetAwaiter();

            TurnManager.instance.UpdateBattleState(BattleState.PlayerTurn);
            return;
        }

        SelectNextCharacter();
    }
}
