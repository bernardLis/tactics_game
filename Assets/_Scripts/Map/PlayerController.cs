using System.Collections;
using UnityEngine;
using DG.Tweening;
using Lis.Core.Utilities;
using UnityEngine.Splines;

namespace Lis.Map
{
    public class PlayerController : Singleton<PlayerController>
    {
        public NodeController CurrentNode;


        CameraController _mainCamera;

        bool _isMoving = false;

        void Start()
        {
            _mainCamera = CameraController.Instance;
        }

        public bool TryMovingPlayerToNode(NodeController nodeController)
        {
            if (CurrentNode == nodeController) return false;
            if (!CurrentNode.Node.IsConnectedTo(nodeController.Node)) return false;
            if (_isMoving) return false;

            _mainCamera.DefaultCamera();
            MoveTo(nodeController);
            return true;
        }

        void MoveTo(NodeController nodeController)
        {
            CurrentNode.Deactivate();

            SplinePath path = CurrentNode.GetPathTo(nodeController);
            StartCoroutine(MoveOnPath(path));
            CurrentNode = nodeController;
        }


        IEnumerator MoveOnPath(SplinePath path)
        {
            _isMoving = true;
            float t = 0f;
            while (t < 1f)
            {
                Vector3 pos = path.EvaluatePosition(t);
                pos.z = -2;
                transform.position = pos;

                t += 0.1f * Time.deltaTime;
                yield return null;
            }

            _isMoving = false;
        }
    }
}