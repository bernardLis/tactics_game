using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : MonoBehaviour
{
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
    public async Task<bool> Heal(GameObject target, int value, int manaCost)
    {
        Vector2 dir = target.transform.position - transform.position;
        // healing self, should be able to choose what direction to face
        if (target == gameObject)
            dir = await faceDirectionUI.PickDirection();

        // animation
        await characterRendererManager.SpellcastAnimation(dir);

        // data
        int healAmount = value + myStats.intelligence.GetValue();
        target.GetComponent<IHealable>().GainHealth(healAmount);
        myStats.UseMana(manaCost);

        return true;
    }
}
