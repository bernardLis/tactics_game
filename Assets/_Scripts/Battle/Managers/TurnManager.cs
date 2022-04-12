using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum BattleState { MapBuilding, Deployment, PlayerTurn, EnemyTurn, Won, Lost }

public class TurnManager : Singleton<TurnManager>
{
    // global
    JourneyManager _journeyManager;

    public static BattleState BattleState;
    public static int CurrentTurn = 0;

    InfoCardUI _infoCardUI;

    List<GameObject> _playerCharacters;
    List<GameObject> _enemies;

    int _playerCharactersLeftToTakeTurn;
    int _enemyCharactersLeftToTakeTurn;

    public static event Action<BattleState> OnBattleStateChanged;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _infoCardUI = InfoCardUI.Instance;
        _journeyManager = JourneyManager.Instance;

        CurrentTurn = 0;

        UpdateBattleState(BattleState.MapBuilding);
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

        _playerCharactersLeftToTakeTurn = _playerCharacters.Count;
        _enemyCharactersLeftToTakeTurn = _enemies.Count;

        // subscribe to death events
        foreach (GameObject enemy in _enemies)
            enemy.GetComponent<CharacterStats>().CharacterDeathEvent += OnEnemyDeath;
        foreach (GameObject player in _playerCharacters)
            player.GetComponent<CharacterStats>().CharacterDeathEvent += OnPlayerCharDeath;
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

    // TODO: eeee... does it make sense?
    void ResetCounts()
    {
        _playerCharactersLeftToTakeTurn = _playerCharacters.Count;
        _enemyCharactersLeftToTakeTurn = _enemies.Count;
    }

    public void OnPlayerCharDeath(GameObject _obj)
    {
        PlayerCharacterTurnFinished();

        _playerCharacters.Remove(_obj);
        if (_playerCharacters.Count <= 0)
            UpdateBattleState(BattleState.Lost);
    }

    public void OnEnemyDeath(GameObject _obj)
    {
        EnemyCharacterTurnFinished();

        _enemies.Remove(_obj);
        if (_enemies.Count <= 0)
            UpdateBattleState(BattleState.Won);
    }

    void HandleWinning()
    {
        Debug.Log("Congratz player! You win!!!");

        _journeyManager.SetNodeReward(BattleManager.Instance.GetReward());

        // TODO: maybe show a win screen, where you get reward, 
        // you characters level up and stuff and there is a button to go back to journey

        List<Character> playerCharactersAlive = new();
        foreach (GameObject p in _playerCharacters)
            playerCharactersAlive.Add(p.GetComponent<CharacterStats>().Character);

        _journeyManager.SetPlayerTroops(playerCharactersAlive);
    }

    void HandleLosing()
    {
        // TODO: this
        // for now game over screen
        // load home 
        Debug.Log("Ugh... you lost!");
        _journeyManager.ClearSaveData(); // TODO: this is not how it should be handled.
    }

    public async void PlayerCharacterTurnFinished()
    {
        _playerCharactersLeftToTakeTurn -= 1;
        if (_playerCharactersLeftToTakeTurn <= 0 && BattleState == BattleState.PlayerTurn)
            await ChangeTurn(BattleState.EnemyTurn);
    }

    public async void EnemyCharacterTurnFinished()
    {
        _enemyCharactersLeftToTakeTurn -= 1;
        if (_enemyCharactersLeftToTakeTurn <= 0 && BattleState == BattleState.EnemyTurn)
            await ChangeTurn(BattleState.PlayerTurn);
    }

    async Task ChangeTurn(BattleState _state)
    {
        await Task.Delay(500);

        if (BattleState == BattleState.Won || BattleState == BattleState.Lost)
            return;

        UpdateBattleState(_state);
    }

    public List<GameObject> GetEnemies()
    {
        return _enemies;
    }

    public List<GameObject> GetPlayerCharacters()
    {
        return _playerCharacters;
    }

}
