using System.Collections;
using Lis.Core;
using UnityEngine;
using Lis.Core.Utilities;
using Lis.HeroCreation;
using Lis.Units.Hero;
using UnityEngine.Splines;

namespace Lis.Map
{
    public class PlayerController : Singleton<PlayerController>
    {
        [SerializeField] GameObject _maleHero;
        [SerializeField] GameObject _femaleHero;

        GameObject _activeBody;
        Animator _animator;
        int _animVelocityX;
        int _animVelocityZ;

        public NodeController CurrentNode;

        CameraController _mainCamera;

        bool _isMoving;

        void Start()
        {
            _mainCamera = CameraController.Instance;
            InitializeHeroGameObject(GameManager.Instance.CurrentCampaign.SelectedHero);
        }

        void InitializeHeroGameObject(Hero hero)
        {
            if (hero.VisualHero.BodyType == 0)
            {
                _femaleHero.SetActive(true);
                _activeBody = _femaleHero;
            }
            else
            {
                _maleHero.SetActive(true);
                _activeBody = _maleHero;
            }

            _animator = _activeBody.GetComponent<Animator>();
            _animVelocityX = Animator.StringToHash("VelocityX");
            _animVelocityZ = Animator.StringToHash("VelocityZ");

            ItemDisplayer id = _activeBody.GetComponent<ItemDisplayer>();
            id.SetVisualHero(hero.VisualHero);
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
            SetAnimationDirection(nodeController);
        }

        SplinePath GetPathTo(NodeController nodeController)
        {
            // connection goes both ways
            return CurrentNode.Node.IsConnectedTo(nodeController.Node)
                ? CurrentNode.GetPathTo(nodeController).Path
                : nodeController.GetPathTo(CurrentNode).ReversedPath;
        }


        void SetAnimationDirection(NodeController target)
        {
        }

        IEnumerator MoveOnPath(SplinePath path)
        {
            _isMoving = true;
            _animator.SetFloat(_animVelocityX, 5);
            // _animator.SetFloat(_animVelocityX, rightDot * blend * _animationBlend);

            float t = 0f;
            while (t < 1f)
            {
                Vector3 pos = path.EvaluatePosition(t);
                Vector3 direction = path.EvaluateTangent(t);

                _animator.SetFloat(_animVelocityX, direction.x * 5);
                _animator.SetFloat(_animVelocityZ, direction.y * 5);

                pos.z = -2;
                transform.position = pos;

                t += 0.5f * Time.deltaTime;
                yield return null;
            }

            _animator.SetFloat(_animVelocityX, 0);
            _animator.SetFloat(_animVelocityZ, 0);

            _isMoving = false;
            CurrentNode.Visited();
        }
    }
}