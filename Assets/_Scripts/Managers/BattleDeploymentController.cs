using System.Collections.Generic;
using UnityEngine;

public class BattleDeploymentController : MonoBehaviour
{

    // global utility
    Highlighter highlighter;
    InfoCardUI infoCardUI;

    [Header("Chararacter from SO")]
    public List<Character> charactersToPlace = new();
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
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        //if (TurnManager.battleState == BattleState.Deployment)
         //   InstantiateCharacter(0);
    }

    // Start is called before the first frame update

    public void SelectNextCharacter()
    {
        characterBeingPlacedIndex += 1;
        if (characterBeingPlacedIndex >= charactersToPlace.Count)
            characterBeingPlacedIndex = 0;

        InstantiateCharacter(characterBeingPlacedIndex);
    }

    public void SelectPreviousCharacter()
    {
        // TODO: character being placed
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
        characterBeingPlaced.name = charactersToPlace[index].characterName;
        Character instantiatedSO = Instantiate(charactersToPlace[index]);
        instantiatedSO.Initialize(characterBeingPlaced);
        characterBeingPlaced.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
        // TODO: maybe face enemy?
        characterBeingPlaced.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.left);

        infoCardUI.ShowCharacterCard(characterBeingPlaced.GetComponent<CharacterStats>());
    }

    // TODO: character being placed
    public void UpdateCharacterBeingPlacedPosition()
    {
        characterBeingPlaced.transform.position = transform.position;
        infoCardUI.ShowCharacterCard(characterBeingPlaced.GetComponent<CharacterStats>());
    }

    // TODO: character being placed
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
