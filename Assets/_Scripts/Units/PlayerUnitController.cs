using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Units.Hero;
using NaughtyAttributes;
using UnityEngine;

namespace Lis.Units
{
    public class PlayerUnitController : UnitController
    {
        [SerializeField] GameObject _reviveEffect;

        HeroController _heroController;

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            unit.OnLevelUp -= OnLevelUp;
            unit.OnLevelUp += OnLevelUp;
            unit.OnRevival -= Revive;
            unit.OnRevival += Revive;
        }

        protected override void InitializeControllers()
        {
            base.InitializeControllers();

            if (!TryGetComponent(out UnitGrabController grab)) return;
            grab.Initialize(this);
            grab.OnGrabbed += OnGrabbed;
            grab.OnReleased += OnReleased;

            _heroController = HeroManager.Instance.HeroController;
            _heroController.OnTeleportedToBase += TeleportToBase;
        }

        protected override void OnFightEnded()
        {
            base.OnFightEnded();
            if (IsDead) return;
            GetHealed(100);
        }

        public void GoToLocker()
        {
            if (IsDead) return;

            AddToLog("Going back to locker room.");
            UnitPathingController.SetStoppingDistance(0);

            CurrentMainCoroutine =
                UnitPathingController.PathToPositionAndStop(ArenaManager.GetRandomPositionInPlayerLockerRoom());

            StartCoroutine(CurrentMainCoroutine);
        }

        public virtual void TeleportToBase()
        {
            if (ArenaManager.IsPositionInPlayerBase(transform.position)) return;

            AddToLog("Teleporting to Base");
            StopUnit();
            transform.position = ArenaManager.GetRandomPositionInPlayerLockerRoom();
            GoToLocker();
        }

        public virtual void TeleportToArena()
        {
            AddToLog("Teleporting to Arena.");
            transform.position = new(Random.Range(-3, 3), 1, Random.Range(-3, 3));
        }

        void Revive()
        {
            _reviveEffect.SetActive(false);
            AddToLog("Reviving...");
            Animator.Rebind();
            Animator.Update(0f);
            _reviveEffect.SetActive(true);
            EnableSelf();
            GoToLocker();
        }

        /* GRAB */
        void OnGrabbed()
        {
            ResetOpponent(default, default);
            StopUnit();
        }

        void OnReleased()
        {
            if (FightManager.IsFightActive) RunUnit();
            else if (!ArenaManager.IsPositionInPlayerLockerRoom(transform.position)) GoToLocker();
        }

        [Button]
        public void DebugRespawn()
        {
            Revive();
        }
    }
}