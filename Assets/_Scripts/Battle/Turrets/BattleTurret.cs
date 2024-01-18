using System.Collections;
using System.Collections.Generic;
using System.Linq;






using DG.Tweening;
using Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis
{
    public class BattleTurret : MonoBehaviour, IGrabbable, IPointerDownHandler
    {
        BattleManager _battleManager;
        BattleTooltipManager _tooltipManager;
        BattleGrabManager _grabManager;

        [SerializeField] GameObject _rangeIndicator;
        Disc _rangeDisc;
        [SerializeField] BattleProjectile _projectilePrefab;
        [SerializeField] GameObject _GFX;

        public int Team { get; private set; }
        public Turret Turret { get; private set; }

        BattleEntity _target;

        IEnumerator _runTurretCoroutine;

        void Start()
        {
            _battleManager = BattleManager.Instance;
            _tooltipManager = BattleTooltipManager.Instance;
            _grabManager = BattleGrabManager.Instance;

            transform.parent = _battleManager.EntityHolder;
        }

        public void Initialize(Turret turret)
        {
            Team = 0; // HERE: turret team

            Turret = turret;
            Turret.OnLevelUp += UpdateTurret;
            UpdateTurret();

            _rangeIndicator.SetActive(true);
        }

        void UpdateTurret()
        {
            UpdateGFX();
            UpdateRangeIndicator();
        }

        public void UpdateGFX()
        {
            // TODO: maybe some effect
            _GFX.transform.DOScale(_GFX.transform.localScale * 1.1f, 0.5f)
                .SetEase(Ease.InFlash);
        }

        void UpdateRangeIndicator()
        {
            _rangeDisc = _rangeIndicator.GetComponent<Disc>();
            _rangeDisc.Radius = Turret.AttackRange.GetValue();

            Color c = Color.blue;
            c.a = 0.3f;
            _rangeDisc.Color = c;
        }


        public void StartTurretCoroutine()
        {
            _rangeIndicator.SetActive(false);

            _runTurretCoroutine = RunTurret();
            StartCoroutine(_runTurretCoroutine);
        }

        IEnumerator RunTurret()
        {
            while (true)
            {
                yield return new WaitForSeconds(Turret.AttackCooldown.GetValue());
                ChooseNewTarget();
                if (_target == null) continue;
                FireProjectile();
            }
        }

        void ChooseNewTarget()
        {
            _target = null;

            Dictionary<BattleEntity, float> distances = new();
            foreach (BattleEntity be in _battleManager.OpponentEntities)
            {
                if (be.IsDead) continue;
                float distance = Vector3.Distance(transform.position, be.transform.position);
                if (distance <= Turret.AttackRange.GetValue())
                    distances.Add(be, distance);
            }

            if (distances.Count == 0) return;

            _target = distances.OrderByDescending(pair => pair.Value).Reverse().Take(1).First().Key;
        }

        void FireProjectile()
        {
            BattleProjectile projectileInstance = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            projectileInstance.transform.parent = transform;
            projectileInstance.Initialize(Team);
            Vector3 dir = (_target.transform.position - transform.position).normalized;
            projectileInstance.Shoot(Turret, dir);
        }

        public bool CanBeGrabbed() { return true; }

        public void Grabbed()
        {
            if (_runTurretCoroutine != null)
                StopCoroutine(_runTurretCoroutine);
        }

        public void Released()
        {
            if (_runTurretCoroutine != null) StopCoroutine(_runTurretCoroutine);
            StartCoroutine(_runTurretCoroutine);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_runTurretCoroutine == null) return;

            if (_tooltipManager.CurrentTooltipDisplayer == gameObject) return;
            TurretCard c = new(Turret);
            _tooltipManager.ShowTooltip(c, gameObject);

            _rangeIndicator.SetActive(true);

            _tooltipManager.OnTooltipHidden += OnTooltipHidden;
        }

        void OnTooltipHidden()
        {
            _rangeIndicator.SetActive(false);
            _tooltipManager.OnTooltipHidden -= OnTooltipHidden;
        }
    }
}
