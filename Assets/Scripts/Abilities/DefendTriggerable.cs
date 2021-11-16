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
        // TODO: probably wrong, if you are defending empty space, use movepoint's transform to set the direction;  
        if (target == null)
            target = MovePointController.instance.gameObject;

        // play animation TODO: add defend animation
        Vector2 dir = target.transform.position - transform.position;
        characterRendererManager.SpellcastAnimation(dir);

        // add armor
        myStats.armor.AddModifier(value);
        myStats.UseMana(manaCost);

        return true;
    }
}
