using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Ranged")]
public class RangedBrain : Brain
{
    public Weapon[] bows;

    public override void Initialize(GameObject _self)
    {
        base.Initialize(_self);
    }
    public override void Move()
    {
        potentialTargets = GetPotentialTargets("Player");
        // attack;
        selectedAbility = abilities[0]; // TODO: hardocded indexes.
        if (enemyStats.currentMana >= 20)
            selectedAbility = abilities[1]; // TODO: hardocded indexes.

        // ranged brain wants to keep distance but to be in the range of his attack
        PotentialTarget selectedTarget = GetClosestTarget(potentialTargets, selectedAbility);
        Vector3 myPos = characterGameObject.transform.position;
        Vector3 targetPos = selectedTarget.gObject.transform.position;
        Vector3 destinationPos = Vector3.zero;
        // 1. you cannot reach the target with you attack range
        if (Helpers.GetManhattanDistance(myPos, targetPos) > selectedAbility.range)
        {
            // you want to move closer to the target and not attack
            Debug.Log("target not within attack range");
            destinationPos = GetDestinationCloserTo(selectedTarget);
            target = null;
        }

        // 2. the target is perfectly at the edge of your attack range
        if (Helpers.GetManhattanDistance(myPos, targetPos) == selectedAbility.range)
        {
            Debug.Log("target perfectly where we want them");
            destinationPos = myPos;
            target = selectedTarget.gObject;

        }
        // = you want to keep position and attack

        // 3. the target is closer than you would like
        if (Helpers.GetManhattanDistance(myPos, targetPos) < selectedAbility.range)
        {
            Debug.Log("target is too close I want to move away");
            // = you want to path the furthest you can from the target but still within attack range
            destinationPos = FindArcherPosition(selectedTarget, selectedAbility);
            target = selectedTarget.gObject;

            if (destinationPos == Vector3.zero)
                destinationPos = characterGameObject.transform.position;
        }


        highlighter.ClearHighlightedTiles().GetAwaiter();
        aiLerp.speed = 6f;

        tempObject = new GameObject("Enemy Destination");
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

        await base.Interact();
    }

    PotentialTarget GetClosestTarget(List<PotentialTarget> _potentialTargets, Ability _ability)
    {
        List<PotentialTarget> pTargets = new();
        pTargets = _potentialTargets.OrderBy(entry => entry.distanceToTarget).ToList();
        return pTargets[0];
    }

    Vector3 FindArcherPosition(PotentialTarget _target, Ability _ability)
    {
        Vector3 targetPos = _target.gObject.transform.position;
        float maxDist = _target.distanceToTarget;
        Vector3 bestDest = Vector3.zero;
        // going through all within reach tiles
        foreach (WorldTile tile in highlighter.highlightedTiles)
        {
            float hypotheticalDistance = Helpers.GetManhattanDistance(tile.GetMiddleOfTile(), targetPos);

            // must be within attack range
            if (hypotheticalDistance > _ability.range)
                continue;

            // and checking whether they are further from target than current positon
            if (hypotheticalDistance > maxDist)
            {
                maxDist = hypotheticalDistance;
                bestDest = tile.GetMiddleOfTile();
            }
        }
        return bestDest;
    }



}
