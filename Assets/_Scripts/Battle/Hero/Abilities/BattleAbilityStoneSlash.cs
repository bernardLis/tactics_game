using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleAbilityStoneSlash : BattleAbility
{
    [SerializeField] GameObject _slashPrefab;

    public override void Initialize(Ability ability)
    {
        base.Initialize(ability);
        transform.localPosition = new Vector3(0f, 0.5f, 1f); // it is where the effect spawns...
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();

        for (int i = 0; i < 1; i++)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.parent.rotation; // risky bisquits, I want to get player's rotation
            GameObject instance = Instantiate(_slashPrefab, pos, rot);
            instance.transform.localScale = Vector3.one * _ability.GetScale();

            Debug.Log($"Vector3.one * _ability.GetScale() {Vector3.one * _ability.GetScale()}");
            instance.transform.localRotation = rot;
            instance.SetActive(true);
            instance.GetComponent<BattleStoneSlash>().Initialize(_ability);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
