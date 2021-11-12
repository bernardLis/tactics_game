using System.Collections.Generic;
using UnityEngine;

public class BattlePreparationController : MonoBehaviour
{

    // global utility
    Highlighter highlighter;

    [Header("Chararacter from SO")]
    public List<Character> charactersToPlace = new();
    public GameObject characterTemplate;

    public GameObject characterBeingPlaced { get; private set; }

    int characterBeingPlacedIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (TurnManager.battleState == BattleState.PREPARATION)
            InstantiateCharacter(0);

        highlighter = Highlighter.instance;
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
        // TODO: character being placed
        characterBeingPlacedIndex -= 1;
        if (characterBeingPlacedIndex < 0)
            characterBeingPlacedIndex = charactersToPlace.Count - 1;

        InstantiateCharacter(characterBeingPlacedIndex);

    }

    void InstantiateCharacter(int index)
    {
        if (characterBeingPlaced != null)
            Destroy(characterBeingPlaced);

        characterBeingPlaced = Instantiate(characterTemplate, transform.position, Quaternion.identity);
        characterBeingPlaced.name = charactersToPlace[index].characterName;
        Character instantiatedSO = Instantiate(charactersToPlace[index]);
        instantiatedSO.Initialize(characterBeingPlaced);
        characterBeingPlaced.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
    }

    // TODO: character being placed
    public void UpdateCharacterBeingPlacedPosition()
    {
        characterBeingPlaced.transform.position = transform.position;
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
#pragma warning disable CS4014
            highlighter.ClearHighlightedTiles();

            TurnManager.instance.StartBattle();
            return;
        }

        SelectNextCharacter();
    }
}
