using System.Collections;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Pickup;
using Lis.Core;
using Lis.Units.Pawn;
using UnityEngine;

namespace Lis.Units.Peasant
{
    public class PeasantController : PlayerUnitController
    {
        BreakableVaseManager _breakableVaseManager;

        [SerializeField] PeasantUpgradeEffectController _upgradeEffect;
        Peasant _peasant;
        static readonly int AnimAttack = Animator.StringToHash("Attack");

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _peasant = (Peasant)unit;
            _breakableVaseManager = BattleManager.GetComponent<BreakableVaseManager>();

            _peasant.OnUpgraded += OnPeasantUpgraded;
        }

        protected override void OnFightEnded()
        {
            base.OnFightEnded();
            if (IsDead) return;
            CurrentMainCoroutine = BreakVasesCoroutine();
            StartCoroutine(CurrentMainCoroutine);
        }

        IEnumerator BreakVasesCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                BreakableVaseController vase = _breakableVaseManager.GetRandomActiveVase();
                if (vase == null) yield break;

                yield return UnitPathingController.PathToPositionAndStop(vase.transform.position);
                if (vase.IsBroken) continue;
                Animator.SetTrigger(AnimAttack);
                yield return new WaitForSeconds(0.5f);
                vase.TriggerBreak();
            }
        }

        void OnPeasantUpgraded(Nature nature)
        {
            IsDead = true;
            InvokeDeathEvent();
            StartCoroutine(PeasantUpgradeCoroutine(nature));
        }

        IEnumerator PeasantUpgradeCoroutine(Nature nature)
        {
            Pawn.Pawn p = Instantiate(GameManager.UnitDatabase.GetPawnByNature(nature));
            p.InitializeBattle(0);

            _upgradeEffect.gameObject.SetActive(true);
            _upgradeEffect.Play(nature);

            // set this object as inactive
            Animator.gameObject.SetActive(false);

            HeroManager.Instance.Hero.Army.Add(p); // without update to spawn at position
            UnitController c = FightManager.SpawnPlayerUnit(p, transform.position);
            PawnController pc = c as PawnController;
            if (pc != null) pc.GoToLocker();

            yield return new WaitForSeconds(2f);
            DestroySelf();
        }
    }
}