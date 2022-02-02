using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    bool collected;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // TODO: so the problem is that you can trigger when placing a character
        if (!collected && other.CompareTag("PlayerCollider") && TurnManager.battleState == BattleState.PlayerTurn) 
            Collect();
    }

    void Collect()
    {
        collected = true;
        sr.color = Color.red;
        Debug.Log("Collected!");
    }



}
