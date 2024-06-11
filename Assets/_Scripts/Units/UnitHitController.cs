using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Units
{
    public class UnitHitController : MonoBehaviour
    {
        [SerializeField] private List<HitEffectNature> _hitEffectNatures;

        public void Initialize(UnitController unitController)
        {
            unitController.OnHit += ShowHitEffect;
        }

        private void ShowHitEffect(Attack.Attack attack)
        {
            foreach (HitEffectNature hitEffectNature in _hitEffectNatures)
            {
                if (hitEffectNature.NatureName != attack.Nature.NatureName) continue;
                if (hitEffectNature.EffectGameObject.activeSelf) continue;
                StartCoroutine(ShowHitEffect(hitEffectNature.EffectGameObject));
                break;
            }
        }

        private static IEnumerator ShowHitEffect(GameObject effectGameObject)
        {
            effectGameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            effectGameObject.SetActive(false);
        }
    }

    [Serializable]
    internal struct HitEffectNature
    {
        public GameObject EffectGameObject;
        public NatureName NatureName;
    }
}