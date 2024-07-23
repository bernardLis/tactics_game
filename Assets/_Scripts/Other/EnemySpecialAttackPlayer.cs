using System.Collections;
using UnityEngine;

namespace Lis.Other
{
    public class EnemySpecialAttackPlayer : MonoBehaviour
    {
        Animator _anim;

        public void Initialize()
        {
            _anim = transform.parent.GetComponentInChildren<Animator>();
            Invoke(nameof(StartCor), 3f);
        }

        void StartCor()
        {
            StartCoroutine(PlaySpecialAttackAnimationCoroutine());
        }

        IEnumerator PlaySpecialAttackAnimationCoroutine()
        {
            while (true)
            {
                _anim.SetTrigger("Special Attack");
                yield return new WaitForSeconds(2f);
            }
        }
    }
}