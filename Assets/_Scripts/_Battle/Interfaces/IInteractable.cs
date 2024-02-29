

using Lis.Units.Hero;

namespace Lis
{
    public interface IInteractable
    {
        public bool CanInteract(Interactor interactor);
        public void DisplayTooltip();
        public void HideTooltip();
        public bool Interact(Interactor interactor);
    }
}