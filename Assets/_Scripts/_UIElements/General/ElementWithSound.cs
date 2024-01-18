
using UnityEngine.UIElements;

namespace Lis
{
    public class ElementWithSound : VisualElement
    {
        protected AudioManager _audioManager;

        public ElementWithSound()
        {
            _audioManager = AudioManager.Instance;
            RegisterCallback<MouseEnterEvent>((evt) => PlayClick());
        }

        protected void PlayClick() { _audioManager.PlayUI("UI Click"); }

    }
}
