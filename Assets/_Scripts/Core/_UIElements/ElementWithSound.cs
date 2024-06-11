using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ElementWithSound : VisualElement
    {
        private readonly AudioManager _audioManager;

        protected ElementWithSound()
        {
            _audioManager = AudioManager.Instance;
            RegisterCallback<PointerEnterEvent>(PlayClick);
        }

        private void PlayClick(PointerEnterEvent evt)
        {
            _audioManager.PlayUI("UI Click");
        }
    }
}