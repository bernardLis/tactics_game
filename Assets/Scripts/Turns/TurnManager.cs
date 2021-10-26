using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState { PREPARATION, PLAYERTURN, ENEMYTURN, WON, LOST }

[RequireComponent(typeof(TurnDisplayer))]
public class TurnManager : MonoBehaviour
{
    public static BattleState battleState;
    public static int currentTurn = 0;

    Highlighter highlighter;

    public static TurnManager instance;
    GameObject[] playerCharacters;
    GameObject[] enemies;



    int playerCharactersLeftToTakeTurn;
    int enemyCharactersLeftToTakeTurn;

    int playerCharactersAlive;
    int enemyCharactersAlive;

    public event Action enemyTurnEndEvent;
    public event Action playerTurnEndEvent;

    // get the amount of player characters
    // each time player character finishes its move subtract one from the total
    // if all player characters finished their turn -> start a new turn
    void Awake()
    {
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of TurnManager found");
            return;
        }
        instance = this;
    }

    void Start()
    {
        highlighter = Highlighter.instance;
        // TODO: create a start battleState where you place your characters
        battleState = BattleState.PREPARATION;
    }

    // TODO: this will be called when player places their characters and confirms that he wants to start the battle.
    public void StartBattle()
    {
        battleState = BattleState.PLAYERTURN;

        playerCharacters = GameObject.FindGameObjectsWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        playerCharactersLeftToTakeTurn = playerCharacters.Length;
        enemyCharactersLeftToTakeTurn = enemies.Length;

        playerCharactersAlive = playerCharacters.Length;
        enemyCharactersAlive = enemies.Length;

        // subscribe to death events
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<CharacterStats>().characterDeathEvent += OnEnemyDeath;
        }
        foreach (GameObject player in playerCharacters)
        {
            player.GetComponent<CharacterStats>().characterDeathEvent += OnPlayerCharDeath;
        }
        
        // TODO: do I need a separate method for starting or can I just use this;
        EndEnemyTurn();
    }


    public void EndPlayerTurn()
    {
        // display text & run enemy ai
        if (playerTurnEndEvent != null)
        {
            playerTurnEndEvent();
        }

        battleState = BattleState.ENEMYTURN;

        // TODO: Is this taxing?
        // Recalculate all graphs
        AstarPath.active.Scan();

        // reset counts
        playerCharactersLeftToTakeTurn = playerCharactersAlive;
        enemyCharactersLeftToTakeTurn = enemyCharactersAlive;
    }

    public void EndEnemyTurn()
    {
        currentTurn++;

        // display text
        if (enemyTurnEndEvent != null)
        {
            enemyTurnEndEvent();
        }

        //yield return new WaitForSeconds(1f);

        battleState = BattleState.PLAYERTURN;

        // TODO: Is this very taxing? 
        // Recalculate all graphs
        AstarPath.active.Scan();

        // just for a good measure
        highlighter.ClearHighlightedTiles();

        // reset counts
        playerCharactersLeftToTakeTurn = playerCharactersAlive;
        enemyCharactersLeftToTakeTurn = enemyCharactersAlive;

        // reset enemy flags;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<EnemyCharMovementController>().reachedDestinationThisTurn = false;
            }
        }
    }

    public void OnEnemyDeath()
    {
        enemyCharactersAlive--;
        if (enemyCharactersAlive <= 0)
        {
            battleState = BattleState.WON;
            PlayerWon();
        }
    }

    public void OnPlayerCharDeath()
    {
        playerCharactersAlive--;
        if (playerCharactersAlive <= 0)
        {
            battleState = BattleState.LOST;
            PlayerLost();
        }
    }

    public void PlayerWon()
    {
        // TODO: this
        Debug.Log("Congratz player! You win!!!");
    }

    public void PlayerLost()
    {
        // TODO: this
        Debug.Log("Ugh... you lost!");
    }

    public void PlayerCharacterTurnFinished()
    {
        // -= player chars left. At 0 turn ends;
        playerCharactersLeftToTakeTurn -= 1;
        if (playerCharactersLeftToTakeTurn <= 0)
        {
            EndPlayerTurn();
        }
    }

    public void EnemyCharacterTurnFinished()
    {
        // -= player chars left. At 0 turn ends;
        enemyCharactersLeftToTakeTurn -= 1;
        if (enemyCharactersLeftToTakeTurn <= 0)
        {
            EndEnemyTurn();
        }
    }
}
