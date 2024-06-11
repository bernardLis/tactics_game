using UnityEngine;

namespace Lis.Core.Utilities
{
    public class CircularMotion : MonoBehaviour
    {
        public float StartAngle;
        public float RotationSpeed = 0.1f;
        public float CircleRadius = 800;
        public float ElevationOffset;

        [SerializeField] private float _angle;

        private Vector3 _positionOffset;

        private void Start()
        {
            _angle = StartAngle;
        }

        private void LateUpdate()
        {
            _positionOffset.Set(
                Mathf.Cos(_angle) * CircleRadius,
                ElevationOffset,
                Mathf.Sin(_angle) * CircleRadius
            );
            transform.position = Vector3.zero + _positionOffset;
            _angle += Time.deltaTime * RotationSpeed;
        }
    }
}