using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pathfinding;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Ranged")]
public class RangedBrain : Brain
{
    public override async Task Move()
    {
        _potentialTargets = GetPotentialTargets("Player");
        foreach (PotentialTarget t in _potentialTargets)
        {
            Debug.Log($"distance to {t.GameObj.name}: {t.DistanceToTarget}");
        }

        PotentialTarget selectedTarget = GetClosestTarget(_potentialTargets, _selectedAbility);
        Debug.Log($"selectedTarget: {selectedTarget.GameObj.name}");
        Ability bestAbility = Abilities[0];
        foreach (Ability a in Abilities)
        {
            if (a.ManaCost < bestAbility.ManaCost)
                continue;
            if (a.ManaCost > _enemyStats.CurrentMana)
                continue;
            if (a.AbilityType == AbilityType.Buff)
                continue;
            if (!IsTargetWithinAttackRange(a, selectedTarget.GameObj.transform))
                continue;
            bestAbility = a;
        }

        _selectedAbility = bestAbility;

        // ranged brain wants to keep distance but to be in the range of his attack
        Vector3 destinationPos = ChoosePosition(selectedTarget);
        if (destinationPos == Vector3.zero)
            destinationPos = _characterGameObject.transform.position;

        await _highlighter.ClearHighlightedTiles();
        _aiLerp.speed = 6f;

        _tempObject = new GameObject("Enemy Destination");
        _tempObject.transform.position = destinationPos;
        ABPath p = GetPathTo(_tempObject.transform);
        SetPath(p);

        await _highlighter.HighlightSingle(_tempObject.transform.position, Helpers.GetColor("movementBlue"));
    }

    Vector3 ChoosePosition(PotentialTarget selectedTarget)
    {
        Vector3 myPos = _characterGameObject.transform.position;
        Vector3 targetPos = selectedTarget.GameObj.transform.position;
        Target = selectedTarget.GameObj;

        // 1. the target is perfectly at the edge of your attack range
        // = you want to keep the position and attack
        if (Helpers.GetManhattanDistance(myPos, targetPos) == _selectedAbility.Range)
            return myPos;

        // 2. the target is closer than you would like
        // = you want to path the furthest you can from the target but still within attack range
        if (Helpers.GetManhattanDistance(myPos, targetPos) < _selectedAbility.Range)
            return FindArcherPosition(selectedTarget, _selectedAbility, false);

        // 3. if you can reach the target by repositioning 
        // you want to path the furthest you can from the target but still within attack range
        if (IsTargetWithinAttackRange(_selectedAbility, selectedTarget.GameObj.transform))
            return FindArcherPosition(selectedTarget, _selectedAbility, true);

        // 4. you cannot reach the target with walking + attack range 
        // you want to move closer to the target and not attack
        Target = null;
        return GetDestinationCloserTo(selectedTarget);
    }

    bool IsTargetWithinAttackRange(Ability ability, Transform target)
    {
        Vector3 myPos = _characterGameObject.transform.position;
        // you don't even have to move!  
        if (Helpers.GetManhattanDistance(myPos, target.position) <= ability.Range)
            return true;
        // when you cannot reach it like that you will need to move, so you will need to path 
        // and it can bring you closer to your target, but it can also bring you further...
        ABPath p = GetPathTo(target);
        // target unreachable
        if (p.vectorPath.Count == 0)
            return false;

        // this far you can go on the path to opponent 
        int reachableVectorIndex = _enemyStats.MovementRange.GetValue();
        if (_enemyStats.MovementRange.GetValue() >= p.vectorPath.Count)
            reachableVectorIndex = p.vectorPath.Count - 1;

        Vector3 furthestPosOnPath = p.vectorPath[reachableVectorIndex];
        if (Helpers.GetManhattanDistance(furthestPosOnPath, target.position) <= ability.Range)
            return true;

        return false;
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
            Defend();

        await base.Interact();
    }

    PotentialTarget GetClosestTarget(List<PotentialTarget> potentialTargets, Ability ability)
    {
        Debug.Log($"potentialTargets.Count {potentialTargets.Count}");
        List<PotentialTarget> pTargets = new();
        pTargets = potentialTargets.OrderBy(entry => entry.DistanceToTarget).ToList();
        return pTargets[0];
    }

    Vector3 FindArcherPosition(PotentialTarget target, Ability ability, bool closer)
    {
        Vector3 targetPos = target.GameObj.transform.position;
        float maxDist = target.DistanceToTarget;
        if (closer) // for finding position closer to target, but still the furthest you can be.
            maxDist = 0;
        Vector3 bestDest = _characterGameObject.transform.position;
        // going through all within reach tiles
        foreach (WorldTile tile in _highlighter.HighlightedTiles)
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
