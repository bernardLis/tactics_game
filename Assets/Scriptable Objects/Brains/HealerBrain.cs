using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Brain/Healer")]
public class HealerBrain : Brain
{
    public override void Move()
    {
        potentialTargets = GetPotentialTargets(characterGameObject.tag); // get guys from your team
        PotentialTarget selectedTarget = ChooseTarget(potentialTargets);
        if (selectedTarget != null)
            target = selectedTarget.gObject; // for interaction

        Vector3 destinationPos;
        if (target == null)
            destinationPos = GetDestinationWithoutTarget(potentialTargets);
        else
            destinationPos = GetDestinationCloserTo(selectedTarget);

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

        // so, you have moved closer to lowest health boi, but you are not sure whether you can reach him
        selectedAbility = abilities[0]; // this is heal // TODO: hardocded indexes.
        // target does not exist or you cannot reach him 
        if (target == null || Helpers.GetManhattanDistance(characterGameObject.transform.position, target.transform.position) > selectedAbility.range)
        {
            // select within reach target that has lower hp than max
            // if noone within reach or everyone within reach has full health buff someone
            potentialTargets = GetPotentialTargets(characterGameObject.tag); // get guys from your team

            PotentialTarget pTarget = GetWithinReachHealableTarget(potentialTargets, selectedAbility);
            if (pTarget != null)
                target = pTarget.gObject;
            else
                target = null;
        }

        // there is no within reach healable targets
        if (target == null)
        {
            // buff someone
            selectedAbility = abilities[1]; // this is buff // TODO: hardocded indexes.
            List<PotentialTarget> buffableTargets = GetWithinReachBuffableTargets(potentialTargets, selectedAbility);
            // it will always return someone, because you are within reach
            target = buffableTargets[Random.Range(0, buffableTargets.Count)].gObject;
        }

        Vector2 faceDir;
        if (target == null || target == characterGameObject)
            faceDir = (potentialTargets[0].gObject.transform.position - characterGameObject.transform.position).normalized;
        else
            faceDir = (target.transform.position - characterGameObject.transform.position).normalized;

        // face 'stronger direction'
        faceDir = Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y) ? new Vector2(faceDir.x, 0f) : new Vector2(0f, faceDir.y);
        characterRendererManager.Face(faceDir);

        if (target == null)
            return;

        // heal/buff
        await base.Interact();
    }

    PotentialTarget ChooseTarget(List<PotentialTarget> _potentialTargets)
    {
        // looking for the lowest health boi who is damaged
        int lowestHealth = int.MaxValue;
        PotentialTarget target = null;
        foreach (PotentialTarget t in _potentialTargets)
        {
            CharacterStats stats = t.gObject.GetComponent<CharacterStats>();
            if (stats.currentHealth < stats.maxHealth.GetValue() && stats.currentHealth < lowestHealth)
            {
                lowestHealth = stats.currentHealth;
                target = t;
            }
        }

        return target;
    }


    PotentialTarget GetWithinReachHealableTarget(List<PotentialTarget> _potentialTargets, Ability _selectedAbility)
    {
        // actually, get the lowest health reachable target
        PotentialTarget target = null;
        int lowestHealth = int.MaxValue;
        foreach (PotentialTarget t in _potentialTargets)
        {
            CharacterStats stats = t.gObject.GetComponent<CharacterStats>();

            if (stats.currentHealth < stats.maxHealth.GetValue()
                && Helpers.GetManhattanDistance(characterGameObject.transform.position, t.gObject.transform.position) < _selectedAbility.range
                && stats.currentHealth < lowestHealth)
            {
                lowestHealth = stats.currentHealth;
                target = t;
            }
        }
        return target;
    }

    List<PotentialTarget> GetWithinReachBuffableTargets(List<PotentialTarget> _potentialTargets, Ability _selectedAbility)
    {

        List<PotentialTarget> buffableTargets = new();
        foreach (PotentialTarget t in _potentialTargets)
            if (Helpers.GetManhattanDistance(characterGameObject.transform.position, t.gObject.transform.position) < _selectedAbility.range)
                buffableTargets.Add(t);
        return buffableTargets;
    }
}
