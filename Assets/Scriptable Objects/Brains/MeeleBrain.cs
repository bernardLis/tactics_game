using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Brain/Meele")]
public class MeeleBrain : Brain
{
    GameObject tempObject;
    List<PotentialTarget> potentialTargets;
    public override void Move()
    {
        potentialTargets = GetPotentialTargets("Player");
        AttackPosition attackPos = GetBestAttackPosition(potentialTargets);
        // 3. null if there are no good attack positions
        Vector3 destinationPos;
        if (attackPos == null)
        {
            destinationPos = GetDestinationCloserTo(potentialTargets.FirstOrDefault());
            // get a random tile if there are no good tiles on path
            if (destinationPos == Vector3.zero)
                destinationPos = highlighter.highlightedTiles[Random.Range(0, highlighter.highlightedTiles.Count)].GetMiddleOfTile();
        }
        else
        {
            destinationPos = attackPos.tile.GetMiddleOfTile();
            target = attackPos.target;
        }

        highlighter.ClearHighlightedTiles().GetAwaiter();
        aiLerp.speed = 6f;

        tempObject = new GameObject("Enemy Destination");

        if (destinationPos == characterGameObject.transform.position)
        {
            tempObject.transform.position = characterGameObject.transform.position;
            destinationSetter.target = tempObject.transform;
            return;
        }

        tempObject.transform.position = destinationPos;
        highlighter.HighlightSingle(tempObject.transform.position, Helpers.GetColor("movementBlue"));
        destinationSetter.target = tempObject.transform;
    }

    public override async Task Interact()
    {
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

        characterRendererManager.Face(faceDir);

        if (target == null)
            return;

        // attack;
        // highlight tile target char is standing on
        highlighter.HighlightSingle(target.transform.position, enemyStats.abilities[0].highlightColor);

        // TODO: select appropriate ability; for now it's only basic attack;
        await enemyStats.abilities[0].TriggerAbility(target);
    }

    // meele wants to attack anyone from the back
    // this means that they need a reachable  
    AttackPosition GetBestAttackPosition(List<PotentialTarget> _potentialTargets)
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
