using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Lis.Core.Utilities
{
    public class LevelLoader : MonoBehaviour
    {
        VisualElement _root;
        VisualElement _crossfade;

        void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _crossfade = _root.Q<VisualElement>("crossfade");
        }

        public void LoadLevel(string newScene)
        {
            DOTween.KillAll();

            _crossfade.style.opacity = 0;
            _crossfade.style.display = DisplayStyle.Flex;
            DOTween.To(() => _crossfade.style.opacity.value, x => _crossfade.style.opacity = x, 1f, 1f)
                .SetEase(Ease.InSine)
                .OnComplete(() => FadeIn(newScene));
        }

        void FadeIn(string newScene)
        {
            SceneManager.LoadScene(newScene);

            _crossfade.style.opacity = 1;
            _crossfade.style.display = DisplayStyle.Flex;
            DOTween.To(() => _crossfade.style.opacity.value, x => _crossfade.style.opacity = x, 0f, 1f)
                .SetEase(Ease.InSine)
                .OnComplete(HideCrossfade);
        }

        void HideCrossfade() { _crossfade.style.display = DisplayStyle.None; }
    }
}
