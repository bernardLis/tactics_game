using UnityEngine;

public class DefendTriggerable : MonoBehaviour
{
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();

        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public bool Defend(GameObject target, int value, int manaCost)
    {
        Debug.Log("defend is called, target: " + target);
        // TODO: don't allow click if mana cost is less than the current mana 
        if (myStats.currentMana < manaCost)
            return false;

        // TODO: probably wrong, if you are defending empty space, use movepoint's transform to set the direction;
        if (target == null)
            target = MovePointController.instance.gameObject;
        // face the target
        Debug.Log(" target after null check: " + target);

        Vector2 dir = target.transform.position - transform.position;
        characterRendererManager.Face(dir);

        myStats.armor.AddModifier(value);
        myStats.UseMana(manaCost);

        return true;
    }
}
