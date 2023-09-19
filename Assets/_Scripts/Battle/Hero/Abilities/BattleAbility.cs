using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbility : MonoBehaviour
{

    protected Ability _ability; // HERE: for now
    protected IEnumerator _runAbilityCoroutine;
    protected IEnumerator _fireAbilityCoroutine;

    public virtual void Initialize(Ability ability)
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
            
            _ability.StartCooldown();
            yield return new WaitForSeconds(_ability.GetCooldown());
        }

    }

    protected virtual IEnumerator FireAbilityCoroutine()
    {
        // override this method in child classes
        yield return null;
    }

}
