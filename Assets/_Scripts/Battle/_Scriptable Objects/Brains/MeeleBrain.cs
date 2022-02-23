using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Meele")]
public class MeeleBrain : Brain
{
    public override void Move()
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

        // attack;
        _selectedAbility = _abilities[0]; // TODO: hardocded indexes.
        if (_enemyStats.currentMana >= 20)
            _selectedAbility = _abilities[1]; // TODO: hardocded indexes.

        await base.Interact();
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
}
