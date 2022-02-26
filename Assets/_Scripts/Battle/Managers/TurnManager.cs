using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum BattleState { MapBuilding, Deployment, PlayerTurn, EnemyTurn, Won, Lost }

[RequireComponent(typeof(TurnDisplayer))]
public class TurnManager : MonoBehaviour
{
    // global
    JourneyManager journeyManager;

    public static BattleState battleState;
    public static int currentTurn = 0;

    InfoCardUI infoCardUI;

    List<GameObject> playerCharacters;
    List<GameObject> enemies;

    public int playerCharactersLeftToTakeTurn;
    public int enemyCharactersLeftToTakeTurn;

    public static event Action<BattleState> OnBattleStateChanged;

    public static TurnManager instance;
    void Awake()
    {
        #region Singleton
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
        infoCardUI = InfoCardUI.instance;
        journeyManager = JourneyManager.instance;

        currentTurn = 0;

        UpdateBattleState(BattleState.MapBuilding);
    }

    // https://www.youtube.com/watch?v=4I0vonyqMi8&t=193s
    public void UpdateBattleState(BattleState newState)
    {
        battleState = newState;

        switch (newState)
        {
            case BattleState.MapBuilding:
                HandleMapBuilding();
                break;
            case BattleState.Deployment:
                HandleDeployment();
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
    void HandleMapBuilding()
    {
    }

    void HandleDeployment()
    {
    }

    public void InitBattle()
    {
        playerCharacters = new(GameObject.FindGameObjectsWithTag("Player"));
        enemies = new(GameObject.FindGameObjectsWithTag("Enemy"));

        playerCharactersLeftToTakeTurn = playerCharacters.Count;
        enemyCharactersLeftToTakeTurn = enemies.Count;

        // subscribe to death events
        foreach (GameObject enemy in enemies)
            enemy.GetComponent<CharacterStats>().CharacterDeathEvent += OnEnemyDeath;
        foreach (GameObject player in playerCharacters)
            player.GetComponent<CharacterStats>().CharacterDeathEvent += OnPlayerCharDeath;
    }
    void HandlePlayerTurn()
    {
        // TODO: I don't think there is a need to get rid of this, but it is kinda sucky.
        if (currentTurn == 0)
            InitBattle();

        // TODO: do I need a separate method for starting or can I just use this;
        currentTurn++;

        // hide character card
        infoCardUI.HideCharacterCard();

        // TODO: Is this very taxing?   
        // Recalculate all graphs
        AstarPath.active.Scan();

        ResetCounts();
    }

    void HandleEnemyTurn()
    {
        // hide character card
        infoCardUI.HideCharacterCard();

        // TODO: Is this taxing?
        // Recalculate all graphs
        AstarPath.active.Scan();

        ResetCounts();
    }

    // TODO: eeee... does it make sense?
    void ResetCounts()
    {
        playerCharactersLeftToTakeTurn = playerCharacters.Count;
        enemyCharactersLeftToTakeTurn = enemies.Count;
    }

    public void OnPlayerCharDeath(GameObject _obj)
    {
        PlayerCharacterTurnFinished();

        playerCharacters.Remove(_obj);
        if (playerCharacters.Count <= 0)
            UpdateBattleState(BattleState.Lost);
    }

    public void OnEnemyDeath(GameObject _obj)
    {
        EnemyCharacterTurnFinished();

        enemies.Remove(_obj);
        if (enemies.Count <= 0)
            UpdateBattleState(BattleState.Won);
    }

    void HandleWinning()
    {
        Debug.Log("Congratz player! You win!!!");

        journeyManager.SetNodeReward(BattleManager.instance.GetReward());

        // TODO: maybe show a win screen, where you get reward, 
        // you characters level up and stuff and there is a button to go back to journey

        List<Character> playerCharactersAlive = new();
        foreach (GameObject p in playerCharacters)
            playerCharactersAlive.Add(p.GetComponent<CharacterStats>().character);
        journeyManager.SetPlayerTroops(playerCharactersAlive);

        journeyManager.LoadLevel("Journey");
    }

    void HandleLosing()
    {
        // TODO: this
        // for now game over screen
        // load home 
        Debug.Log("Ugh... you lost!");
    }

    public async void PlayerCharacterTurnFinished()
    {
        // -= player chars left. At 0 turn ends;
        playerCharactersLeftToTakeTurn -= 1;
        if (playerCharactersLeftToTakeTurn <= 0 && battleState != BattleState.EnemyTurn)
            await ChangeTurn(BattleState.EnemyTurn);
    }

    public async void EnemyCharacterTurnFinished()
    {
        // -= player chars left. At 0 turn ends;
        enemyCharactersLeftToTakeTurn -= 1;
        if (enemyCharactersLeftToTakeTurn <= 0 && battleState != BattleState.PlayerTurn)
            await ChangeTurn(BattleState.PlayerTurn);
    }

    async Task ChangeTurn(BattleState _state)
    {
        await Task.Delay(500);
        UpdateBattleState(_state);
    }

}
