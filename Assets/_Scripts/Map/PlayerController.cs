using System.Collections;
using DG.Tweening;
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
        MapManager _mapManager;

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
            _mapManager = MapManager.Instance;
            _mainCamera = CameraController.Instance;
            InitializeHeroGameObject(GameManager.Instance.Campaign.Hero);
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
            if (!nodeController.IsConnectedTo(CurrentNode)) return false;
            if (_isMoving) return false;

            _mainCamera.DefaultCamera();
            MoveTo(nodeController);
            _mapManager.ResolveNodes(nodeController);
            return true;
        }

        void MoveTo(NodeController nodeController)
        {
            SplinePath path = nodeController.GetPathTo(CurrentNode);
            CurrentNode = nodeController;
            StartCoroutine(MoveOnPath(path));
        }

        IEnumerator MoveOnPath(SplinePath path)
        {
            DOTween.Kill("current node disc tween");

            _isMoving = true;

            float t = 0f;
            while (t < 1f)
            {
                Vector3 pos = path.EvaluatePosition(t);
                Vector3 direction = path.EvaluateTangent(t);

                float forwardDot = Vector3.Dot(direction, transform.forward);
                float rightDot = Vector3.Dot(direction, transform.right);
                float forwardBlend = Mathf.Abs(forwardDot);
                float rightBlend = Mathf.Abs(rightDot);
                float blend = Mathf.Max(forwardBlend, rightBlend);
                _animator.SetFloat(_animVelocityZ, forwardDot * blend);
                _animator.SetFloat(_animVelocityX, rightDot * blend);

                transform.position = pos;

                t += 0.5f * Time.deltaTime;
                yield return null;
            }

            _animator.SetFloat(_animVelocityX, 0);
            _animator.SetFloat(_animVelocityZ, 0);

            _isMoving = false;
            CurrentNode.SetCurrentNode();
        }
    }
}