using UnityEngine;

namespace Lis.Core
{
    public class Billboard : MonoBehaviour
    {
        private Camera _cam;

        // Start is called before the first frame update
        private void Start()
        {
            _cam = Camera.main;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (!isActiveAndEnabled) return;
            transform.LookAt(transform.position + _cam.transform.forward);
        }
    }
}