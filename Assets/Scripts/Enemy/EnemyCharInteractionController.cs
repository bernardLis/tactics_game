using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyCharInteractionController : CharacterInteractionController
{
    WorldTile targetTile;

    public void Attack(GameObject targetCharacter)
    {
        // get reference to camera follow
        // TODO: if it is projectile ability follow the projectile
        // also, is this a correct place to manage camera? 
        BasicCameraFollow.instance.followTarget = targetCharacter.transform;

        // highlight tile target char is standing on
        targetTile = highlighter.HighlightSingle(targetCharacter.transform.position, myStats.abilities[0].highlightColor);

        // TODO: select appropriate ability; for now it's only basic attack;
        selectedAbility = myStats.abilities[0];
        myStats.abilities[0].TriggerAbility(targetCharacter).GetAwaiter();
    }

    public void Heal(GameObject targetCharacter)
    {
        // get reference to camera follow
        // TODO: if it is projectile ability follow the projectile
        // also, is this a correct place to manage camera? 
        BasicCameraFollow.instance.followTarget = targetCharacter.transform;

        // only do that if target has less than full health
        // TODO: is this the correct place for this check? 
        CharacterStats targetStats = targetCharacter.GetComponent<CharacterStats>();
        if (targetStats.currentHealth < targetStats.maxHealth.GetValue())
        {
            // highlight tile target char is standing on
            targetTile = highlighter.HighlightSingle(targetCharacter.transform.position, myStats.abilities[1].highlightColor);

            // TODO: select appropriate ability; it's hardcoded now;
            selectedAbility = myStats.abilities[1];
            myStats.abilities[1].TriggerAbility(targetCharacter).GetAwaiter();
        }
        else
        {
            FaceAndFinishInteraction(targetCharacter);
        }
    }

    public void FaceAndFinishInteraction(GameObject targetCharacter)
    {
        Vector2 dir = Vector2.zero;
        if (targetCharacter != null)
        {
            dir = targetCharacter.transform.position - transform.position;
        }

        Face(dir);
    }
}
