using System.Collections;
using UnityEngine;
using DG.Tweening;
using Lis.Core;
using Lis.Map.MapNodes;

namespace Lis.Map
{
    public class NodeControllerChest : NodeController
    {
        [SerializeField] Sound _chestOpenSound;
        [SerializeField] Transform _chest;


        protected override void ResolveNode()
        {
            base.ResolveNode();
            if (!Node.IsVisited) ShowChest();
        }

        void ShowChest()
        {
            _chest.DOKill();

            _chest.DOMoveY(1.5f, 0.5f).SetEase(Ease.InOutSine);
            _chest.DOLocalRotate(new(0, -180, 0), 0.5f).OnComplete(() =>
            {
                AudioManager.Instance.CreateSound().WithSound(_chestOpenSound)
                    .WithPosition(transform.position).Play();

                _chest.GetComponent<ChestController>().OpenChest();
            });

            StartCoroutine(ShowChestUICoroutine());
        }

        IEnumerator ShowChestUICoroutine()
        {
            yield return new WaitForSeconds(3f);

            ChestScreen chestScreen = new();
            chestScreen.InitializeChest((MapNodeChest)Node);
            chestScreen.OnHide += () =>
            {
                Node.NodeCompleted();
                _chest.DOScale(0f, 0.5f).SetEase(Ease.InOutBack)
                    .OnComplete(() => { Icon.gameObject.SetActive(true); });
                MapManager.EnableCampButton();
                PlayerController.IsMoving = false;
            };
        }

        public override void SetAvailable()
        {
            base.SetAvailable();

            _chest.transform.localRotation = Quaternion.Euler(new(0, 180, -5));
            _chest.DOLocalRotate(new(0, 180, 5), 0.5f)
                .SetEase(Ease.InOutBack)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0.1f, 0.5f));
        }

        public override void SetUnavailable()
        {
            base.SetUnavailable();
            if (this == PlayerController.CurrentNode) return;

            _chest.DOKill();
            // _chestJumpSequence.Kill();
            Icon.gameObject.SetActive(true);
            Gfx.DOScale(0f, 0.5f).SetEase(Ease.InOutBack);
        }
    }
}