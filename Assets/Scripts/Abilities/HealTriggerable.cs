using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : MonoBehaviour
{
    // local
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;
    
    FaceDirectionUI faceDirectionUI;

    bool hasPlayedAnimation;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        faceDirectionUI = GetComponent<FaceDirectionUI>();
    }

    // returns true if successfully healed
    public async Task<bool> Heal(GameObject target, int value, int manaCost)
    {
        // TODO: !hasPlayedAnimation kinda sucks
        if (!hasPlayedAnimation)
        {
            Vector2 dir = target.transform.position - transform.position;
            // healing self, should be able to choose what direction to face
            if (target == gameObject)
            {
                dir = await faceDirectionUI.PickDirection();
                // TODO: is that correct, facedir returns vector2.zero when it's broken out of
                if (dir == Vector2.zero)
                    return false;
            }

            // animation
            await characterRendererManager.SpellcastAnimation(dir);
            // reseted by char selection - this makes sure you play only one animation per attack - useful for aoe attacks
            if (myStats.isAttacker) // to not set this when you retaliate
                hasPlayedAnimation = true;
        }

        // data
        int healAmount = value + myStats.intelligence.GetValue();
        target.GetComponent<IHealable>().GainHealth(healAmount);
        myStats.UseMana(manaCost);

        return true;
    }

    public void SetHasPlayedAnimation(bool _has) { hasPlayedAnimation = _has; }

}
