using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Pawn
{
    public class PawnController : UnitController
    {
        [Header("Pawn")] // TODO: this is so imperfect
        [SerializeField] GameObject _upgradeZeroBody;

        [SerializeField] GameObject _upgradeOneBody;
        [SerializeField] GameObject _upgradeTwoBody;

        [SerializeField] GameObject _upgradeEffect;

        Pawn _pawn;

        bool _isUpgrading;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _pawn = (Pawn)unit;

            _upgradeZeroBody.SetActive(true);
            _upgradeOneBody.SetActive(false);
            _upgradeTwoBody.SetActive(false);
            HandleAnimatorChange(_upgradeZeroBody.GetComponent<Animator>());

            _pawn.OnUpgraded += OnPawnUpgraded;
        }

        protected override IEnumerator RunUnitCoroutine()
        {
            while (_isUpgrading) yield return new WaitForSeconds(1f);
            yield return base.RunUnitCoroutine();
        }

        void OnPawnUpgraded()
        {
            // TODO: this is so imperfect
            StartCoroutine(UpgradeCoroutine());
        }

        IEnumerator UpgradeCoroutine()
        {
            AddToLog("Upgrading...");
            _isUpgrading = true;
            _upgradeEffect.SetActive(true);
            Animator.transform.DOLocalRotate(new(0, 360, 0), 1.5f, RotateMode.LocalAxisAdd);
            yield return Animator.transform.DOLocalMoveY(2, 1f).WaitForCompletion();
            yield return new WaitForSeconds(0.5f);

            if (_pawn.CurrentUpgrade == 1)
            {
                _upgradeZeroBody.SetActive(false);
                ActivateBody(_upgradeOneBody);
                HandleAnimatorChange(_upgradeOneBody.GetComponent<Animator>());
            }
            else if (_pawn.CurrentUpgrade == 2)
            {
                _upgradeOneBody.SetActive(false);
                ActivateBody(_upgradeTwoBody);
                HandleAnimatorChange(_upgradeTwoBody.GetComponent<Animator>());
            }

            yield return new WaitForSeconds(3f);
            _isUpgrading = false;
            _upgradeEffect.SetActive(false);
        }

        void ActivateBody(GameObject body)
        {
            body.transform.localPosition += Vector3.up * 2;
            body.SetActive(true);
            body.transform.DOLocalMoveY(0, 1.5f);
            body.transform.DORotate(new(0, 360, 0), 1.5f, RotateMode.LocalAxisAdd);
        }

        void HandleAnimatorChange(Animator animator)
        {
            Animator = animator;
            UnitPathingController.SetAnimator(animator);
            foreach (Attack.Attack a in _pawn.Attacks)
            {
                if (a == null) continue;
                if (a.AttackController == null) continue;
                a.AttackController.SetAnimator(animator);
            }
        }
    }
}