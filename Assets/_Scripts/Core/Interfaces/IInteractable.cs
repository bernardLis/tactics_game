using Lis.Units.Hero;

namespace Lis.Core
{
    public interface IInteractable
    {
        public string InteractionPrompt { get; }
        public bool CanInteract();
        public bool Interact(Interactor interactor);
    }
}