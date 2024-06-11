using UnityEngine;

namespace Lis.Core.Utilities
{
    public class CircularMotion : MonoBehaviour
    {
        public float StartAngle;
        public float RotationSpeed = 0.1f;
        public float CircleRadius = 800;
        public float ElevationOffset;

        [SerializeField] float _angle;

        Vector3 _positionOffset;

        void Start()
        {
            _angle = StartAngle;
        }

        void LateUpdate()
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