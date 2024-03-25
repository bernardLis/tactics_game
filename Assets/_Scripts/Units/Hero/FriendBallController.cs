using System.Collections;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Creature;
using Lis.Units.Minion;
using UnityEngine;

namespace Lis.Units.Hero
{
    public class FriendBallController : MonoBehaviour
    {
        AudioManager _audioManager;
        HeroController _heroController;

        Rigidbody _rb;
        Collider _collider;

        [SerializeField] GameObject _hit;
        [SerializeField] GameObject _successEffect;

        [SerializeField] Sound _caughtSound;
        [SerializeField] Sound _notCaughtSound;

        int _floorCollisionCount;
        bool _wasTryingToCatch;


        void Awake()
        {
            _audioManager = AudioManager.Instance;
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void PerfectThrow(Quaternion rot, CreatureController bc)
        {
            InitializeThrow(rot);
            StartCoroutine(MoveInArcToCreatureCoroutine(bc));
        }

        IEnumerator MoveInArcToCreatureCoroutine(CreatureController bc)
        {
            float time = 0;
            const float duration = 1f;
            Vector3 startPos = transform.position;
            Vector3 endPos = bc.transform.position;
            endPos.y = 0;
            Vector3 midPoint = (startPos + endPos) / 2;
            midPoint.y += 5;

            while (time < duration)
            {
                endPos = bc.transform.position;

                time += Time.deltaTime;
                float t = time / duration;
                transform.position = Vector3.Lerp(Vector3.Lerp(startPos, midPoint, t),
                    Vector3.Lerp(midPoint, endPos, t), t);
                yield return new WaitForFixedUpdate();
            }
        }

        public void Throw(Quaternion rot, Vector3 endPos)
        {
            InitializeThrow(rot);
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

        void InitializeThrow(Quaternion rot)
        {
            if (_heroController == null) _heroController = BattleManager.Instance.HeroController;

            _wasTryingToCatch = false;

            Transform t = transform;
            t.DOScale(Vector3.one, 0.2f);
            t.rotation = rot;
            t.position += Vector3.up + t.forward;
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

            if (other.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (other.gameObject.TryGetComponent(out CreatureController bc))
            {
                if (bc.Team == 0) return; // TODO: hardcoded team number
                if (bc.IsDead) return;
                if (bc is RangedMinionController) return;
                TryCatching(bc);
            }
        }

        void TryCatching(CreatureController bc)
        {
            if (_wasTryingToCatch) return;
            _wasTryingToCatch = true;

            StopAllCoroutines();
            transform.DOKill();
            StartCoroutine(CatchingCoroutine(bc));
        }

        IEnumerator CatchingCoroutine(CreatureController bc)
        {
            _hit.SetActive(true);

            if (!_heroController.Hero.CanAddToTroops())
            {
                _audioManager.PlaySfx(_notCaughtSound, transform.position);
                string text = "No space for more creatures!";
                bc.DisplayFloatingText(text, Color.white);
                yield break;
            }

            _rb.isKinematic = true;
            yield return transform.DOMoveY(5f, 0.5f).WaitForCompletion();
            bc.TryCatching(this);
            yield return CatchingEffect();

            float chanceToCatch = bc.Creature.CalculateChanceToCatch(_heroController.Hero);
            if (Random.value <= chanceToCatch)
            {
                StartCoroutine(Caught(bc));
                yield break;
            }

            _audioManager.PlaySfx(_notCaughtSound, transform.position);
            bc.DisplayFloatingText("Escaped!", Color.red);
            _rb.isKinematic = false;
            bc.ReleaseFromCatching();
            yield return new WaitForSeconds(2f);
            DisableSelf();
        }

        IEnumerator CatchingEffect()
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            int shakeCount = Random.Range(1, 4);
            Transform t = transform;
            t.DOShakePosition(1, Vector3.one * 0.5f)
                .SetLoops(shakeCount, LoopType.Restart);
            float punchScale = t.localScale.x + 0.1f;
            yield return t.DOPunchScale(Vector3.one * punchScale, 1f, 3, 0.3f)
                .SetLoops(shakeCount, LoopType.Restart)
                .WaitForCompletion();
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }

        IEnumerator Caught(CreatureController bc)
        {
            if (_heroController == null) _heroController = BattleManager.Instance.HeroController;
            Vector3 pos = _heroController.transform.position
                          + Vector3.right * Random.Range(-3f, 3f)
                          + Vector3.forward * Random.Range(-3f, 3f)
                          + Vector3.up * 3f;

            _audioManager.PlaySfx(_caughtSound, transform.position);
            bc.DisplayFloatingText("Caught!", Color.green);
            Vector3 effPos = bc.transform.position;
            effPos.y = 0;
            GameObject successEffect = Instantiate(_successEffect, effPos, Quaternion.identity);
            successEffect.SetActive(true);
            Destroy(successEffect, 4f);

            yield return new WaitForSeconds(1f);
            yield return transform.DOMove(pos, 0.5f).WaitForCompletion();

            bc.Caught(pos);

            DisableSelf();
        }

        void DisableSelf()
        {
            _hit.SetActive(false);

            _collider.enabled = false;
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                _rb.isKinematic = false;

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