using Cinemachine;
using UnityEngine;

namespace Lis.HeroCreation
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _defaultVirtualCamera;

        [SerializeField] CinemachineVirtualCamera _femaleVirtualCamera;
        [SerializeField] CinemachineVirtualCamera _femaleHeadVirtualCamera;

        [SerializeField] CinemachineVirtualCamera _maleVirtualCamera;
        [SerializeField] CinemachineVirtualCamera _maleHeadVirtualCamera;

        int _currentBodyType;

        public void SelectBodyType(int bodyType)
        {
            _currentBodyType = bodyType;
            _defaultVirtualCamera.Priority = 0;
            Invoke(nameof(LookAtDefault), 0.5f);
            LookAtDefault();
        }

        public void LookAtHead()
        {
            if (_currentBodyType == 0)
            {
                _femaleVirtualCamera.Priority = 0;
                _femaleHeadVirtualCamera.Priority = 1;
            }
            else
            {
                _maleVirtualCamera.Priority = 0;
                _maleHeadVirtualCamera.Priority = 1;
            }
        }

        public void LookAtDefault()
        {
            if (_currentBodyType == 0)
            {
                _femaleVirtualCamera.Priority = 1;
                _femaleHeadVirtualCamera.Priority = 0;
            }
            else
            {
                _maleVirtualCamera.Priority = 1;
                _maleHeadVirtualCamera.Priority = 0;
            }
        }
    }
}