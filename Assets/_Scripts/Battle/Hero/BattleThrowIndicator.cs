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

        BattleHero _hero;

        IEnumerator _showCoroutine;
        IEnumerator _followMouseCoroutine;

        const float _fillTime = 2; // TODO: magic number
        const float _maxDistanceFromHero = 20;

        public void Awake()
        {
            _cam = Camera.main;
            _mouse = Mouse.current;
        }

        public void Show()
        {
            if (_hero == null) _hero = BattleManager.Instance.BattleHero;

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
                if (!Physics.Raycast(ray, out RaycastHit hit, 100, 1 << LayerMask.NameToLayer("Floor")))
                    yield return new WaitForSeconds(0.1f);

                // limit distance from hero
                Vector3 heroPos = _hero.transform.position;
                Vector3 hitPos = hit.point;
                Vector3 dir = hitPos - heroPos;
                if (dir.magnitude > _maxDistanceFromHero)
                {
                    dir = dir.normalized * _maxDistanceFromHero;
                    hitPos = heroPos + dir;
                }

                hitPos.y = 0.1f;
                transform.position = hitPos;
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