using System.Collections;
using DG.Tweening;
using Lis.Battle;
using Lis.Units.Creature;
using Lis.Units.Minion;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.Units.Hero
{
    public class FriendBallThrowIndicator : MonoBehaviour
    {
        BattleManager _battleManager;

        Camera _cam;
        Mouse _mouse;

        [SerializeField] Disc _disc;
        [SerializeField] Disc _overflowDisc;
        Collider _collider;

        HeroController _heroController;

        IEnumerator _showCoroutine;
        IEnumerator _followMouseCoroutine;

        const float _fillTime = 2; // TODO: magic number
        const float _maxDistanceFromHero = 20;

        public void Awake()
        {
            _cam = Camera.main;
            _mouse = Mouse.current;
            _collider = GetComponent<Collider>();
        }

        public void Show()
        {
            if (_battleManager == null) _battleManager = BattleManager.Instance;
            if (_heroController == null) _heroController = _battleManager.HeroController;
            transform.parent = _battleManager.EntityHolder;

            _collider.enabled = true;

            if (_followMouseCoroutine != null)
                StopCoroutine(_followMouseCoroutine);
            _followMouseCoroutine = FollowMouseCoroutine();
            StartCoroutine(_followMouseCoroutine);

            if (_showCoroutine != null)
                StopCoroutine(_showCoroutine);
            _showCoroutine = ShowCoroutine();
            StartCoroutine(_showCoroutine);
        }

        public void EndShow()
        {
            _collider.enabled = false;

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
                Vector3 heroPos = _heroController.transform.position;
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
        CreatureController _currentCreatureController;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CreatureController cc))
            {
                if (cc.Team == 0) return; // TODO: hardcoded team number
                if (cc is RangedMinionController) return;
                DisplayChanceToCatch(cc);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CreatureController cc))
            {
                if (cc != _currentCreatureController) return;
                HideChanceToCatch();
            }
        }

        void DisplayChanceToCatch(CreatureController cc)
        {
            _currentCreatureController = cc;
            _canvas.gameObject.SetActive(true);
            UpdateCaptureChance(default);
            cc.OnDamageTaken += UpdateCaptureChance;
        }

        void UpdateCaptureChance(int _)
        {
            if (_currentCreatureController == null) return;

            float chanceToCatch = _currentCreatureController.Creature.CalculateChanceToCatch(_heroController.Hero);
            Color color = Color.red;
            if (chanceToCatch > 0.4f) color = Color.yellow;
            if (chanceToCatch > 0.6f) color = Color.green;
            _captureChanceText.color = color;

            _captureChanceText.text =
                $"{chanceToCatch * 100}% chance to catch";
        }

        void HideChanceToCatch()
        {
            if (_currentCreatureController == null) return;
            _currentCreatureController.OnDamageTaken -= UpdateCaptureChance;
            _currentCreatureController = null;
            _canvas.gameObject.SetActive(false);
            _captureChanceText.text = "";
        }

        readonly Collider[] _colliders = new Collider[10];

        public CreatureController GetCreature()
        {
            // check if there are creatures in collider
            int r = Physics.OverlapSphereNonAlloc(transform.position, 0.8f, _colliders);
            if (r == 0) return null;
            foreach (Collider c in _colliders)
            {
                if (c == null) continue;
                if (!c.gameObject.TryGetComponent(out CreatureController bc)) continue;
                if (bc.Team != 1) continue;
                if (bc is RangedMinionController) continue;
                return bc;
            }

            return null;
        }
    }
}