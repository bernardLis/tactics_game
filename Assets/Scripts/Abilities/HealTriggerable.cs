using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : MonoBehaviour
{
    // local
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;
    FaceDirectionUI faceDirectionUI;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        faceDirectionUI = GetComponent<FaceDirectionUI>();
    }

    // returns true if successfully healed
    public async Task<bool> Heal(GameObject _target, int _value, int _manaCost)
    {
        // triggered only once if AOE
        if (!myStats.isAttacker)
        {
            // healing self, should be able to choose what direction to face
            if (_target == gameObject)
            {
                Vector2 dir = await faceDirectionUI.PickDirection();

                // TODO: is that correct, facedir returns vector2.zero when it's broken out of
                if (dir == Vector2.zero)
                    return false;

                characterRendererManager.Face(dir.normalized);
            }

            // animation
            await characterRendererManager.SpellcastAnimation();

            myStats.UseMana(_manaCost);
        }

        // data
        int healAmount = _value + myStats.intelligence.GetValue();
        _target.GetComponent<IHealable>().GainHealth(healAmount);

        myStats.SetAttacker(true);

        return true;
    }
}
