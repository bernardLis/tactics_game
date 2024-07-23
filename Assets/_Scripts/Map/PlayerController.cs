using System.Collections;
using UnityEngine;
using Lis.Core.Utilities;
using UnityEngine.Splines;

namespace Lis.Map
{
    public class PlayerController : Singleton<PlayerController>
    {
        public NodeController CurrentNode;


        CameraController _mainCamera;

        bool _isMoving;

        void Start()
        {
            _mainCamera = CameraController.Instance;
        }

        public bool TryMovingPlayerToNode(NodeController nodeController)
        {
            if (CurrentNode == nodeController) return false;
            if (!HasConnection(nodeController)) return false;
            if (_isMoving) return false;

            _mainCamera.DefaultCamera();
            MoveTo(nodeController);
            return true;
        }

        bool HasConnection(NodeController nodeController)
        {
            // connection goes both ways
            return CurrentNode.Node.IsConnectedTo(nodeController.Node) ||
                   nodeController.Node.IsConnectedTo(CurrentNode.Node);
        }

        void MoveTo(NodeController nodeController)
        {
            SplinePath path = GetPathTo(nodeController);
            CurrentNode = nodeController;
            StartCoroutine(MoveOnPath(path));
        }

        SplinePath GetPathTo(NodeController nodeController)
        {
            // connection goes both ways
            return CurrentNode.Node.IsConnectedTo(nodeController.Node)
                ? CurrentNode.GetPathTo(nodeController).Path
                : nodeController.GetPathTo(CurrentNode).ReversedPath;
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

                t += 0.5f * Time.deltaTime;
                yield return null;
            }

            _isMoving = false;
            CurrentNode.Visited();
        }
    }
}