using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Shapes;
using DG.Tweening;

namespace Lis
{
    public class BattleThrowIndicator : MonoBehaviour
    {
        Camera _cam;
        Mouse _mouse;

        [SerializeField] Disc _disc;
        [SerializeField] Disc _overflowDisc;

        IEnumerator _showCoroutine;
        IEnumerator _followMouseCoroutine;

        const float _fillTime = 2; // TODO: magic number

        public void Awake()
        {
            _cam = Camera.main;
            _mouse = Mouse.current;
        }

        public void Show()
        {
            EndShow();

            _followMouseCoroutine = FollowMouseCoroutine();
            StartCoroutine(_followMouseCoroutine);

            _showCoroutine = ShowCoroutine();
            StartCoroutine(_showCoroutine);
        }

        public void EndShow()
        {
            _disc.gameObject.SetActive(false);
            _overflowDisc.gameObject.SetActive(false);

            if (_followMouseCoroutine != null)
                StopCoroutine(_followMouseCoroutine);

            if (_showCoroutine != null)
                StopCoroutine(_showCoroutine);
        }

        IEnumerator FollowMouseCoroutine()
        {
            while (true)
            {
                if (_mouse == null || _cam == null) yield break;
                Vector3 mousePosition = _mouse.position.ReadValue();
                Ray ray = _cam.ScreenPointToRay(mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, 100,  1 << LayerMask.NameToLayer("Floor")))
                    yield return new WaitForSeconds(1f);

                Vector3 pos = hit.point;
                Debug.Log($"pos: {pos}");
                pos.y = 0.1f;
                transform.position = pos;
                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator ShowCoroutine()
        {
            _disc.gameObject.SetActive(true);
            _overflowDisc.gameObject.SetActive(false);
            yield return FillDiscInTime(_disc, _fillTime);

            yield return _disc.transform.DOPunchScale(Vector3.one * 1.1f, 0.5f, 3, 0.3f)
                .SetLoops(2, LoopType.Restart)
                .WaitForCompletion();

            _disc.gameObject.SetActive(false);
            _overflowDisc.gameObject.SetActive(true);
            yield return FillDiscInTime(_overflowDisc, _fillTime);
        }

        private IEnumerator FillDiscInTime(Disc disc, float duration)
        {
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float fill = time / duration;
                disc.AngRadiansEnd = 2 * Mathf.PI * fill;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}