using System;
using DG.Tweening;
using Lis.Arena;
using Lis.Units.Hero;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    public class PlayerUnitController : UnitController
    {
        [SerializeField] protected GameObject TeleportEffect;

        protected HeroController HeroController;

        public event Action OnRevived;

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
            HeroController = HeroManager.Instance.HeroController;
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

            StartCoroutine(CurrentMainCoroutine);
        }

        public virtual void TeleportToMap()
        {
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
            {
                AudioManager.CreateSound()
                    .WithSound(Unit.TeleportSound)
                    .WithParent(transform)
                    .Play();
            }
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
            OnRevived?.Invoke();
        }

        [Button]
        public void DebugRespawn()
        {
            Revive();
        }
    }
}