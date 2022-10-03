using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinding;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Healer")]
public class HealerBrain : Brain
{
    public override async Task Move()
    {
        // healer looks for damaged friend with lowest health
        _potentialTargets = GetPotentialTargets(_characterGameObject.tag); // get guys from your team
        PotentialTarget selectedTarget = ChooseTarget(_potentialTargets);
        if (selectedTarget != null)
            Target = selectedTarget.GameObj; // for interaction

        // if there is no damaged friends, he will path to be next to his allies
        Vector3 destinationPos;
        if (Target == null)
            destinationPos = GetDestinationWithoutTarget(_potentialTargets);
        else
            destinationPos = GetDestinationCloserTo(selectedTarget);

        await _highlighter.ClearHighlightedTiles();
        _aiLerp.speed = 6f;

        _tempObject = new GameObject("Enemy Destination");
        _tempObject.transform.position = destinationPos;
        ABPath path = GetPathTo(_tempObject.transform);
        SetPath(path);

        await _highlighter.HighlightSingle(_tempObject.transform.position, Helpers.GetColor("movementBlue"));
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

    public override async Task Interact()
    {
        // clean-up after movement
        if (_tempObject != null)
            Destroy(_tempObject);

        SelectInteraction();

        Vector2 faceDir;
        if (Target == null || Target == _characterGameObject)
        {
            // facing the closest opponent
            PotentialTarget opponentToFace = GetClosestPotentialTargetWithTag(Tags.Player);
            faceDir = (opponentToFace.GameObj.transform.position -
                        _characterGameObject.transform.position).normalized;
        }
        else
            faceDir = (Target.transform.position - _characterGameObject.transform.position).normalized;

        // face 'stronger direction'
        faceDir = Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y) ? new Vector2(faceDir.x, 0f) : new Vector2(0f, faceDir.y);
        _characterRendererManager.Face(faceDir);

        // heal/buff
        await base.Interact();
    }


    void SelectInteraction()
    {
        if (CanHealSomeone())
            return;
        if (CanBuffSomeone())
            return;

        DoNothing();
    }

    bool CanHealSomeone()
    {
        // so, you have moved closer to lowest health boi, but you are not sure whether you can reach him
        // for all heal abilities, check if there is one that can reach someone harmed, starting with the most costly one
        List<Ability> healAbilities = Abilities.Where(a => a.AbilityType == AbilityType.Heal).ToList();
        healAbilities.Sort((p1, p2) => p2.ManaCost.CompareTo(p1.ManaCost)); //order by mana cost (https://i.redd.it/iuy9fxt300811.png)
        healAbilities.RemoveAll(x => x.ManaCost > _enemyStats.CurrentMana);// remove ones that you don't have mana for

        if (healAbilities.Count == 0)
            return false;

        PotentialTarget pTarget = null;
        _selectedAbility = null;
        foreach (Ability a in healAbilities)
        {
            pTarget = GetWithinReachHealableTarget(_potentialTargets, a);
            if (pTarget != null)
            {
                Target = pTarget.GameObj;
                _selectedAbility = a;
                return true;
            }
        }

        return false;
    }

    bool CanBuffSomeone()
    {
        List<Ability> buffAbilities = Abilities.Where(a => a.AbilityType == AbilityType.Buff).ToList();
        buffAbilities.Sort((p1, p2) => p2.ManaCost.CompareTo(p1.ManaCost)); //order by mana cost (https://i.redd.it/iuy9fxt300811.png)
        buffAbilities.RemoveAll(x => x.ManaCost > _enemyStats.CurrentMana);// remove ones that you don't have mana for
        buffAbilities.RemoveAll(x => x.Id == "5f7d8c47-7ec1-4abf-b8ec-74ea82be327f");// remove defend, it will be reworked TODO: end turn rework

        if (buffAbilities.Count == 0)
            return false;

        PotentialTarget pTarget = null;
        _selectedAbility = null;
        foreach (Ability a in buffAbilities)
        {
            pTarget = GetWithinReachBuffableTargets(_potentialTargets, a);
            if (pTarget != null)
            {
                Target = pTarget.GameObj;
                _selectedAbility = a;
                return true;
            }
        }
        return false;
    }

    PotentialTarget GetWithinReachHealableTarget(List<PotentialTarget> potentialTargets, Ability selectedAbility)
    {
        // actually, get the lowest health reachable target
        PotentialTarget target = null;
        int lowestHealth = int.MaxValue;
        foreach (PotentialTarget t in potentialTargets)
        {
            if (t.GameObj == null)
                continue;

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

    PotentialTarget GetWithinReachBuffableTargets(List<PotentialTarget> potentialTargets, Ability selectedAbility)
    {
        List<PotentialTarget> buffableTargets = new();
        foreach (PotentialTarget t in potentialTargets)
            if (Helpers.GetManhattanDistance(_characterGameObject.transform.position, t.GameObj.transform.position) < selectedAbility.Range)
                buffableTargets.Add(t);

        return buffableTargets[Random.Range(0, buffableTargets.Count - 1)];
    }
}
