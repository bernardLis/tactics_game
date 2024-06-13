using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Fight;
using Lis.Units.Hero;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    public class PlayerUnitController : UnitController
    {
        [SerializeField] protected GameObject TeleportEffect;

        HeroController _heroController;

        AudioSource _teleportSound;

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

        public override void OnFightStarted()
        {
            if (this == null) return;
            if (Team == 0 && IsDead)
            {
                transform.DOMoveY(0f, 5f)
                    .SetDelay(1f)
                    .OnComplete(DestroySelf);
                return;
            }

            base.OnFightStarted();
        }


        protected override void OnFightEnded()
        {
            if (this == null) return;
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
            if (this == null) return;
            if (IsDead) return;
            if (ArenaManager.IsPositionInPlayerBase(transform.position)) return;

            BaseTeleport();
            AddToLog("Teleporting to Base");
            transform.position = ArenaManager.GetRandomPositionInPlayerLockerRoom() + Vector3.up;
            GoToLocker();
        }

        public virtual void TeleportToArena()
        {
            BaseTeleport();
            AddToLog("Teleporting to Arena.");
            transform.position = new(Random.Range(-3, 3), 1, Random.Range(-3, 3));
        }

        void BaseTeleport()
        {
            StopUnit();
            if (TeleportEffect != null)
            {
                TeleportEffect.SetActive(false);
                TeleportEffect.SetActive(true);
            }

            if (Unit.TeleportSound != null)
                _teleportSound = AudioManager.PlaySfx(Unit.TeleportSound, transform);
            Invoke(nameof(ReturnAudioSource), 1f);
        }

        void ReturnAudioSource()
        {
            if (_teleportSound != null) _teleportSound.transform.parent = AudioManager.transform;
        }

        void Revive()
        {
            TeleportEffect.SetActive(false);
            AddToLog("Reviving...");
            Animator.Rebind();
            Animator.Update(0f);
            TeleportEffect.SetActive(true);
            EnableSelf();
            OnFightEnded();
        }

        void OnDestroy()
        {
            if (_teleportSound != null) _teleportSound.transform.parent = AudioManager.transform;
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