using System.Collections.Generic;
using UnityEngine;

public class BattleDeploymentController : MonoBehaviour
{
    // global utility
    Highlighter _highlighter;
    InfoCardUI _infoCardUI;

    List<Character> _charactersToPlace;
    public GameObject CharacterTemplate;
    public GameObject CharacterBeingPlaced { get; private set; }
    int _characterBeingPlacedIndex = 0;

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
        _highlighter = Highlighter.instance;
        _infoCardUI = InfoCardUI.instance;

        // TODO: here I can actually create characters from save data
        // no, actually characters are created by journey manager by loading save data, so you should just use them;
        string path = "Characters";
        Object[] playerCharacters = Resources.LoadAll(path, typeof(Character));
        _charactersToPlace = new();
        foreach (Character character in playerCharacters)
            _charactersToPlace.Add(character);
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.BattleState == BattleState.MapBuilding)
            HandleMapBuilding();
    }

    void HandleMapBuilding()
    {
        if (CharacterBeingPlaced != null)
            Destroy(CharacterBeingPlaced);
    }

    public void SelectNextCharacter()
    {
        _characterBeingPlacedIndex += 1;
        if (_characterBeingPlacedIndex >= _charactersToPlace.Count)
            _characterBeingPlacedIndex = 0;

        InstantiateCharacter(_characterBeingPlacedIndex);
    }

    public void SelectPreviousCharacter()
    {
        _characterBeingPlacedIndex -= 1;
        if (_characterBeingPlacedIndex < 0)
            _characterBeingPlacedIndex = _charactersToPlace.Count - 1;

        InstantiateCharacter(_characterBeingPlacedIndex);
    }

    public void InstantiateCharacter(int index)
    {
        if (CharacterBeingPlaced != null)
            Destroy(CharacterBeingPlaced);

        CharacterBeingPlaced = Instantiate(CharacterTemplate, transform.position, Quaternion.identity);
        CharacterBeingPlaced.name = _charactersToPlace[index].CharacterName;
        Character instantiatedSO = Instantiate(_charactersToPlace[index]);
        instantiatedSO.Initialize(CharacterBeingPlaced);
        CharacterBeingPlaced.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

        EnemySpawnDirection dir = BattleManager.instance.GetComponent<BoardManager>().EnemySpawnDirection;
        Vector2 faceDir = Vector2.left;
        if (dir == EnemySpawnDirection.Left)
            faceDir = Vector2.left;
        if (dir == EnemySpawnDirection.Right)
            faceDir = Vector2.right;
        if (dir == EnemySpawnDirection.Bottom)
            faceDir = Vector2.down;
        if (dir == EnemySpawnDirection.Top)
            faceDir = Vector2.up;

        CharacterBeingPlaced.GetComponentInChildren<CharacterRendererManager>().Face(faceDir);

        _infoCardUI.ShowCharacterCard(CharacterBeingPlaced.GetComponent<CharacterStats>());
    }

    public void UpdateCharacterBeingPlacedPosition()
    {
        CharacterBeingPlaced.transform.position = transform.position;
        _infoCardUI.ShowCharacterCard(CharacterBeingPlaced.GetComponent<CharacterStats>());
    }

    public void PlaceCharacter()
    {
        // remove the tile from highlighted tiles = block placement of other characters on the same tile
        Highlighter.instance.ClearHighlightedTile(CharacterBeingPlaced.transform.position);

        // nullify character being placed so it doesn't get destroyed;
        CharacterBeingPlaced = null;

        // remove character from the list
        _charactersToPlace.RemoveAt(_characterBeingPlacedIndex);

        // start the battle
        if (_charactersToPlace.Count == 0)
        {
            // TODO: should I make it all async?
            _highlighter.ClearHighlightedTiles().GetAwaiter();

            TurnManager.instance.UpdateBattleState(BattleState.PlayerTurn);
            return;
        }

        SelectNextCharacter();
    }
}
