

namespace Lis
{
    public interface IInteractable
    {
        public bool CanInteract(BattleInteractor interactor);
        public void DisplayTooltip();
        public void HideTooltip();
        public bool Interact(BattleInteractor interactor);
    }
}