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
#pragma warning disable CS4014
        targetStats.TakeDamage(myStats.strength.GetValue(), gameObject);
#pragma warning restore CS4014

    }

}
