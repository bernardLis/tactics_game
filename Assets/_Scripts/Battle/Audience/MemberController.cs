using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Battle.Audience
{
    public class MemberController : MonoBehaviour
    {
        public void Initialize()
        {
            StartCoroutine(JumpCoroutine());
        }

        IEnumerator JumpCoroutine()
        {
            while (true)
            {
                if (this == null) yield break;
                float power = Random.Range(0.5f, 1.5f);
                float duration = Random.Range(0.5f, 1.5f);
                transform.DOLocalJump(transform.localPosition, power, 1, duration);
                yield return new WaitForSeconds(Random.Range(2f, 10f));
            }
        }
    }
}