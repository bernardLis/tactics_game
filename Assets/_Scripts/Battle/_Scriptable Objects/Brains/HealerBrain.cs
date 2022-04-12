using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinding;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Healer")]
public class HealerBrain : Brain
{
    public override void Move()
    {
        _potentialTargets = GetPotentialTargets(_characterGameObject.tag); // get guys from your team
        PotentialTarget selectedTarget = ChooseTarget(_potentialTargets);
        if (selectedTarget != null)
            Target = selectedTarget.GameObj; // for interaction

        Vector3 destinationPos;
        if (Target == null)
            destinationPos = GetDestinationWithoutTarget(_potentialTargets);
        else
            destinationPos = GetDestinationCloserTo(selectedTarget);

        _highlighter.ClearHighlightedTiles().GetAwaiter();
        _aiLerp.speed = 6f;

        _tempObject = new GameObject("Enemy Destination");
        _tempObject.transform.position = destinationPos;
        ABPath path = GetPathTo(_tempObject.transform);
        SetPath(path);

        _highlighter.HighlightSingle(_tempObject.transform.position, Helpers.GetColor("movementBlue"));
    }

    public override async Task Interact()
    {
        // clean-up after movement
        if (_tempObject != null)
            Destroy(_tempObject);

        // so, you have moved closer to lowest health boi, but you are not sure whether you can reach him
        _selectedAbility = _abilities[0]; // this is heal // TODO: hardocded indexes.
        // target does not exist or you cannot reach him 
        if (Target == null || Helpers.GetManhattanDistance(_characterGameObject.transform.position, Target.transform.position) > _selectedAbility.Range)
        {
            // select within reach target that has lower hp than max
            // if noone within reach or everyone within reach has full health buff someone
            _potentialTargets = GetPotentialTargets(_characterGameObject.tag); // get guys from your team

            PotentialTarget pTarget = GetWithinReachHealableTarget(_potentialTargets, _selectedAbility);
            if (pTarget != null)
                Target = pTarget.GameObj;
            else
                Target = null;
        }

        // there is no within reach healable targets
        if (Target == null)
        {
            // buff someone
            _selectedAbility = _abilities[1]; // this is buff // TODO: hardocded indexes.
            List<PotentialTarget> buffableTargets = GetWithinReachBuffableTargets(_potentialTargets, _selectedAbility);
            // it will always return someone, because you are within reach
            Target = buffableTargets[Random.Range(0, buffableTargets.Count)].GameObj;
        }

        Vector2 faceDir;
        if (Target == null || Target == _characterGameObject)
            faceDir = (_potentialTargets[0].GameObj.transform.position - _characterGameObject.transform.position).normalized;
        else
            faceDir = (Target.transform.position - _characterGameObject.transform.position).normalized;

        // face 'stronger direction'
        faceDir = Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y) ? new Vector2(faceDir.x, 0f) : new Vector2(0f, faceDir.y);
        _characterRendererManager.Face(faceDir);

        if (Target == null)
            return;

        // heal/buff
        await base.Interact();
    }

    PotentialTarget ChooseTarget(List<PotentialTarget> potentialTargets)
    {
        // looking for the lowest health boi who is damaged
        int lowestHealth = int.MaxValue;
        PotentialTarget target = null;
        foreach (PotentialTarget t in potentialTargets)
        {
            CharacterStats stats = t.GameObj.GetComponent<CharacterStats>();
            if (stats.CurrentHealth < stats.MaxHealth.GetValue() && stats.CurrentHealth < lowestHealth)
            {
                lowestHealth = stats.CurrentHealth;
                target = t;
            }
        }

        return target;
    }


    PotentialTarget GetWithinReachHealableTarget(List<PotentialTarget> potentialTargets, Ability selectedAbility)
    {
        // actually, get the lowest health reachable target
        PotentialTarget target = null;
        int lowestHealth = int.MaxValue;
        foreach (PotentialTarget t in potentialTargets)
        {
            CharacterStats stats = t.GameObj.GetComponent<CharacterStats>();

            if (stats.CurrentHealth < stats.MaxHealth.GetValue()
                && Helpers.GetManhattanDistance(_characterGameObject.transform.position, t.GameObj.transform.position) < selectedAbility.Range
                && stats.CurrentHealth < lowestHealth)
            {
                lowestHealth = stats.CurrentHealth;
                target = t;
            }
        }
        return target;
    }

    List<PotentialTarget> GetWithinReachBuffableTargets(List<PotentialTarget> potentialTargets, Ability selectedAbility)
    {

        List<PotentialTarget> buffableTargets = new();
        foreach (PotentialTarget t in potentialTargets)
            if (Helpers.GetManhattanDistance(_characterGameObject.transform.position, t.GameObj.transform.position) < selectedAbility.Range)
                buffableTargets.Add(t);
        return buffableTargets;
    }
}
