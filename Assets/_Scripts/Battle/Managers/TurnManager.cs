using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public class TurnManager : Singleton<TurnManager>
{
    // global
    GameManager _gameManager;

    public static BattleState BattleState;
    public static int CurrentTurn = 0;

    InfoCardUI _infoCardUI;

    List<GameObject> _playerCharacters;
    List<GameObject> _enemies;

    List<GameObject> _playerCharactersLeftToTakeTurn;
    List<GameObject> _enemyCharactersLeftToTakeTurn;

    public static event Action<BattleState> OnBattleStateChanged;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _infoCardUI = InfoCardUI.Instance;
        _gameManager = GameManager.Instance;

        CurrentTurn = 0;
    }

    // https://www.youtube.com/watch?v=4I0vonyqMi8&t=193s
    public void UpdateBattleState(BattleState newState)
    {
        BattleState = newState;
        switch (newState)
        {
            case BattleState.MapBuilding:
                break;
            case BattleState.Deployment:
                break;
            case BattleState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case BattleState.EnemyTurn:
                HandleEnemyTurn();
                break;
            case BattleState.Won:
                HandleWinning();
                break;
            case BattleState.Lost:
                HandleLosing();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnBattleStateChanged?.Invoke(newState);
    }

    public void InitBattle()
    {
        _playerCharacters = new(GameObject.FindGameObjectsWithTag("Player"));
        _enemies = new(GameObject.FindGameObjectsWithTag("Enemy"));

        _playerCharactersLeftToTakeTurn = new(_playerCharacters);
        _enemyCharactersLeftToTakeTurn = new(_enemies);

        // subscribe to death events
        foreach (GameObject enemy in _enemies)
            enemy.GetComponent<CharacterStats>().OnCharacterDeath += OnEnemyDeath;
        foreach (GameObject player in _playerCharacters)
            player.GetComponent<CharacterStats>().OnCharacterDeath += OnPlayerCharDeath;
    }

    public void AddEnemy(GameObject enemy)
    {
        _enemies.Add(enemy);
        enemy.GetComponent<CharacterStats>().OnCharacterDeath += OnEnemyDeath;
    }

    public void AddPlayer(GameObject player)
    {
        _playerCharacters.Add(player);
        player.GetComponent<CharacterStats>().OnCharacterDeath += OnPlayerCharDeath;
    }

    void HandlePlayerTurn()
    {
        // TODO: I don't think there is a need to get rid of this, but it is kinda sucky.
        if (CurrentTurn == 0)
            InitBattle();

        // TODO: do I need a separate method for starting or can I just use this;
        CurrentTurn++;

        // hide character card
        _infoCardUI.HideCharacterCard();

        ResetCounts();
    }

    void HandleEnemyTurn()
    {
        // hide character card
        _infoCardUI.HideCharacterCard();

        ResetCounts();
    }

    void ResetCounts()
    {
        _playerCharactersLeftToTakeTurn = new(GameObject.FindGameObjectsWithTag("Player"));
        _enemyCharactersLeftToTakeTurn = new(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    public void OnPlayerCharDeath(GameObject obj)
    {
        PlayerCharacterTurnFinished(obj);

        _playerCharacters.Remove(obj);
        if (_playerCharacters.Count <= 0)
            UpdateBattleState(BattleState.Lost);
    }

    public void OnEnemyDeath(GameObject obj)
    {
        EnemyCharacterTurnFinished(obj);

        _enemies.Remove(obj);
        if (_enemies.Count <= 0)
            UpdateBattleState(BattleState.Won);
    }

    void HandleWinning()
    {
        // TODO: this
        //_gameManager.BattleWon();
        Debug.Log("You won!");
    }

    void HandleLosing()
    {
        // TODO: this
        // for now game over screen
        // load home 
        // _gameManager.BattleLost();

        Debug.Log("Ugh... you lost!");
    }

    public async void PlayerCharacterTurnFinished(GameObject obj)
    {
        _playerCharactersLeftToTakeTurn.Remove(obj);
        if (_playerCharactersLeftToTakeTurn.Count <= 0 && BattleState == BattleState.PlayerTurn)
            await ChangeTurn(BattleState.EnemyTurn);
    }

    public async void EnemyCharacterTurnFinished(GameObject obj)
    {
        _enemyCharactersLeftToTakeTurn.Remove(obj);
        if (_enemyCharactersLeftToTakeTurn.Count <= 0 && BattleState == BattleState.EnemyTurn)
            await ChangeTurn(BattleState.PlayerTurn);
    }

    async Task ChangeTurn(BattleState _state)
    {
        await Task.Delay(500);

        if (BattleState == BattleState.Won || BattleState == BattleState.Lost)
            return;

        UpdateBattleState(_state);
    }

    public List<GameObject> GetEnemies() { return _enemies; }

    public List<GameObject> GetPlayerCharacters() { return _playerCharacters; }
}
