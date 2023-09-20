using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleAbilityWindTornado : BattleAbility
{
    [SerializeField] GameObject _tornadoPrefab;
    List<GameObject> _tornadoInstances = new();

    public override void Initialize(Ability ability)
    {
        base.Initialize(ability);
        transform.localPosition = new Vector3(0.5f, 1f, 0.5f);
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();

        for (int i = 0; i < Mathf.RoundToInt(_ability.GetScale()); i++)
        {
            Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
            Quaternion q = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            GameObject instance = Instantiate(_tornadoPrefab, pos, q);
            instance.SetActive(true);
            instance.GetComponent<BattleTornado>().Initialize(_ability);
            _tornadoInstances.Add(instance);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
