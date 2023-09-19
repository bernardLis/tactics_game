using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbility : MonoBehaviour
{

    [SerializeField] protected Ability _ability; // HERE: for now
    protected IEnumerator _runAbilityCoroutine;
    protected IEnumerator _fireAbilityCoroutine;

    // HERE: for now
    void Start()
    {
        Initialize(_ability);
    }
    
    public void Initialize(Ability ability)
    {
        _ability = ability;

        _runAbilityCoroutine = RunAbilityCoroutine();
        StartCoroutine(_runAbilityCoroutine);
    }

    IEnumerator RunAbilityCoroutine()
    {
        while (true)
        {
            _fireAbilityCoroutine = FireAbilityCoroutine();
            StartCoroutine(_fireAbilityCoroutine);

            yield return new WaitForSeconds(_ability.GetCooldown());
        }

    }

    protected virtual IEnumerator FireAbilityCoroutine()
    {
        // override this method in child classes
        yield return null;
    }

}
