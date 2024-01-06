using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbility : MonoBehaviour
{
    protected Ability _ability; // HERE: for now
    protected IEnumerator _runAbilityCoroutine;
    protected IEnumerator _fireAbilityCoroutine;

    public event Action<Vector3, Vector3> OnAbilityFire;
    public virtual void Initialize(Ability ability, bool startAbility = true)
    {
        _ability = ability;
        if (startAbility) StartAbility();
    }

    public void StartAbility()
    {
        _runAbilityCoroutine = RunAbilityCoroutine();
        StartCoroutine(_runAbilityCoroutine);
    }

    public void StopAbility()
    {
        StopCoroutine(_runAbilityCoroutine);
    }

    IEnumerator RunAbilityCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // time to initialize button
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
        OnAbilityFire?.Invoke(transform.position, transform.rotation.eulerAngles);
        // override this method in child classes
        yield return null;
    }

}
