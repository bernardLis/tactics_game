using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Shapes;
using DG.Tweening;
using TMPro;

namespace Lis
{
    public class BattleThrowIndicator : MonoBehaviour
    {
        BattleManager _battleManager;

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
            if (_battleManager == null) _battleManager = BattleManager.Instance;
            if (_hero == null) _hero = _battleManager.BattleHero;
            transform.parent = _battleManager.EntityHolder;

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

            HideChanceToCatch();
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

        IEnumerator FillDiscInTime(Disc disc, float duration)
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

        [SerializeField] Canvas _canvas;
        [SerializeField] TMP_Text _captureChanceText;
        BattleCreature _currentCreature;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BattleCreature bc))
            {
                if (bc.Team == 0) return; // TODO: hardcoded team number
                DisplayChanceToCatch(bc);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out BattleCreature bc))
            {
                if (bc != _currentCreature) return;
                HideChanceToCatch();
            }
        }

        void DisplayChanceToCatch(BattleCreature bc)
        {
            _currentCreature = bc;
            _canvas.gameObject.SetActive(true);
            UpdateCaptureChance(default);
            bc.OnDamageTaken += UpdateCaptureChance;
        }

        void UpdateCaptureChance(int _)
        {
            float chanceToCatch = _currentCreature.Creature.CalculateChanceToCatch(_hero.Hero);
            Color color = Color.red;
            if (chanceToCatch > 0.4f) color = Color.yellow;
            if (chanceToCatch > 0.6f) color = Color.green;
            _captureChanceText.color = color;

            _captureChanceText.text =
                $"{chanceToCatch * 100}% chance to catch";
        }

        void HideChanceToCatch()
        {
            if (_currentCreature == null) return;
            _currentCreature.OnDamageTaken -= UpdateCaptureChance;
            _currentCreature = null;
            _canvas.gameObject.SetActive(false);
            _captureChanceText.text = "";
        }
        
        readonly Collider[] _colliders = new Collider[10];
        public BattleCreature GetCreature()
        {
            // check if there are creatures in collider
            int r = Physics.OverlapSphereNonAlloc(transform.position, 0.8f, _colliders);
            if (r == 0) return null;
            foreach (Collider c in _colliders)
            {
                if (c == null) continue;
                if (!c.gameObject.TryGetComponent(out BattleCreature bc)) continue;
                if (bc.Team != 1) continue;
                return bc;
            }

            return null;
        }
    }
}