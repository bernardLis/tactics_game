using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class CharacterCombat : MonoBehaviour
{
    CharacterStats myStats;

    void Start()
    {
        myStats = GetComponent<CharacterStats>();
    }

    public void Attack(CharacterStats targetStats)
    {
        // TODO: wrong. will be rewritten during the big enemy rewrite, need to pass ability to be resolved by stats
        targetStats.TakeDamage(myStats.strength.GetValue(), gameObject, myStats.basicAbilities[1]).GetAwaiter();
    }

}
