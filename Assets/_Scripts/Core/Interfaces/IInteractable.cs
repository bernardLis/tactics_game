using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public interface IInteractable
    {
        public string InteractionPrompt { get; }
        public bool CanInteract();
        public void DisplayTooltip();
        public void HideTooltip();
        public bool Interact(Interactor interactor, out bool wasSuccess);
    }
}