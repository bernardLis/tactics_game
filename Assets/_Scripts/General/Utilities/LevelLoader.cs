using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
        GameManager.Instance.SetPreviousLevel(SceneManager.GetActiveScene().name);

        DOTween.KillAll();

        // fade out opacity 0 -> 1
        _crossfade.style.opacity = 0;
        _crossfade.style.display = DisplayStyle.Flex;
        DOTween.To(() => _crossfade.style.opacity.value, x => _crossfade.style.opacity = x, 1f, 1f)
            .SetEase(Ease.InSine).OnComplete(() => FadeIn(newScene));
    }

    void FadeIn(string newScene)
    {
        SceneManager.LoadScene(newScene);

        // fade in opacity 1 -> 0
        _crossfade.style.opacity = 1;
        _crossfade.style.display = DisplayStyle.Flex;
        DOTween.To(() => _crossfade.style.opacity.value, x => _crossfade.style.opacity = x, 0f, 1f)
            .SetEase(Ease.InSine).OnComplete(HideCrossfade);

    }

    void HideCrossfade()
    {
        //_root.style.display = DisplayStyle.None; // I can't use it for transition within the menu then
        _crossfade.style.display = DisplayStyle.None;
    }
}
