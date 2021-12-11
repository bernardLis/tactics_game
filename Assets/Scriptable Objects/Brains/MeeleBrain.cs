using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Brain/Meele")]
public class MeeleBrain : Brain
{
    public override void Move()
    {
        List<PotentialTarget> d = GetPotentialTargets("Player");
        AttackPosition attackPos = GetAttackPosition(d);
        // 3. null if there are no good attack positions
        Vector3 destinationPos = attackPos.tile.GetMiddleOfTile();
        if (attackPos == null)
            destinationPos = GetDestinationCloserTo(d.FirstOrDefault());

        // now I can receive: 
        // 1. a tile that I am currently standing on
        // 2. new tile within reach of the target
        //GameObject target = //GetClosestPlayerCharacter();
        //Vector3 destination = GetDestination(target);
        // in the side 1, face to face 2, from the back 0, 

    }

    public override void Interact()
    {

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
