using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BuffTriggerable : MonoBehaviour
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

    public async Task<bool> Buff(GameObject _target, Ability _ability, GameObject _attacker)
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

            myStats.UseMana(_ability.manaCost);
        }

        _target.GetComponent<IBuffable<Ability>>().GetBuffed(_ability);

        return true;
    }

    //TODO: repetition between heal and buff triggerables
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
