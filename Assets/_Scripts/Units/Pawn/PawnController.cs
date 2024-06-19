using System.Collections;
using DG.Tweening;
using Lis.Battle.Pickup;
using UnityEngine;

namespace Lis.Units.Pawn
{
    public class PawnController : PlayerUnitController
    {
        [Header("Pawn")] // TODO: this is so imperfect
        [SerializeField]
        GameObject _upgradeZeroBody;

        [SerializeField] GameObject _upgradeOneBody;
        [SerializeField] GameObject _upgradeTwoBody;

        [SerializeField] GameObject _upgradeEffect;

        bool _isUpgrading;

        Pawn _pawn;

        PickupController _pickedUpPickup;
        IEnumerator _bringingToHeroCoroutine;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _pawn = (Pawn)unit;

            HandleUpgrade();

            _pawn.OnUpgraded += OnPawnUpgraded;
        }

        protected override void OnFightEnded()
        {
            base.OnFightEnded();
            if (IsDead) return;
            _pawn.LevelUp();

            if (_pawn.CurrentMission.IsCompleted)
            {
                _pawn.Upgrade();
                return;
            }

            CollectPickups();
        }


        protected override IEnumerator RunUnitCoroutine()
        {
            while (_isUpgrading) yield return new WaitForSeconds(1f);
            yield return base.RunUnitCoroutine();
        }

        public override void TeleportToBase()
        {
            DropPickup();
            base.TeleportToBase();
        }

        void OnPawnUpgraded()
        {
            StopUnit();
            CurrentMainCoroutine = UpgradeCoroutine();
            StartCoroutine(CurrentMainCoroutine);
        }

        IEnumerator UpgradeCoroutine()
        {
            DropPickup();

            AddToLog("Upgrading...");
            _isUpgrading = true;
            _upgradeEffect.SetActive(true);
            Animator.transform.DOLocalRotate(new(0, 360, 0), 1.5f, RotateMode.LocalAxisAdd);
            yield return Animator.transform.DOLocalMoveY(2, 1f).WaitForCompletion();
            yield return new WaitForSeconds(0.5f);

            HandleUpgrade(true);
            yield return new WaitForSeconds(3f);

            _isUpgrading = false;
            _upgradeEffect.SetActive(false);
            CollectPickups();
        }

        void CollectPickups()
        {
            AddToLog("Collecting pickups...");
            CurrentMainCoroutine = CollectPickupsCoroutine();
            StartCoroutine(CurrentMainCoroutine);
        }

        IEnumerator CollectPickupsCoroutine()
        {
            while (true)
            {
                if (this == null) yield break;

                yield return new WaitForSeconds(1f);
                PickupController pickup = PickupManager.GetPawnCollectablePickup();
                if (pickup == null)
                {
                    yield return new WaitForSeconds(2f);
                    continue;
                }

                yield return UnitPathingController.PathToPositionAndStop(pickup.transform.position);
                if (!pickup.CanBePickedUpByPawn()) continue;
                pickup.PawnPickup(this);
                _pickedUpPickup = pickup;

                _bringingToHeroCoroutine = UnitPathingController.PathToTarget(HeroController.transform);
                StartCoroutine(_bringingToHeroCoroutine);
                while (_pickedUpPickup != null && !_pickedUpPickup.IsCollected)
                    yield return new WaitForSeconds(0.1f);
                DropPickup();
            }
        }

        void DropPickup()
        {
            if (_pickedUpPickup == null) return;
            StopCoroutine(_bringingToHeroCoroutine);
            _pickedUpPickup.PawnDrop();
        }


        void HandleUpgrade(bool animate = false)
        {
            if (_pawn.CurrentUpgrade == 0)
            {
                _upgradeZeroBody.SetActive(true);
                _upgradeOneBody.SetActive(false);
                _upgradeTwoBody.SetActive(false);

                if (animate) ActivateBody(_upgradeZeroBody);
                HandleAnimatorChange(_upgradeZeroBody.GetComponent<Animator>());
            }

            if (_pawn.CurrentUpgrade == 1)
            {
                _upgradeZeroBody.SetActive(false);
                _upgradeOneBody.SetActive(true);
                _upgradeTwoBody.SetActive(false);

                if (animate) ActivateBody(_upgradeOneBody);
                HandleAnimatorChange(_upgradeOneBody.GetComponent<Animator>());
            }
            else if (_pawn.CurrentUpgrade == 2)
            {
                _upgradeZeroBody.SetActive(false);
                _upgradeOneBody.SetActive(false);
                _upgradeTwoBody.SetActive(true);

                if (animate) ActivateBody(_upgradeTwoBody);
                HandleAnimatorChange(_upgradeTwoBody.GetComponent<Animator>());
            }
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