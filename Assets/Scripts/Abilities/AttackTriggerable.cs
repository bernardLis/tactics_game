using UnityEngine;

public class AttackTriggerable : MonoBehaviour
{
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();

        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public bool Attack(GameObject target, int value, int manaCost)
    {
        var attackableObject = target.GetComponent<IAttackable>();
        if (attackableObject == null)
            return false;

        // TODO: don't allow click if mana cost is less than the current mana 
        if (myStats.currentMana < manaCost)
            return false;

        // face the target
        Vector2 dir = target.transform.position - transform.position;
        characterRendererManager.Face(dir);

        int damage = value + myStats.strength.GetValue();
        attackableObject.TakeDamage(damage);
        myStats.UseMana(manaCost);

        return true;
    }
}
