using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UtilityTriggerable : MonoBehaviour
{
    public Transform projectileSpawnPoint; // TODO: is that ok way to handle this?

    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;
    FaceDirectionUI faceDirectionUI;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        faceDirectionUI = GetComponent<FaceDirectionUI>();
    }

    public async Task<bool> TriggerUtility(GameObject _target, UtilityAbility _ability, GameObject _attacker)
    {
        if (_target == null)
            return false;

        // triggered only once if AOE
        if (!myStats.isAttacker)
        {
            // buffing self, should be able to choose what direction to face
            if (_target == gameObject && _attacker.CompareTag("Player"))
            {
                if (!await PlayerFaceDirSelection()) // allows to break out from selecing face direction
                    return false;
            }
            await characterRendererManager.SpellcastAnimation();

            myStats.UseMana(_ability.ManaCost);
        }
        
        // there is item usable, it is being checked in ability;
        _target.GetComponent<IItemUsable<UtilityAbility>>().UseItem(_ability);

        return true;
    }

    //TODO: repetition between heal and buff and item triggerables
    async Task<bool> PlayerFaceDirSelection()
    {
        Vector2 dir = Vector2.zero;
        if (faceDirectionUI != null)
            dir = await faceDirectionUI.PickDirection();

        // TODO: is that correct, facedir returns vector2.zero when it's broken out of
        if (dir == Vector2.zero)
            return false;

        characterRendererManager.Face(dir.normalized);

        return true;
    }

}
