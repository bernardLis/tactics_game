using UnityEngine;

namespace Lis.Units.Pawn
{
    public class PawnController : UnitController
    {
        [Header("Pawn")] // TODO: this is so imperfect
        [SerializeField] private GameObject _upgradeOneBody;

        [SerializeField] private GameObject _upgradeTwoBody;
        [SerializeField] private GameObject _upgradeThreeBody;

        Pawn _pawn;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _pawn = (Pawn)unit;

            _upgradeOneBody.SetActive(true);
            _upgradeTwoBody.SetActive(false);
            _upgradeThreeBody.SetActive(false);
            HandleAnimatorChange(_upgradeOneBody.GetComponent<Animator>());


            _pawn.OnUpgraded += OnPawnUpgraded;
        }

        void OnPawnUpgraded()
        {
            // TODO: this is so imperfect

            // HERE: play effect
            if (_pawn.CurrentUpgrade == 2)
            {
                _upgradeOneBody.SetActive(false);
                _upgradeTwoBody.SetActive(true);
                HandleAnimatorChange(_upgradeTwoBody.GetComponent<Animator>());
            }
            else if (_pawn.CurrentUpgrade == 3)
            {
                _upgradeTwoBody.SetActive(false);
                _upgradeThreeBody.SetActive(true);
                HandleAnimatorChange(_upgradeThreeBody.GetComponent<Animator>());
            }
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