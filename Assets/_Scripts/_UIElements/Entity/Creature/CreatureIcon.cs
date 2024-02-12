using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;


namespace Lis
{
    public class CreatureIcon : EntityIcon
    {
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
        }
    }
}