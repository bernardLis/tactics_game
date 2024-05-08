using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units.Hero;
using TMPro;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class ArenaInteractable : MonoBehaviour, IInteractable
    {
        protected FightManager FightManager;

        [SerializeField] Canvas _tooltipCanvas;
        [SerializeField] protected TMP_Text TooltipText;
        [SerializeField] protected GameObject InteractionAvailableEffect;

        protected bool IsInteractionAvailable;

        protected virtual void Start()
        {
            FightManager = BattleManager.Instance.GetComponent<FightManager>();
            FightManager.OnFightEnded += OnFightEnded;
            FightManager.OnFightStarted += OnFightStarted;

            SetTooltipText();
        }

        protected virtual void OnFightEnded()
        {
        }

        protected virtual void OnFightStarted()
        {
        }

        protected virtual void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        public string InteractionPrompt => "Press F To Something";

        public virtual bool CanInteract() => IsInteractionAvailable;

        public virtual void DisplayTooltip()
        {
            if (CanInteract())
                _tooltipCanvas.gameObject.SetActive(true);
        }

        public virtual void HideTooltip()
        {
            _tooltipCanvas.gameObject.SetActive(false);
        }

        public virtual bool Interact(Interactor interactor)
        {
            throw new System.NotImplementedException();
        }
    }
}