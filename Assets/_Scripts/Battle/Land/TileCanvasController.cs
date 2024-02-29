using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Battle.Land
{
    public class TileCanvasController : MonoBehaviour
    {
        Camera _cam;
        Transform _heroTransform;
        TileController _tileController;

        float _maxMoveDistance;

        IEnumerator _updatePositionCoroutine;

        void OnEnable()
        {
            _heroTransform = BattleManager.Instance.HeroController.transform;
            _tileController = GetComponentInParent<TileController>();
            _maxMoveDistance = _tileController.Scale * 0.5f - 5f; // TODO: magic 5

            _updatePositionCoroutine = UpdatePositionCoroutine();
            StartCoroutine(_updatePositionCoroutine);

            transform.localPosition = Vector3.zero;

            _cam = Camera.main;
            if (_cam == null) return;
            transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward,
                Vector3.up);
        }

        private void OnDisable()
        {
            if (_updatePositionCoroutine != null)
                StopCoroutine(_updatePositionCoroutine);
        }

        IEnumerator UpdatePositionCoroutine()
        {
            while (true)
            {
                if (!gameObject.activeSelf) yield break;

                Vector3 heroPos = _heroTransform.position;
                Vector3 tilePos = _tileController.transform.position;
                float sqDist = (heroPos - tilePos).sqrMagnitude;

                while (sqDist > 1200)
                {
                    heroPos = _heroTransform.position;
                    sqDist = (heroPos - tilePos).sqrMagnitude;
                    yield return new WaitForSeconds(0.3f);
                }

                Vector3 dir = (heroPos - tilePos).normalized;
                Vector3 newPos = tilePos + dir * _maxMoveDistance;
                newPos.y = 2;
                transform.DOMove(newPos, 0.9f);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}