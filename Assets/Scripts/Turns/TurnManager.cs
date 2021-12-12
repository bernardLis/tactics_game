using UnityEngine;
using System;

public enum BattleState { Preparation, PlayerTurn, EnemyTurn, Won, Lost }

[RequireComponent(typeof(TurnDisplayer))]
public class TurnManager : MonoBehaviour
{
    public static BattleState battleState;
    public static int currentTurn = 0;

    InfoCardUI infoCardUI;

    GameObject[] playerCharacters;
    GameObject[] enemies;

    int playerCharactersLeftToTakeTurn;
    int enemyCharactersLeftToTakeTurn;

    int playerCharactersAlive;
    int enemyCharactersAlive;

    public static event Action<BattleState> OnBattleStateChanged;

    public static TurnManager instance;
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
        infoCardUI = InfoCardUI.instance;

        // TODO: create a start battleState where you place your characters
        UpdateBattleState(BattleState.Preparation);
    }

    // https://www.youtube.com/watch?v=4I0vonyqMi8&t=193s
    public void UpdateBattleState(BattleState newState)
    {
        battleState = newState;

        switch (newState)
        {
            case BattleState.Preparation:
                HandlePreparation();
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

    void HandlePreparation()
    {
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

        // reset counts
        playerCharactersLeftToTakeTurn = playerCharactersAlive;
        enemyCharactersLeftToTakeTurn = enemyCharactersAlive;
    }

    // TODO: this will be called when player places their characters and confirms that he wants to start the battle.
    public void InitBattle()
    {
        playerCharacters = GameObject.FindGameObjectsWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        playerCharactersLeftToTakeTurn = playerCharacters.Length;
        enemyCharactersLeftToTakeTurn = enemies.Length;

        playerCharactersAlive = playerCharacters.Length;
        enemyCharactersAlive = enemies.Length;

        // subscribe to death events
        foreach (GameObject enemy in enemies)
            enemy.GetComponent<CharacterStats>().CharacterDeathEvent += OnEnemyDeath;
        foreach (GameObject player in playerCharacters)
            player.GetComponent<CharacterStats>().CharacterDeathEvent += OnPlayerCharDeath;
    }

    void HandleEnemyTurn()
    {
        // hide character card
        infoCardUI.HideCharacterCard();

        // TODO: Is this taxing?
        // Recalculate all graphs
        AstarPath.active.Scan();

        // reset counts
        playerCharactersLeftToTakeTurn = playerCharactersAlive;
        enemyCharactersLeftToTakeTurn = enemyCharactersAlive;
    }

    public void OnEnemyDeath()
    {
        enemyCharactersAlive--;
        if (enemyCharactersAlive <= 0)
            UpdateBattleState(BattleState.Won);
    }

    public void OnPlayerCharDeath()
    {
        playerCharactersAlive--;
        if (playerCharactersAlive <= 0)
            UpdateBattleState(BattleState.Lost);
    }

    void HandleWinning()
    {
        // TODO: this
        Debug.Log("Congratz player! You win!!!");
    }

    void HandleLosing()
    {
        // TODO: this
        Debug.Log("Ugh... you lost!");
    }

    public void PlayerCharacterTurnFinished()
    {
        // -= player chars left. At 0 turn ends;
        playerCharactersLeftToTakeTurn -= 1;
        if (playerCharactersLeftToTakeTurn <= 0)
            UpdateBattleState(BattleState.EnemyTurn);
    }

    public void EnemyCharacterTurnFinished()
    {
        // -= player chars left. At 0 turn ends;
        enemyCharactersLeftToTakeTurn -= 1;
        if (enemyCharactersLeftToTakeTurn <= 0)
            UpdateBattleState(BattleState.PlayerTurn);
    }
}
