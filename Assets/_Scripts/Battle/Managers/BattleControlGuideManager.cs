using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleControlGuideManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    VisualElement _root;

    VisualElement _controlGuideContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _root = _battleManager.Root;
        _controlGuideContainer = _root.Q<VisualElement>("controlGuideContainer");

        BattleIntroManager.Instance.OnIntroFinished += ShowControlGuide;
    }

    void ShowControlGuide()
    {
        DOTween.To(x => _controlGuideContainer.style.opacity = x, 0, 1, 0.6f).SetDelay(5f);
    }

}
