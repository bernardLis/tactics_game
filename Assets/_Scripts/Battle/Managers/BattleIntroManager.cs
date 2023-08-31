using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;

public class BattleIntroManager : Singleton<BattleIntroManager>
{
    BattleManager _battleManager;
    BattleInputManager _battleInputManager;

    [SerializeField] Sound _introVO;

    [SerializeField] CinemachineVirtualCamera _mainCamera;

    [SerializeField] List<GameObject> _lookAtTargets = new();

    CinemachineVirtualCamera _introCamera;
    CinemachineDollyCart _dollyCart;

    int _currentLookAtIndex = 0;

    TextPrintingElement _introTextElement;

    public event Action OnIntroFinished;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleInputManager = _battleManager.GetComponent<BattleInputManager>();
        _battleInputManager.OnContinueClicked += SkipIntro;

        _introCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _dollyCart = GetComponentInChildren<CinemachineDollyCart>();

        _introTextElement = new("You wake up confused. You have a feeling that your fate is connected to the structure in the middle. For some reason you think if you survive 10 minutes you will win and if enough enemies reach the middle you will lose. You don’t know where you are but you feel bad vibes from portals in the map corners. It is you and your trusty creature against this foreign place. Now spawn your creature and good luck.", 30f);
        _battleManager.Root.Add(_introTextElement);
        StartCoroutine(CameraIntroCoroutine());
        StartCoroutine(PlayIntroVO());
    }

    void OnDisable()
    {
        _battleInputManager.OnContinueClicked -= SkipIntro;
    }

    void SkipIntro()
    {
        if (this == null) return;
        StopAllCoroutines();
        AudioManager.Instance.StopDialogue();
        FinishIntro();
    }

    IEnumerator PlayIntroVO()
    {
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayDialogue(_introVO);
    }

    IEnumerator CameraIntroCoroutine()
    {
        // TODO: this is meh, but it works for now
        float nextBreak = 1f / (float)_lookAtTargets.Count;
        float breakPointEvery = nextBreak;

        while (_dollyCart.m_Position < 1) // normalized position
        {
            if (_dollyCart.m_Position >= nextBreak)
            {
                nextBreak += breakPointEvery;
                LookAtNextTarget();
            }
            yield return new WaitForSeconds(0.1f);
        }
        FinishIntro();
    }

    void FinishIntro()
    {
        _battleManager.Root.Remove(_introTextElement);

        _mainCamera.gameObject.SetActive(true);
        _introCamera.gameObject.SetActive(false);
        BattleCameraManager.Instance.enabled = true;
        OnIntroFinished?.Invoke();

    }

    void LookAtNextTarget()
    {
        _currentLookAtIndex++;
        _currentLookAtIndex = Mathf.Clamp(_currentLookAtIndex, 0, _lookAtTargets.Count - 1);
        _introCamera.LookAt = _lookAtTargets[_currentLookAtIndex].transform;
    }
}
