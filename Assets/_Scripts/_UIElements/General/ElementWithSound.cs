using UnityEngine.UIElements;

namespace Lis
{
    public class ElementWithSound : VisualElement
    {
        readonly AudioManager _audioManager;

        protected ElementWithSound()
        {
            _audioManager = AudioManager.Instance;
            RegisterCallback<MouseEnterEvent>(PlayClick);
        }

        void PlayClick(MouseEnterEvent evt)
        {
            _audioManager.PlayUI("UI Click");
        }
    }
}