using Cinemachine;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lis.HeroCreation
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _defaultVirtualCamera;
        [SerializeField] CinemachineVirtualCamera _headVirtualCamera;

        public void LookAtHead()
        {
            _defaultVirtualCamera.Priority = 0;
            _headVirtualCamera.Priority = 1;
        }

        public void LookAtDefault()
        {
            _defaultVirtualCamera.Priority = 1;
            _headVirtualCamera.Priority = 0;
        }
    }
}