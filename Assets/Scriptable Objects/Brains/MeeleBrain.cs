using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Brain/Meele")]
public class MeeleBrain : Brain
{
    GameObject target;
    GameObject tempObject;
    List<PotentialTarget> potentialTargets;
    public override void Move()
    {
        potentialTargets = GetPotentialTargets("Player");
        AttackPosition attackPos = GetAttackPosition(potentialTargets);
        // 3. null if there are no good attack positions
        Vector3 destinationPos;
        if (attackPos == null)
        {
            destinationPos = GetDestinationCloserTo(potentialTargets.FirstOrDefault());
        }
        else
        {
            destinationPos = attackPos.tile.GetMiddleOfTile();
            target = attackPos.target;
        }

        // go there!
        highlighter.ClearHighlightedTiles().GetAwaiter();

        // reset ailerp speed
        aiLerp.speed = 3f;

        // move if destination is not the same as current position
        if (destinationPos != characterGameObject.transform.position)
        {
            tempObject = new GameObject("Enemy Destination");
            // I want it to be in the center of the tile, not on the edge.
            tempObject.transform.position = destinationPos;

            highlighter.HighlightSingle(tempObject.transform.position, new Color(0.53f, 0.52f, 1f, 1f));

            // move
            destinationSetter.target = tempObject.transform;
            aiLerp.canSearch = true;
            aiLerp.canMove = true;
        }

        // now I can receive: 
        // 1. a tile that I am currently standing on
        // 2. new tile within reach of the target
        // 3. tile that's closer to target
        //GameObject target = //GetClosestPlayerCharacter();
        //Vector3 destination = GetDestination(target);
        // in the side 1, face to face 2, from the back 0, 

    }

    public override void Interact()
    {
        Debug.Log("intarct is called");
        // clean-up after movement
        if (tempObject != null)
            Destroy(tempObject);

        Vector2 faceDir;
        if (target == null)
            faceDir = (potentialTargets[0].gObject.transform.position - characterGameObject.transform.position).normalized;
        else
            faceDir = (target.transform.position - characterGameObject.transform.position).normalized;
    
        // face 'stronger direction'
        faceDir = Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y) ? new Vector2(faceDir.x, 0f) : new Vector2(0f, faceDir.y);

        Debug.Log("faceDir " + faceDir);
        characterRendererManager.Face(faceDir);

        if (target == null)
            return;

        // attack;
        // highlight tile target char is standing on
        highlighter.HighlightSingle(target.transform.position, enemyStats.abilities[0].highlightColor);

        // TODO: select appropriate ability; for now it's only basic attack;
        enemyStats.abilities[0].TriggerAbility(target).GetAwaiter();
    }

    // meele wants to attack anyone from the back
    // this means that they need a reachable  
    AttackPosition GetAttackPosition(List<PotentialTarget> _potentialTargets)
    {
        // TODO: does it make sense to get ALLL the tiles around ALLL the player characters and than work on that? 
        List<AttackPosition> allAvailableAttackPositions = new();
        foreach (PotentialTarget potentialTarget in _potentialTargets)
        {
            List<AttackPosition> attackPositions = potentialTarget.GetAttackPositions(characterGameObject);
            foreach (AttackPosition pos in attackPositions)
                allAvailableAttackPositions.Add(pos);
        }

        // in the side 1, face to face 2, from the back 0 - back is most preferential
        allAvailableAttackPositions = allAvailableAttackPositions.OrderBy(entry => entry.attackDirection).ToList();

        // now I want to get the first tile that I can reach
        foreach (AttackPosition pos in allAvailableAttackPositions)
        {
            if (!pos.tile.WithinRange) // kinda sucky, but it works way better than calculating path and comparing to movement range
                continue;

            return pos;
        }

        return null;
    }
}
