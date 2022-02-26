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
    VisualElement Crossfade;

    void Start()
    {
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        Crossfade = root.Q<VisualElement>("crossfade");
    }

    public void LoadLevel(string newScene)
    {
        Debug.Log("load level");
        DOTween.KillAll();

        // fade out opacity 0 -> 1
        Crossfade.style.opacity = 0;
        Crossfade.style.display = DisplayStyle.Flex;
        DOTween.To(() => Crossfade.style.opacity.value, x => Crossfade.style.opacity = x, 1f, 1f)
            .SetEase(Ease.InSine).OnComplete(() => FadeIn(newScene));
    }

    void FadeIn(string newScene)
    {
        SceneManager.LoadScene(newScene);

        // fade in opacity 1 -> 0
        Crossfade.style.opacity = 1;
        Crossfade.style.display = DisplayStyle.Flex;
        DOTween.To(() => Crossfade.style.opacity.value, x => Crossfade.style.opacity = x, 0f, 1f)
            .SetEase(Ease.InSine).OnComplete(HideCrossfade);

    }

    void HideCrossfade()
    {
        Crossfade.style.display = DisplayStyle.None;
    }
}
