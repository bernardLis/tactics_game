using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Brain/Healer")]
public class HealerBrain : Brain
{
    GameObject tempObject;
    List<PotentialTarget> potentialTargets;
    public override void Move()
    {
        potentialTargets = GetPotentialTargets(characterGameObject.tag); // get guys from your team
        PotentialTarget selectedTarget = ChooseTarget(potentialTargets);
        target = selectedTarget.gObject; // for interaction
        Vector3 destinationPos;
        if (target == null)
            destinationPos = GetDestinationWithoutTarget(potentialTargets);
        else
            destinationPos = GetDestinationCloserTo(selectedTarget);

        highlighter.ClearHighlightedTiles().GetAwaiter();
        aiLerp.speed = 6f;

        tempObject = new GameObject("Enemy Destination");

        if (destinationPos != characterGameObject.transform.position)
            tempObject.transform.position = destinationPos;
        else
            tempObject.transform.position = characterGameObject.transform.position;

        highlighter.HighlightSingle(tempObject.transform.position, Helpers.GetColor("movementBlue"));
        destinationSetter.target = tempObject.transform;
    }

    public override async Task Interact()
    {
        // clean-up after movement
        if (tempObject != null)
            Destroy(tempObject);

        // TODO: check whether target is within range of our ability, if yes, heal him
        // otherwise heal someone else who is in range
        // or buff someone


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
        Ability selectedAbility = abilities[0]; // TODO: hardocded indexes.

        await selectedAbility.HighlightAreaOfEffect(target.transform.position);
        await Task.Delay(500);
        await selectedAbility.TriggerAbility(target);
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

}
