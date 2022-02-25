using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Ranged")]
public class RangedBrain : Brain
{
    public override void Move()
    {
        _potentialTargets = GetPotentialTargets("Player");
        // attack;
        _selectedAbility = _abilities[0]; // TODO: hardocded indexes.
        if (_enemyStats.currentMana >= 20)
            _selectedAbility = _abilities[1]; // TODO: hardocded indexes.

        // ranged brain wants to keep distance but to be in the range of his attack
        PotentialTarget selectedTarget = GetClosestTarget(_potentialTargets, _selectedAbility);
        Vector3 myPos = _characterGameObject.transform.position;
        Vector3 targetPos = selectedTarget.GameObj.transform.position;
        Vector3 destinationPos = Vector3.zero;
        // 1. you cannot reach the target with you attack range
        if (Helpers.GetManhattanDistance(myPos, targetPos) > _selectedAbility.Range)
        {
            // you want to move closer to the target and not attack
            Debug.Log("target not within attack range");
            destinationPos = GetDestinationCloserTo(selectedTarget);
            Target = null;
        }

        // 2. the target is perfectly at the edge of your attack range
        if (Helpers.GetManhattanDistance(myPos, targetPos) == _selectedAbility.Range)
        {
            Debug.Log("target perfectly where we want them");
            destinationPos = myPos;
            Target = selectedTarget.GameObj;

        }
        // = you want to keep position and attack

        // 3. the target is closer than you would like
        if (Helpers.GetManhattanDistance(myPos, targetPos) < _selectedAbility.Range)
        {
            Debug.Log("target is too close I want to move away");
            // = you want to path the furthest you can from the target but still within attack range
            destinationPos = FindArcherPosition(selectedTarget, _selectedAbility);
            Target = selectedTarget.GameObj;

            if (destinationPos == Vector3.zero)
                destinationPos = _characterGameObject.transform.position;
        }


        _highlighter.ClearHighlightedTiles().GetAwaiter();
        _aiLerp.speed = 6f;

        _tempObject = new GameObject("Enemy Destination");
        _tempObject.transform.position = destinationPos;

        _highlighter.HighlightSingle(_tempObject.transform.position, Helpers.GetColor("movementBlue"));
        _destinationSetter.target = _tempObject.transform;
    }

    public override async Task Interact()
    {
        // clean-up after movement
        if (_tempObject != null)
            Destroy(_tempObject);

        Vector2 faceDir;
        if (Target == null)
            faceDir = (_potentialTargets[0].GameObj.transform.position - _characterGameObject.transform.position).normalized;
        else
            faceDir = (Target.transform.position - _characterGameObject.transform.position).normalized;

        // face 'stronger direction'
        faceDir = Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y) ? new Vector2(faceDir.x, 0f) : new Vector2(0f, faceDir.y);

        _characterRendererManager.Face(faceDir);
        if (Target == null)
            return;

        await base.Interact();
    }

    PotentialTarget GetClosestTarget(List<PotentialTarget> potentialTargets, Ability ability)
    {
        List<PotentialTarget> pTargets = new();
        pTargets = potentialTargets.OrderBy(entry => entry.DistanceToTarget).ToList();
        return pTargets[0];
    }

    Vector3 FindArcherPosition(PotentialTarget target, Ability ability)
    {
        Vector3 targetPos = target.GameObj.transform.position;
        float maxDist = target.DistanceToTarget;
        Vector3 bestDest = Vector3.zero;
        // going through all within reach tiles
        foreach (WorldTile tile in _highlighter.highlightedTiles)
        {
            float hypotheticalDistance = Helpers.GetManhattanDistance(tile.GetMiddleOfTile(), targetPos);

            // must be within attack range
            if (hypotheticalDistance > ability.Range)
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
