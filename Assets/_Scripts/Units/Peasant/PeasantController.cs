using System.Collections;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Peasant
{
    public class PeasantController : UnitController
    {
        [SerializeField] PeasantUpgradeEffectController _upgradeEffect;
        Peasant _peasant;


        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _peasant = (Peasant)unit;

            _peasant.OnUpgraded += OnPeasantUpgraded;
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
            c.GoBackToLocker();

            yield return new WaitForSeconds(2f);
            DestroySelf();
        }
    }
}