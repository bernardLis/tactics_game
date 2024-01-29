using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleTileCanvas : MonoBehaviour
    {
        Transform _battleHeroTransform;
        BattleTile _battleTile;
        float _maxMoveDistance;

        IEnumerator _updatePositionCoroutine;

        void OnEnable()
        {
            _battleHeroTransform = BattleManager.Instance.BattleHero.transform;
            _battleTile = GetComponentInParent<BattleTile>();
            _maxMoveDistance = _battleTile.Scale * 0.5f;

            _updatePositionCoroutine = UpdatePositionCoroutine();
            StartCoroutine(_updatePositionCoroutine);
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

                Vector3 heroPos = _battleHeroTransform.position;
                Vector3 tilePos = _battleTile.transform.position;

                Vector3 dir = (heroPos - tilePos).normalized;
                Vector3 newPos = tilePos + dir * _maxMoveDistance;
                newPos.y = 2;
                transform.position = newPos;

                // Vector3 lookRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
                // transform.rotation = Quaternion.Euler(lookRotation);
                Quaternion heroRot = _battleHeroTransform.rotation;
                transform.LookAt(transform.position + heroRot * Vector3.forward,
                    heroRot * Vector3.up);

                yield return null;
            }
        }
    }
}