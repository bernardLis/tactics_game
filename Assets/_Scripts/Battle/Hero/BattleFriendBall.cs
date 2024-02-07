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
        Camera _cam;
        Mouse _mouse;

        Rigidbody _rb;
        Collider _collider;

        const int _minForceForward = 300;
        const int _maxForceForward = 600;

        const int _minThrowDistance = 10;
        const int _maxThrowDistance = 25;

        int _floorCollisionCount;

        void Awake()
        {
            _cam = Camera.main;
            _mouse = Mouse.current;
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void Throw()
        {
            transform.DOScale(Vector3.one * 0.2f, 0.2f);

            // ray to the floor where mouse is pointing
            Vector3 mousePosition = _mouse.position.ReadValue();
            Ray ray = _cam.ScreenPointToRay(mousePosition);
            int layerMask = Tags.BattleFloorLayer;

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask)) return;

            Vector3 pos = new(hit.point.x, 1, hit.point.z);
            Transform t = transform;
            t.LookAt(pos);
            t.position += Vector3.up + t.forward;

            // I want to add force depending on the distance between the mouse and the ball
            float distance = Vector3.Distance(t.position, hit.point);
            float force = Helpers.Remap(distance, _minThrowDistance, _maxThrowDistance,
                _minForceForward, _maxForceForward);

            _rb.AddForce(transform.forward * force);
            _rb.AddForce(transform.up * force * 0.8f);

            StartCoroutine(Disappear());
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

            if (other.gameObject.TryGetComponent(out BattleCreature bc))
            {
                if (bc.Team == 0) return; // TODO: hardcoded team number
                Debug.Log("friends ball collision with hostile battle creature!");
            }
        }

        void DisableSelf()
        {
            _collider.enabled = false;
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                transform.position = Vector3.zero;
                _collider.enabled = true;
                _floorCollisionCount = 0;
            });
        }
    }
}