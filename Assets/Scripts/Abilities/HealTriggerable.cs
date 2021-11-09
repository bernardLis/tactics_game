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

        // TODO: don't allow click if mana cost is less than the current mana 
        if(myStats.currentMana < manaCost)
            return false;
        
        // face the target
        Vector2 dir = target.transform.position - transform.position;
        characterRendererManager.Face(dir);

        int healAmount = value + myStats.intelligence.GetValue();
        healableObject.GainHealth(healAmount);
        myStats.UseMana(manaCost);

        return true;
    }
}
