using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
{
    public class SpinWheelController : MonoBehaviour
    {
        [SerializeField] Transform _arrow;

        IEnumerator _spinCoroutine;

        public event Action<float> OnSpinCompleted;

        [Button]
        public void Spin()
        {
            if (_spinCoroutine != null) return;
            _spinCoroutine = SpinCoroutine();
            StartCoroutine(_spinCoroutine);
        }

        IEnumerator SpinCoroutine()
        {
            yield return new WaitForSeconds(1f);
            float genSpeed = Random.Range(3, 5);
            float subSpeed = Random.Range(0.001f, 0.004f);
            while (genSpeed > 0)
            {
                genSpeed -= subSpeed;
                _arrow.Rotate(0, 0, genSpeed);
                yield return null;
            }

            _spinCoroutine = null;
            OnSpinCompleted?.Invoke(_arrow.eulerAngles.z);
        }
    }
}