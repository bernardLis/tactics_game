using System.Collections;
using UnityEngine;

namespace Lis
{
    public class EnemySpecialAttackPlayer : MonoBehaviour
    {
        private Animator _anim;

        public void Initialize()
        {
            _anim = transform.parent.GetComponentInChildren<Animator>();
            Invoke(nameof(StartCor), 3f);
        }

        private void StartCor()
        {
            StartCoroutine(PlaySpecialAttackAnimationCoroutine());
        }

        private IEnumerator PlaySpecialAttackAnimationCoroutine()
        {
            while (true)
            {
                _anim.SetTrigger("Special Attack");
                yield return new WaitForSeconds(2f);
            }
        }
    }
}