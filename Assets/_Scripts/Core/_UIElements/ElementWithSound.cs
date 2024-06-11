using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ElementWithSound : VisualElement
    {
        readonly AudioManager _audioManager;

        protected ElementWithSound()
        {
            _audioManager = AudioManager.Instance;
            RegisterCallback<PointerEnterEvent>(PlayClick);
        }

        void PlayClick(PointerEnterEvent evt)
        {
            _audioManager.PlayUI("UI Click");
        }
    }
}