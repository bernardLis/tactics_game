using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : MonoBehaviour
{
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    // returns true if successfully healed
    public async Task<bool> Heal(GameObject target, int value, int manaCost)
    {
        // animation
        Vector2 dir = target.transform.position - transform.position;
        await characterRendererManager.SpellcastAnimation(dir);

        // data
        int healAmount = value + myStats.intelligence.GetValue();
        target.GetComponent<IHealable>().GainHealth(healAmount);
        myStats.UseMana(manaCost);

        return true;
    }
}
