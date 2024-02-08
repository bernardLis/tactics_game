using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace Lis
{
    public class BattleFriendBall : MonoBehaviour
    {
        BattleHero _hero;

        Rigidbody _rb;
        Collider _collider;

        const int _minForceForward = 100;
        const int _maxForceForward = 600;

        int _floorCollisionCount;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        //https://forum.unity.com/threads/how-to-calculate-force-needed-to-jump-towards-target-point.372288/
        public void Throw(Quaternion rot, Vector3 endPos)
        {
            Transform t = transform;
            t.DOScale(Vector3.one * 0.2f, 0.2f);
            t.rotation = rot;
            t.position += Vector3.up + t.forward;
            StartCoroutine(MoveInArcCoroutine(endPos));

            StartCoroutine(Disappear());
        }

        IEnumerator MoveInArcCoroutine(Vector3 pos)
        {
            float time = 0;
            const float duration = 1f;
            Vector3 startPos = transform.position;
            Vector3 endPos = pos;
            endPos.y = 0;
            Vector3 midPoint = (startPos + endPos) / 2;
            midPoint.y += 5;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                transform.position = Vector3.Lerp(Vector3.Lerp(startPos, midPoint, t),
                    Vector3.Lerp(midPoint, endPos, t), t);
                yield return new WaitForFixedUpdate();
            }

            // so it goes forward a bit
            _rb.AddForce(transform.forward * 150);
        }

        IEnumerator Disappear()
        {
            yield return new WaitForSeconds(3f);
            DisableSelf();
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == Tags.BattleFloorLayer)
            {
                _floorCollisionCount++;
                if (_floorCollisionCount > 2)
                    DisableSelf();
            }

            if (other.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (other.gameObject.TryGetComponent(out BattleCreature bc))
            {
                if (bc.Team == 0) return; // TODO: hardcoded team number
                Debug.Log("friend ball collision with hostile battle creature!");
            }
        }

        void DisableSelf()
        {
            _collider.enabled = false;
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;

                gameObject.SetActive(false);
                transform.position = Vector3.zero;
                _collider.enabled = true;
                _floorCollisionCount = 0;
            });
        }
    }
}