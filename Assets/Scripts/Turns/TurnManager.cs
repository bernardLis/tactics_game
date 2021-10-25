using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BattleState { PREPARATION, PLAYERTURN, ENEMYTURN, WON, LOST }

[RequireComponent(typeof(TurnDisplayer))]
public class TurnManager : MonoBehaviour
{
    public BattleState state;

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
        // TODO: create a start state where you place your characters
        state = BattleState.PREPARATION;

        


    }

    // TODO: this will be called when player places their characters and confirms that he wants to start the battle.
    public void StartBattle()
    {

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


    }


    public void EndPlayerTurn()
    {
        // display text & run enemy ai
        if (playerTurnEndEvent != null)
        {
            playerTurnEndEvent();
        }

        state = BattleState.ENEMYTURN;

        // TODO: Is this taxing?
        // Recalculate all graphs
        AstarPath.active.Scan();

        // reset counts
        playerCharactersLeftToTakeTurn = playerCharactersAlive;
        enemyCharactersLeftToTakeTurn = enemyCharactersAlive;
    }

    public void EndEnemyTurn()
    {
        // display text
        if (enemyTurnEndEvent != null)
        {
            enemyTurnEndEvent();
        }

        //yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;

        // TODO: Is this very taxing? 
        // Recalculate all graphs
        AstarPath.active.Scan();

        // just for a good measure
        Highlighter.instance.ClearHighlightedTiles();

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
            state = BattleState.WON;
            PlayerWon();
        }
    }

    public void OnPlayerCharDeath()
    {
        playerCharactersAlive--;
        if (playerCharactersAlive <= 0)
        {
            state = BattleState.LOST;
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
