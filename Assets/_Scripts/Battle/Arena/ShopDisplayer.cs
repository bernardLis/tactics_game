using Lis.Battle.Fight;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class ShopDisplayer : ArenaInteractable, IInteractable
    {
        public new string InteractionPrompt => "Press F To Shop!";

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight instead of shopping!");
                return false;
            }

            Debug.Log("Shopping!");
            return true;
        }
    }
}