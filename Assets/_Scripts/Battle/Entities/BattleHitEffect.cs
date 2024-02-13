using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleHitEffect : MonoBehaviour
    {
        Transform _originalParent;

        void Start()
        {
            _originalParent = transform.parent;
        }

        public void PlayEffect(BattleEntity entity)
        {
            Transform t = transform;
            t.parent = entity.transform;
            t.position = entity.Collider.bounds.center;
            gameObject.SetActive(true);
            StartCoroutine(Deactivate());
        }

        IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(2f);
            transform.parent = _originalParent;
            gameObject.SetActive(false);
        }
    }
}