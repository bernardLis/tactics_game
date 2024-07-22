using System.Collections;
using UnityEngine;

namespace Lis.Map
{
    public class CameraFollowController : MonoBehaviour
    {
        PlayerController _playerController;

        IEnumerator _followPlayerCoroutine;

        void Start()
        {
            _playerController = PlayerController.Instance;
            FollowPlayer();
        }

        public void FollowPlayer()
        {
            _followPlayerCoroutine = FollowPlayerCoroutine();
            StartCoroutine(_followPlayerCoroutine);
        }

        IEnumerator FollowPlayerCoroutine()
        {
            while (this != null && _playerController != null)
            {
                Vector3 targetPosition = _playerController.transform.position;
                targetPosition.z = -2;
                transform.position = targetPosition;
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void StopFollowingPlayer()
        {
            StopCoroutine(_followPlayerCoroutine);
        }
    }
}