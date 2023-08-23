using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleIntroCameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _mainCamera;

    [SerializeField] GameObject LookAt1;
    [SerializeField] GameObject LookAt2;
    [SerializeField] GameObject LookAt3;
    [SerializeField] GameObject LookAt4;
    [SerializeField] GameObject LookAt5;
    [SerializeField] GameObject LookAt6;
    [SerializeField] GameObject LookAt7;

    CinemachineVirtualCamera _introCamera;
    CinemachineDollyCart _dollyCart;
    CinemachineSmoothPath _path;

    int _numberOfWaypoints;

    void Start()
    {
        _introCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _dollyCart = GetComponentInChildren<CinemachineDollyCart>();
        _path = GetComponentInChildren<CinemachineSmoothPath>();
        _numberOfWaypoints = _path.m_Waypoints.Length;

        StartCoroutine(CameraIntroCoroutine());
    }

    IEnumerator CameraIntroCoroutine()
    {
        float nextBreak = 1;
        while (_dollyCart.m_Position < _numberOfWaypoints)
        {
            if (_dollyCart.m_Position >= nextBreak)
            {
                nextBreak++;
                SetLookAt(_dollyCart.m_Position);
            }
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log($"end");
        _mainCamera.gameObject.SetActive(true);
        _introCamera.gameObject.SetActive(false);
    }

    void SetLookAt(float waypointIndex)
    {
        Debug.Log($"set look at");
        if (waypointIndex > 0 && waypointIndex < 1)
            _introCamera.LookAt = LookAt1.transform;
        else if (waypointIndex > 1 && waypointIndex < 2)
            _introCamera.LookAt = LookAt2.transform;
        else if (waypointIndex > 2 && waypointIndex < 3)
            _introCamera.LookAt = LookAt3.transform;
        else if (waypointIndex > 3 && waypointIndex < 4)
            _introCamera.LookAt = LookAt4.transform;
        else if (waypointIndex > 4 && waypointIndex < 5)
            _introCamera.LookAt = LookAt5.transform;
        else if (waypointIndex > 5 && waypointIndex < 6)
            _introCamera.LookAt = LookAt6.transform;
        else if (waypointIndex > 6 && waypointIndex < 7)
            _introCamera.LookAt = LookAt7.transform;
    }
}
