using Lis.Battle;
using Lis.Battle.Fight;
using UnityEngine;

namespace Lis
{
    public class FightStarter : MonoBehaviour, IInteractable
    {
        FightManager _fightManager;

        void Start()
        {
            _fightManager = BattleManager.Instance.GetComponent<FightManager>();
        }

        public string InteractionPrompt => "Start Fight";
        public bool CanInteract() => true;
        public void DisplayTooltip() => Debug.Log("Displaying tooltip");
        public void HideTooltip() => Debug.Log("Hiding tooltip");

        public bool Interact(Interactor interactor, out bool wasSuccess)
        {
            Debug.Log("Interacting with FightStarter");
            if (_fightManager.IsFightActive)
            {
                Debug.Log("Fight is already active");
                wasSuccess = false;
                return false;
            }

            _fightManager.StartFight();

            wasSuccess = true;
            return true;
        }
    }
}