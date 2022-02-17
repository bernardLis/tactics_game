using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    UIDocument UIDocument;
    VisualElement crossfade;

    void Start()
    {
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        crossfade = root.Q<VisualElement>("crossfade");
    }

    public void ChangeScene(string _newScene)
    {
        // fade out opacity 0 -> 1
        crossfade.style.opacity = 0;
        crossfade.style.display = DisplayStyle.Flex;
        DOTween.To(() => crossfade.style.opacity.value, x => crossfade.style.opacity = x, 1f, 1f)
        .SetEase(Ease.InSine).OnComplete(() => FadeIn(_newScene));
    }

    public void FadeIn(string _newScene)
    {
        SceneManager.LoadScene(_newScene);

        // fade in opacity 1 -> 0
        crossfade.style.opacity = 1;
        crossfade.style.display = DisplayStyle.Flex;
        DOTween.To(() => crossfade.style.opacity.value, x => crossfade.style.opacity = x, 0f, 1f)
            .SetEase(Ease.InSine).OnComplete(HideCrossfade);
    }

    void HideCrossfade()
    {
        crossfade.style.display = DisplayStyle.None;
    }
}
