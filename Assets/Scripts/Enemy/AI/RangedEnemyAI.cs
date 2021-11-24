using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RangedEnemyAI : EnemyAI
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator RunAI()
    {
        targetInRange = false;

        yield return StartCoroutine(base.RunAI());

        // TODO: this is a repetition from the base coroutine,
        // idk how to make base coroutine exit both coroutines.
        // exit if battle is over
        if (TurnManager.battleState == BattleState.Won || TurnManager.battleState == BattleState.Lost)
        {
            yield break;
        }

        // interact with target 
        if (!enemyCharMovementController.trappedOnTheWay)
        {
            bool newTargetInRange = false;
            // if your original target is not in range check if you can attack any other players 
            if (!targetInRange)
            {
                playerCharacters = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in playerCharacters)
                {
                    // check man distance
                    // TODO: don't hardcode abilities
                    int manDistance = Mathf.RoundToInt(Mathf.Abs(transform.position.x - player.transform.position.x)
                                                        + Mathf.Abs(transform.position.y - player.transform.position.y));
                    if (manDistance <= myStats.abilities[0].range)
                    {
                        targetCharacter = player;
                        newTargetInRange = true;
                        break;
                    }
                }
            }

            // attack! 
            if (targetInRange || newTargetInRange)
            {
                // TODO: don't hardcore abilities
                // TODO: should I make it all async?
#pragma warning disable CS4014
                myStats.abilities[0].HighlightTargetable();
                yield return new WaitForSeconds(0.5f);
                enemyInteractionController.Attack(targetCharacter);

                // clearing the highlight and finishing the turn
                yield return new WaitForSeconds(1f);
                // TODO: should I make it all async?
#pragma warning disable CS4014
                highlighter.ClearHighlightedTiles();
            }
            // or just face its direction
            else
            {
                enemyInteractionController.FaceAndFinishInteraction(targetCharacter);
            }
        }

        TurnManager.instance.EnemyCharacterTurnFinished();
        yield return true;
    }

    protected override GameObject GetTargetCharacter()
    {
        playerCharacters = GameObject.FindGameObjectsWithTag("Player");

        // defaults
        int lowestArmor = playerCharacters[0].GetComponent<CharacterStats>().armor.GetValue();
        targetCharacter = playerCharacters[0];

        // prioritize characters with low armor
        foreach (GameObject player in playerCharacters)
        {
            int armor = player.GetComponent<CharacterStats>().armor.GetValue();
            if (armor < lowestArmor)
            {
                targetCharacter = player;
                lowestArmor = armor;
            }
        }
        return targetCharacter;

    }

    protected override void GetDestination(GameObject target)
    {
        // TODO: make it smarter - ex. keep your distance from player characters

        // check how far is the target
        int distanceFromTarget = Mathf.RoundToInt(Mathf.Abs(transform.position.x - target.transform.position.x)
                                                            + Mathf.Abs(transform.position.y - target.transform.position.y));
        abilityRange = myStats.abilities[0].range;

        // if they are exactly in range
        if (distanceFromTarget == abilityRange)
        {
            targetInRange = true;
            enemyCharMovementController.GoToDestination(transform.position);
        }
        // you can move away from target
        else if (distanceFromTarget < abilityRange)
        {
            targetTile = null;
            targetInRange = true;

            int newDistanceFromTarget = distanceFromTarget;
            // maybe from tiles in range you can find a tile that furthest from the player 
            // but you could still attack from it  
            foreach (WorldTile tile in highlighter.highlightedTiles)
            {
                // I want to maximize manDist from target 
                int newDistance = Mathf.RoundToInt(Mathf.Abs(tile.LocalPlace.x - target.transform.position.x)
                                                    + Mathf.Abs(tile.LocalPlace.y - target.transform.position.y));
                // but I also need to stay in attack range
                if (newDistance > newDistanceFromTarget && newDistance <= abilityRange)
                {
                    newDistanceFromTarget = newDistance;
                    targetTile = tile;
                }
            }

            // if we finish without a better tile stay in your position;
            if (targetTile == null)
            {
                Vector3Int tilePos = tilemap.WorldToCell(transform.position);
                if (tiles.TryGetValue(tilePos, out _tile))
                {
                    targetTile = _tile;
                }
            }

            enemyCharMovementController.GoToDestination(new Vector3(targetTile.LocalPlace.x, targetTile.LocalPlace.y, targetTile.LocalPlace.z));
        }
        // you need to come closer to the target
        else
        {
            // find best tile calls seeker to pathfind;
            FindPath();
        }
    }

    void FindPath()
    {
        Path p = seeker.StartPath(transform.position, targetCharacter.transform.position, GoToBestTile);
    }

    void GoToBestTile(Path p)
    {
        Vector3Int tilePos;

        // ok, so we want to be as far as we can from the target, but still in range of the attack;
        // sooo, I am looking for a tile that is on the path but in max range achievable from the player...
        // We got our path back
        if (!p.error)
        {
            // Yay, now we can get a Vector3 representation of the path
            // from p.vectorPath
            // loop from the self to target
            for (int i = 0; i < p.vectorPath.Count; i++)
            {
                tilePos = tilemap.WorldToCell(p.vectorPath[i]);
                if (tiles.TryGetValue(tilePos, out _tile))
                {
                    // man dist between tile and player char
                    int distance = Mathf.RoundToInt(Mathf.Abs(_tile.LocalPlace.x - targetCharacter.transform.position.x)
                                                    + Mathf.Abs(_tile.LocalPlace.y - targetCharacter.transform.position.y));

                    // check if it is within reach and is not the tile I am currently standing on
                    // and is within attack range
                    if (_tile.WithinRange && _tile != currentTile && distance <= abilityRange)
                    {
                        targetInRange = true;
                        enemyCharMovementController.GoToDestination(new Vector3(_tile.LocalPlace.x, _tile.LocalPlace.y, _tile.LocalPlace.z));
                        return;
                    }
                }
            }

            // TODO: there is a possibility that target is further than attack range and walk range, then I should just move as close as I can;
            // right? 
            for (int i = p.vectorPath.Count - 1; i >= 0; i--)
            {
                tilePos = tilemap.WorldToCell(p.vectorPath[i]);
                if (tiles.TryGetValue(tilePos, out _tile))
                {
                    // check if it is within reach and is not the tile I am currently standing on
                    if (_tile.WithinRange && _tile != currentTile)
                    {
                        // return it's transform
                        enemyCharMovementController.GoToDestination(new Vector3(_tile.LocalPlace.x, _tile.LocalPlace.y, _tile.LocalPlace.z));
                        return;
                    }
                }
            }

            // no within range tile that is on path
            // selecting a random tile
            // TODO: something smarter
            WorldTile randomTile = highlighter.highlightedTiles[Random.Range(0, highlighter.highlightedTiles.Count)];
            enemyCharMovementController.GoToDestination(new Vector3(randomTile.LocalPlace.x, randomTile.LocalPlace.y, randomTile.LocalPlace.z));
        }
    }
}
