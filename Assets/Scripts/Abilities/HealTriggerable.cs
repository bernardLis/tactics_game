using UnityEngine;

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
    public bool Heal(GameObject target, int value, int manaCost)
    {
        var healableObject = target.GetComponent<IHealable>();
        if (healableObject == null)
            return false;

        // animation
        Vector2 dir = target.transform.position - transform.position;
        characterRendererManager.SpellcastAnimation(dir);

        // data
        int healAmount = value + myStats.intelligence.GetValue();
        healableObject.GainHealth(healAmount);
        myStats.UseMana(manaCost);

        return true;
    }
}
