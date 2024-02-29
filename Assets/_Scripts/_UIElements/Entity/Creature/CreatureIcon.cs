using DG.Tweening;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis
{
    public class CreatureIcon : EntityIcon
    {
        OverlayTimerElement _overlayTimer;

        // used only in troops element for now, maybe should have different name?
        public CreatureIcon(Creature creature, bool blockClick = false) : base(creature, blockClick)
        {
            DOTween.Kill(transform);
            UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
            UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);

            style.width = 50;
            style.height = 50;

            IconContainer.style.width = 45;
            IconContainer.style.height = 45;

            Frame.style.width = 50;
            Frame.style.height = 50;

            if (creature.Team != 0) return;
            creature.OnDeath += OnCreatureDeath;
        }

        void OnCreatureDeath()
        {
            int time = Creature.DeathPenaltyBase +
                       Creature.DeathPenaltyPerLevel * Entity.Level.Value;
            _overlayTimer = new(time, time, false, "");
            _overlayTimer.SetTimerFontSize(14);
            Add(_overlayTimer);
            _overlayTimer.OnTimerFinished += RemoveOverlayTimer;
        }

        void RemoveOverlayTimer()
        {
            if (_overlayTimer != null)
            {
                _overlayTimer.RemoveFromHierarchy();
                _overlayTimer = null;
            }
        }
    }
}