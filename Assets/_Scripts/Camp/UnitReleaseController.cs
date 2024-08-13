using System;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Camp
{
    public class UnitReleaseController : MonoBehaviour, IInteractable
    {
        public event Action OnUnitsReleased;

        public string InteractionPrompt => "Release Units";

        public bool CanInteract()
        {
            return true;
        }

        public bool Interact(Interactor interactor)
        {
            OnUnitsReleased?.Invoke();
            return true;
        }
    }
}