using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pathfinding;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Meele")]
public class MeeleBrain : Brain
{
    // Find a position where you can attack from the back
    public override async Task Move()
    {
        _potentialTargets = GetPotentialTargets("Player");
        AttackPosition attackPos = GetBestAttackPosition(_potentialTargets);
        // null if there are no good attack positions
        Vector3 destinationPos;
        if (attackPos == null)
        {
            destinationPos = GetDestinationWithoutTarget(_potentialTargets);
        }
        else
        {
            destinationPos = attackPos.Tile.GetMiddleOfTile();
            Target = attackPos.Target;
        }

        await _highlighter.ClearHighlightedTiles();
        _aiLerp.speed = 6f;

        _tempObject = new GameObject("Enemy Destination");
        _tempObject.transform.position = destinationPos;
        ABPath path = GetPathTo(_tempObject.transform);
        SetPath(path);

        await _highlighter.HighlightSingle(_tempObject.transform.position, Helpers.GetColor("movementBlue"));
    }

    // meele wants to attack anyone from the back
    AttackPosition GetBestAttackPosition(List<PotentialTarget> potentialTargets)
    {
        // TODO: does it make sense to get ALLL the tiles around ALLL the player characters and than work on that? 
        List<AttackPosition> allAvailableAttackPositions = new();
        foreach (PotentialTarget potentialTarget in potentialTargets)
        {
            List<AttackPosition> attackPositions = potentialTarget.GetMeeleAttackPositions(_characterGameObject);
            foreach (AttackPosition pos in attackPositions)
                allAvailableAttackPositions.Add(pos);
        }

        // in the side 1, face to face 2, from the back 0 - back is most preferential
        allAvailableAttackPositions = allAvailableAttackPositions.OrderBy(entry => entry.AttackDirection).ToList();

        // now I want to get the first tile that I can reach
        foreach (AttackPosition pos in allAvailableAttackPositions)
        {
            if (!pos.Tile.WithinRange) // kinda sucky, but it works way better than calculating path and comparing to movement range
                continue;

            return pos;
        }

        return null;
    }

    // chooses the ability that costs the most mana and executes (if there is a target)
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
        
        if (Target != null)
            ChooseAbility();
        else
            Defend();

        await base.Interact();
    }

    void ChooseAbility()
    {
        // best ability is the one that costs the most mana
        Ability bestAbility = _abilities[0];
        foreach (Ability a in _abilities)
        {
            if (a.ManaCost < bestAbility.ManaCost)
                continue;
            if (a.ManaCost > _enemyStats.CurrentMana)
                continue;

            bestAbility = a;
        }
        _selectedAbility = bestAbility;
    }

}
