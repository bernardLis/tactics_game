using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class BattleIntroCameraManager : Singleton<BattleIntroCameraManager>
{
    [SerializeField] CinemachineVirtualCamera _mainCamera;

    [SerializeField] List<GameObject> _lookAtTargets = new();

    CinemachineVirtualCamera _introCamera;
    CinemachineDollyCart _dollyCart;
    CinemachineSmoothPath _path;

    int _currentLookAtIndex = 0;

    public event Action OnIntroFinished;

    void Start()
    {
        _introCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _dollyCart = GetComponentInChildren<CinemachineDollyCart>();
        _path = GetComponentInChildren<CinemachineSmoothPath>();

        StartCoroutine(CameraIntroCoroutine());
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
