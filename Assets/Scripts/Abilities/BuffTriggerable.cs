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

    public async Task<bool> Buff(GameObject _target, int _value, int _manaCost, GameObject _projectile, StatModifier _modifier)
    {
        if (_target == null)
            return false;

        // triggered only once if AOE
        if (!myStats.isAttacker)
        {
            // buffing self, should be able to choose what direction to face
            if (_target == gameObject)
            {
                Vector2 dir = await faceDirectionUI.PickDirection();

                // TODO: is that correct, facedir returns vector2.zero when it's broken out of
                if (dir == Vector2.zero)
                    return false;

                characterRendererManager.Face(dir.normalized);
            }

            await characterRendererManager.SpellcastAnimation();

            myStats.UseMana(_manaCost);
        }

        // adding stat modifiers
        List<Stat> stats = _target.GetComponent<CharacterStats>().stats;
        foreach (Stat s in stats)
            if (s.type == _modifier.statType)
                s.AddModifier(Instantiate(_modifier));

        return true;
    }
}
